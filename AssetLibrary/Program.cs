using System.Globalization;
using System.Text.Json;

if (args.Length == 1 && args[0].Equals("--self-test", StringComparison.OrdinalIgnoreCase))
    return SchematicValidator.RunSelfTests();

if (args.Length != 1 || args[0] is "-h" or "--help")
{
    Console.WriteLine("Uso: dotnet run --project PentagolandMERTools.csproj -- <carpeta-Schematics>");
    Console.WriteLine("     dotnet run --project PentagolandMERTools.csproj -- --self-test");
    return args.Length == 1 ? 0 : 2;
}

try
{
    IReadOnlyList<ValidationDiagnostic> diagnostics = SchematicValidator.Validate(args[0]);
    foreach (ValidationDiagnostic diagnostic in diagnostics)
        Console.WriteLine($"{diagnostic.Severity}: {diagnostic.File}: {diagnostic.Message}");

    int errors = diagnostics.Count(diagnostic => diagnostic.Severity == "ERROR");
    int warnings = diagnostics.Count(diagnostic => diagnostic.Severity == "WARN");
    Console.WriteLine($"Resultado: {errors} errores, {warnings} avisos.");
    return errors == 0 ? 0 : 1;
}
catch (Exception exception) when (
    exception is IOException ||
    exception is UnauthorizedAccessException ||
    exception is ArgumentException ||
    exception is JsonException)
{
    Console.Error.WriteLine($"No se pudo validar la carpeta: {exception.Message}");
    return 2;
}

internal sealed record ValidationDiagnostic(string Severity, string File, string Message);

internal static class SchematicValidator
{
    private const int MaximumSchematicFolders = 2000;
    private const long MaximumJsonBytes = 16 * 1024 * 1024;
    private const int MaximumBlocks = 10000;
    private static readonly HashSet<string> PrimitiveNames = new(StringComparer.OrdinalIgnoreCase)
    {
        "Sphere", "Capsule", "Cylinder", "Cube", "Plane", "Quad"
    };

    public static IReadOnlyList<ValidationDiagnostic> Validate(string schematicsDirectory)
    {
        string root = Path.GetFullPath(schematicsDirectory);
        if (!Directory.Exists(root))
            throw new DirectoryNotFoundException($"No existe la carpeta '{root}'.");

        var diagnostics = new List<ValidationDiagnostic>();
        string[] folders = Directory.EnumerateDirectories(root, "*", SearchOption.TopDirectoryOnly)
            .Take(MaximumSchematicFolders + 1)
            .ToArray();
        if (folders.Length > MaximumSchematicFolders)
        {
            diagnostics.Add(new ValidationDiagnostic(
                "WARN",
                ".",
                $"Se limitaron las carpetas a {MaximumSchematicFolders}."));
        }

        foreach (string folder in folders.Take(MaximumSchematicFolders))
        {
            string folderName = Path.GetFileName(folder);
            if ((File.GetAttributes(folder) & FileAttributes.ReparsePoint) != 0)
            {
                diagnostics.Add(new ValidationDiagnostic("WARN", folderName, "Se omitió un enlace o punto de reanálisis."));
                continue;
            }

            string jsonPath = Path.Combine(folder, folderName + ".json");
            if (!File.Exists(jsonPath))
            {
                diagnostics.Add(new ValidationDiagnostic(
                    "ERROR",
                    folderName,
                    $"Falta el archivo esperado {folderName}.json."));
                continue;
            }

            ValidateSchematic(root, folder, jsonPath, diagnostics);
        }

        if (folders.Length == 0)
            diagnostics.Add(new ValidationDiagnostic("WARN", ".", "No se encontraron carpetas de esquemáticos."));

        foreach (string misplacedFile in Directory.EnumerateFiles(root, "*.json", SearchOption.TopDirectoryOnly))
        {
            string fileName = Path.GetFileName(misplacedFile);
            diagnostics.Add(new ValidationDiagnostic(
                "WARN",
                fileName,
                $"El JSON está directamente en Schematics; ProjectMER espera {Path.GetFileNameWithoutExtension(fileName)}\\{fileName}."));
        }

        if (diagnostics.Count == 0)
            diagnostics.Add(new ValidationDiagnostic("OK", ".", "No se encontraron problemas estructurales."));

        return diagnostics;
    }

    private static void ValidateSchematic(
        string root,
        string folder,
        string jsonPath,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        string relativeFolder = Path.GetRelativePath(root, folder);
        if ((File.GetAttributes(jsonPath) & FileAttributes.ReparsePoint) != 0)
        {
            diagnostics.Add(new ValidationDiagnostic("WARN", relativeFolder, "Se omitió un JSON que es enlace o punto de reanálisis."));
            return;
        }

        var info = new FileInfo(jsonPath);
        if (info.Length > MaximumJsonBytes)
        {
            diagnostics.Add(new ValidationDiagnostic(
                "ERROR",
                relativeFolder,
                $"El JSON supera el límite de {MaximumJsonBytes / 1024 / 1024} MiB."));
            return;
        }

        try
        {
            using JsonDocument document = JsonDocument.Parse(
                File.ReadAllText(jsonPath),
                new JsonDocumentOptions { MaxDepth = 64 });
            JsonElement schematic = document.RootElement;
            if (schematic.ValueKind != JsonValueKind.Object)
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, "La raíz JSON debe ser un objeto."));
                return;
            }

            ValidateNoDuplicateProperties(schematic, relativeFolder, "$", diagnostics);
            if (!TryGetProperty(schematic, "RootObjectId", out JsonElement rootId) || !TryInt32(rootId, out int rootObjectId))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, "Falta un RootObjectId entero."));
                return;
            }

            if (!TryGetProperty(schematic, "Blocks", out JsonElement blocks) || blocks.ValueKind != JsonValueKind.Array)
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, "Falta la lista Blocks."));
                return;
            }

            if (blocks.GetArrayLength() > MaximumBlocks)
            {
                diagnostics.Add(new ValidationDiagnostic(
                    "ERROR",
                    relativeFolder,
                    $"El esquemático supera el límite de {MaximumBlocks} bloques."));
                return;
            }

            ValidateBlocks(relativeFolder, rootObjectId, blocks, diagnostics);
            ValidateAnimatorReferences(relativeFolder, folder, blocks, diagnostics);
            ValidateRigidbodies(relativeFolder, folder, Path.GetFileNameWithoutExtension(jsonPath), diagnostics);
        }
        catch (JsonException exception)
        {
            diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, $"JSON inválido: {exception.Message}"));
        }
    }

    private static void ValidateBlocks(
        string file,
        int rootObjectId,
        JsonElement blocks,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        var knownIds = new HashSet<int>();
        var parents = new Dictionary<int, int>();
        int index = 0;
        foreach (JsonElement block in blocks.EnumerateArray())
        {
            string label = $"{file} [bloque {index}]";
            index++;
            if (block.ValueKind != JsonValueKind.Object)
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Cada bloque debe ser un objeto JSON."));
                continue;
            }

            if (!TryGetProperty(block, "Name", out JsonElement name) ||
                name.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(name.GetString()))
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Name es obligatorio y no puede estar vacío."));

            if (!TryGetProperty(block, "ObjectId", out JsonElement objectIdElement) ||
                !TryInt32(objectIdElement, out int objectId))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Falta un ObjectId entero."));
                continue;
            }

            if (!knownIds.Add(objectId))
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"ObjectId duplicado: {objectId}."));

            if (!TryGetProperty(block, "ParentId", out JsonElement parentIdElement) ||
                !TryInt32(parentIdElement, out int parentId))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Falta un ParentId entero."));
            }
            else
            {
                parents[objectId] = parentId;
            }

            ValidateRequiredVector(block, "Position", label, diagnostics);
            ValidateRequiredVector(block, "Rotation", label, diagnostics);
            if (!TryGetProperty(block, "Scale", out JsonElement scale) || !IsVector(scale, 3))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Scale es obligatorio y debe contener tres componentes numéricos."));
            }
            else if (!TryVectorComponents(scale, 3, out double[] components) ||
                     components.Any(component => component <= 0 || component > 1000))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Scale debe contener dimensiones positivas no superiores a 1000."));
            }

            if (!TryGetProperty(block, "BlockType", out JsonElement blockTypeElement) ||
                !TryBlockType(blockTypeElement, out int blockType))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Falta un BlockType reconocido."));
                continue;
            }

            if (blockType is 5 or 6 or 7)
            {
                diagnostics.Add(new ValidationDiagnostic(
                    "WARN",
                    label,
                    $"BlockType {blockType} está declarado por ProjectMER, pero su creador de bloques local cae al objeto vacío."));
            }
            else if (blockType < 0 || blockType > 10)
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"BlockType fuera del rango conocido: {blockType}."));
            }

            ValidateBlockProperties(label, blockType, block, diagnostics);
        }

        foreach (KeyValuePair<int, int> parent in parents)
        {
            if (parent.Key != rootObjectId && parent.Value != rootObjectId && !knownIds.Contains(parent.Value))
                diagnostics.Add(new ValidationDiagnostic(
                    "ERROR",
                    file,
                    $"El bloque {parent.Key} referencia ParentId inexistente {parent.Value}."));

            if (parent.Key == parent.Value)
                diagnostics.Add(new ValidationDiagnostic("ERROR", file, $"El bloque {parent.Key} es su propio padre."));
        }

        foreach (int id in parents.Keys)
        {
            var visited = new HashSet<int>();
            int current = id;
            while (current != rootObjectId && parents.TryGetValue(current, out int parentId))
            {
                if (!visited.Add(current))
                {
                    diagnostics.Add(new ValidationDiagnostic("ERROR", file, $"Se detectó un ciclo en la jerarquía de bloques desde {id}."));
                    break;
                }
                current = parentId;
            }
        }
    }

    private static void ValidateBlockProperties(
        string label,
        int blockType,
        JsonElement block,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (!TryGetProperty(block, "Properties", out JsonElement properties) ||
            properties.ValueKind != JsonValueKind.Object)
        {
            if (blockType is 1 or 2 or 3 or 8 or 9)
                diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Este BlockType necesita un objeto Properties."));
            return;
        }

        switch (blockType)
        {
            case 1:
                RequireScalar(properties, "PrimitiveType", label, diagnostics);
                RequireString(properties, "Color", label, diagnostics);
                if (TryGetProperty(properties, "PrimitiveType", out JsonElement primitiveType) &&
                    !IsKnownPrimitive(primitiveType))
                    diagnostics.Add(new ValidationDiagnostic("ERROR", label, "PrimitiveType no es un tipo geométrico Unity conocido."));
                if (TryGetProperty(properties, "Color", out JsonElement primitiveColor))
                    ValidateHtmlColor(primitiveColor, "Color", label, diagnostics);
                if (TryGetProperty(properties, "PrimitiveFlags", out JsonElement primitiveFlags) &&
                    (!TryInt32(primitiveFlags, out int flags) || flags is < 0 or > 3))
                    diagnostics.Add(new ValidationDiagnostic(
                        "ERROR",
                        label,
                        "PrimitiveFlags solo puede combinar Visible (1) y Collidable (2), por lo que debe estar entre 0 y 3."));
                break;
            case 2:
                RequireString(properties, "Color", label, diagnostics);
                RequireNumber(properties, "Intensity", label, diagnostics);
                RequireNumber(properties, "Range", label, diagnostics);
                if (TryGetProperty(properties, "Color", out JsonElement lightColor))
                    ValidateHtmlColor(lightColor, "Color", label, diagnostics);
                ValidateNonNegative(properties, "Intensity", label, diagnostics);
                ValidateNonNegative(properties, "Range", label, diagnostics);
                if (TryGetProperty(properties, "Shadows", out JsonElement shadows))
                {
                    if (shadows.ValueKind is not (JsonValueKind.True or JsonValueKind.False))
                        diagnostics.Add(new ValidationDiagnostic("ERROR", label, "Properties.Shadows debe ser booleano."));
                }
                else
                {
                    RequireNumber(properties, "ShadowType", label, diagnostics);
                    RequireNumber(properties, "Shape", label, diagnostics);
                    RequireNumber(properties, "SpotAngle", label, diagnostics);
                    RequireNumber(properties, "InnerSpotAngle", label, diagnostics);
                    RequireNumber(properties, "ShadowStrength", label, diagnostics);
                    ValidateNonNegative(properties, "SpotAngle", label, diagnostics);
                    ValidateNonNegative(properties, "InnerSpotAngle", label, diagnostics);
                }
                break;
            case 3:
                RequireScalar(properties, "ItemType", label, diagnostics);
                break;
            case 8:
                RequireString(properties, "Text", label, diagnostics);
                if (!TryGetProperty(properties, "DisplaySize", out JsonElement displaySize) || !IsVector(displaySize, 2))
                    diagnostics.Add(new ValidationDiagnostic("ERROR", label, "DisplaySize debe contener dos componentes numéricos."));
                else if (!TryVectorComponents(displaySize, 2, out double[] textSize) ||
                         textSize.Any(component => component <= 0 || component > 100))
                    diagnostics.Add(new ValidationDiagnostic("ERROR", label, "DisplaySize debe ser positivo y no superar 100 en cada eje."));
                break;
            case 9:
                RequireScalar(properties, "Shape", label, diagnostics);
                RequireNumber(properties, "InteractionDuration", label, diagnostics);
                ValidateNonNegative(properties, "InteractionDuration", label, diagnostics);
                break;
        }
    }

    private static void ValidateRequiredVector(
        JsonElement block,
        string name,
        string label,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (!TryGetProperty(block, name, out JsonElement vector) || !IsVector(vector, 3))
            diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"{name} es obligatorio y debe contener tres componentes numéricos."));
    }

    private static void ValidateHtmlColor(
        JsonElement value,
        string propertyName,
        string label,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        string? color = value.ValueKind == JsonValueKind.String ? value.GetString() : null;
        if (color is null ||
            color.Length is not (7 or 9) ||
            color[0] != '#' ||
            color.AsSpan(1).IndexOfAnyExcept("0123456789abcdefABCDEF") >= 0)
            diagnostics.Add(new ValidationDiagnostic(
                "ERROR",
                label,
                $"Properties.{propertyName} debe ser un color HTML #RRGGBB o #RRGGBBAA."));
    }

    private static void ValidateNonNegative(
        JsonElement properties,
        string name,
        string label,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (TryGetProperty(properties, name, out JsonElement value) &&
            IsNumber(value) &&
            value.GetDouble() < 0)
            diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"Properties.{name} no puede ser negativo."));
    }

    private static void ValidateAnimatorReferences(
        string relativeFolder,
        string folder,
        JsonElement blocks,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        foreach (JsonElement block in blocks.EnumerateArray())
        {
            if (!TryGetProperty(block, "AnimatorName", out JsonElement animator) ||
                animator.ValueKind != JsonValueKind.String ||
                string.IsNullOrWhiteSpace(animator.GetString()))
                continue;

            string name = animator.GetString()!;
            if (Path.IsPathRooted(name) || name.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).Any(part => part == ".."))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, "AnimatorName no puede escapar de la carpeta del esquemático."));
                continue;
            }

            string animatorPath = Path.GetFullPath(Path.Combine(folder, name));
            if (!IsWithinOrEqual(folder, animatorPath))
            {
                diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, "AnimatorName queda fuera de la carpeta del esquemático."));
            }
            else if (!File.Exists(animatorPath))
            {
                diagnostics.Add(new ValidationDiagnostic(
                    "WARN",
                    relativeFolder,
                    $"No se encontró el archivo local de animación '{name}'; podría pertenecer a un AssetBundle ya cargado."));
            }
        }
    }

    private static void ValidateRigidbodies(
        string relativeFolder,
        string folder,
        string schematicName,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        string path = Path.Combine(folder, schematicName + "-Rigidbodies.json");
        if (!File.Exists(path))
            return;
        if ((File.GetAttributes(path) & FileAttributes.ReparsePoint) != 0)
        {
            diagnostics.Add(new ValidationDiagnostic("WARN", relativeFolder, "Se omitió el JSON de rigidbodies por ser un enlace o punto de reanálisis."));
            return;
        }
        if (new FileInfo(path).Length > MaximumJsonBytes)
        {
            diagnostics.Add(new ValidationDiagnostic(
                "ERROR",
                relativeFolder,
                $"El JSON de rigidbodies supera el límite de {MaximumJsonBytes / 1024 / 1024} MiB."));
            return;
        }
        try
        {
            using JsonDocument document = JsonDocument.Parse(File.ReadAllText(path));
            if (document.RootElement.ValueKind != JsonValueKind.Object)
                diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, "El archivo de rigidbodies debe ser un objeto JSON."));
        }
        catch (JsonException exception)
        {
            diagnostics.Add(new ValidationDiagnostic("ERROR", relativeFolder, $"JSON de rigidbodies inválido: {exception.Message}"));
        }
    }

    private static bool IsKnownPrimitive(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.String)
            return PrimitiveNames.Contains(value.GetString() ?? string.Empty);
        return TryInt32(value, out int number) && number is >= 0 and <= 5;
    }

    private static bool TryBlockType(JsonElement value, out int blockType)
    {
        if (TryInt32(value, out blockType))
            return true;

        if (value.ValueKind == JsonValueKind.String &&
            Enum.TryParse(value.GetString(), ignoreCase: true, out ProjectMerBlockType parsed))
        {
            blockType = (int)parsed;
            return true;
        }

        blockType = -1;
        return false;
    }

    private static bool IsVector(JsonElement value, int componentCount)
    {
        if (value.ValueKind == JsonValueKind.Array)
            return value.GetArrayLength() == componentCount && value.EnumerateArray().All(IsNumber);

        if (value.ValueKind != JsonValueKind.Object)
            return false;

        string[] names = componentCount == 2 ? ["x", "y"] : ["x", "y", "z"];
        return names.All(name => TryGetProperty(value, name, out JsonElement component) && IsNumber(component));
    }

    private static bool TryVectorComponents(JsonElement value, int componentCount, out double[] components)
    {
        components = new double[componentCount];
        if (value.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (JsonElement component in value.EnumerateArray())
                components[index++] = component.GetDouble();
            return index == componentCount;
        }

        string[] names = componentCount == 2 ? ["x", "y"] : ["x", "y", "z"];
        for (int index = 0; index < names.Length; index++)
        {
            if (!TryGetProperty(value, names[index], out JsonElement component) || !IsNumber(component))
                return false;
            components[index] = component.GetDouble();
        }
        return true;
    }

    private static bool IsNumber(JsonElement value) =>
        value.ValueKind == JsonValueKind.Number && value.TryGetDouble(out double number) && double.IsFinite(number);

    private static void RequireString(
        JsonElement properties,
        string name,
        string label,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (!TryGetProperty(properties, name, out JsonElement value) || value.ValueKind != JsonValueKind.String)
            diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"Properties.{name} debe ser texto."));
    }

    private static void RequireNumber(
        JsonElement properties,
        string name,
        string label,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (!TryGetProperty(properties, name, out JsonElement value) || !IsNumber(value))
            diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"Properties.{name} debe ser numérico."));
    }

    private static void RequireScalar(
        JsonElement properties,
        string name,
        string label,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (!TryGetProperty(properties, name, out JsonElement value) ||
            value.ValueKind is not (JsonValueKind.String or JsonValueKind.Number))
            diagnostics.Add(new ValidationDiagnostic("ERROR", label, $"Properties.{name} debe ser texto o número."));
    }

    private static bool TryGetProperty(JsonElement element, string name, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            string normalizedName = NormalizePropertyName(name);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                if (NormalizePropertyName(property.Name).Equals(normalizedName, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static void ValidateNoDuplicateProperties(
        JsonElement element,
        string file,
        string location,
        ICollection<ValidationDiagnostic> diagnostics)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (JsonProperty property in element.EnumerateObject())
            {
                string normalizedName = NormalizePropertyName(property.Name);
                if (!seen.Add(normalizedName))
                    diagnostics.Add(new ValidationDiagnostic(
                        "ERROR",
                        file,
                        $"Propiedad JSON duplicada o ambigua en {location}: {property.Name}."));
                ValidateNoDuplicateProperties(property.Value, file, location + "." + property.Name, diagnostics);
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (JsonElement item in element.EnumerateArray())
                ValidateNoDuplicateProperties(item, file, $"{location}[{index++}]", diagnostics);
        }
    }

    private static string NormalizePropertyName(string value) =>
        value.Replace("_", string.Empty, StringComparison.Ordinal)
             .Replace("-", string.Empty, StringComparison.Ordinal);

    private static bool TryInt32(JsonElement value, out int result)
    {
        if (value.ValueKind == JsonValueKind.Number)
            return value.TryGetInt32(out result);
        if (value.ValueKind == JsonValueKind.String)
            return int.TryParse(value.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        result = 0;
        return false;
    }

    private static bool IsWithinOrEqual(string root, string path)
    {
        string normalizedRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        string normalizedPath = Path.GetFullPath(path).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        return string.Equals(normalizedRoot, normalizedPath, StringComparison.OrdinalIgnoreCase) ||
               normalizedPath.StartsWith(normalizedRoot + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
    }

    public static int RunSelfTests()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), "pentagoland-mer-validator-" + Guid.NewGuid().ToString("N"));
        string schematics = Path.Combine(tempRoot, "Schematics");
        string validFolder = Path.Combine(schematics, "Console");
        string invalidFolder = Path.Combine(schematics, "Broken");
        string escapingFolder = Path.Combine(schematics, "Escaping");
        string circularFolder = Path.Combine(schematics, "Circular");
        string duplicateFolder = Path.Combine(schematics, "Duplicate");
        string invalidPrimitiveFolder = Path.Combine(schematics, "InvalidPrimitive");
        string invalidLightFolder = Path.Combine(schematics, "InvalidLight");
        Directory.CreateDirectory(validFolder);
        Directory.CreateDirectory(invalidFolder);
        Directory.CreateDirectory(escapingFolder);
        Directory.CreateDirectory(circularFolder);
        Directory.CreateDirectory(duplicateFolder);
        Directory.CreateDirectory(invalidPrimitiveFolder);
        Directory.CreateDirectory(invalidLightFolder);
        try
        {
            File.WriteAllText(
                Path.Combine(validFolder, "Console.json"),
                """
                {
                  "root_object_id": 0,
                  "blocks": [
                    {
                      "name": "Valid primitive",
                      "object_id": 1,
                      "parent_id": 0,
                      "position": { "x": 0, "y": 0, "z": 0 },
                      "rotation": { "x": 0, "y": 0, "z": 0 },
                      "scale": { "x": 1, "y": 1, "z": 1 },
                      "block_type": 1,
                      "properties": { "primitive_type": 3, "color": "#202832" }
                    }
                  ]
                }
                """);
            File.WriteAllText(
                Path.Combine(invalidFolder, "Broken.json"),
                """
                {
                  "root_object_id": 0,
                  "blocks": [
                    {
                      "name": "Text without display size",
                      "object_id": 2,
                      "parent_id": 999,
                      "position": { "x": 0, "y": 0, "z": 0 },
                      "rotation": { "x": 0, "y": 0, "z": 0 },
                      "scale": { "x": 1, "y": 1, "z": 1 },
                      "block_type": 8,
                      "properties": { "text": "missing display size" }
                    }
                  ]
                }
                """);
            File.WriteAllText(
                Path.Combine(invalidPrimitiveFolder, "InvalidPrimitive.json"),
                """
                {
                  "RootObjectId": 0,
                  "Blocks": [
                    {
                      "Name": "Invalid primitive",
                      "ObjectId": 1,
                      "ParentId": 0,
                      "Position": { "x": 0, "y": 0, "z": 0 },
                      "Rotation": { "x": 0, "y": 0, "z": 0 },
                      "Scale": { "x": 1, "y": 1, "z": 1 },
                      "BlockType": 1,
                      "Properties": { "PrimitiveType": 3, "Color": "not-a-color", "PrimitiveFlags": 8 }
                    }
                  ]
                }
                """);
            File.WriteAllText(
                Path.Combine(invalidLightFolder, "InvalidLight.json"),
                """
                {
                  "RootObjectId": 0,
                  "Blocks": [
                    {
                      "Name": "Incomplete light",
                      "ObjectId": 1,
                      "ParentId": 0,
                      "Position": { "x": 0, "y": 0, "z": 0 },
                      "Rotation": { "x": 0, "y": 0, "z": 0 },
                      "Scale": { "x": 1, "y": 1, "z": 1 },
                      "BlockType": 2,
                      "Properties": { "Color": "#FFFFFF", "Intensity": 1, "Range": 3 }
                    }
                  ]
                }
                """);
            File.WriteAllText(
                Path.Combine(escapingFolder, "Escaping.json"),
                """
                {
                  "root_object_id": 0,
                  "blocks": [
                    {
                      "object_id": 1,
                      "parent_id": 0,
                      "block_type": 0,
                      "animator_name": "../outside.bundle"
                    }
                  ]
                }
                """);
            File.WriteAllText(
                Path.Combine(circularFolder, "Circular.json"),
                """
                {
                  "RootObjectId": 0,
                  "Blocks": [
                    { "ObjectId": 1, "ParentId": 2, "BlockType": 5, "Properties": {} },
                    { "ObjectId": 2, "ParentId": 1, "BlockType": 0, "Properties": {} }
                  ]
                }
                """);
            File.WriteAllText(
                Path.Combine(duplicateFolder, "Duplicate.json"),
                """
                {
                  "RootObjectId": 0,
                  "root_object_id": 1,
                  "Blocks": []
                }
                """);

            IReadOnlyList<ValidationDiagnostic> diagnostics = Validate(schematics);
            bool detectsOrphan = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.Message.Contains("ParentId inexistente 999", StringComparison.Ordinal));
            bool detectsMissingProperty = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.Message.Contains("DisplaySize", StringComparison.Ordinal));
            bool acceptsValidSchematic = !diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" &&
                diagnostic.File.StartsWith("Console", StringComparison.Ordinal));
            bool rejectsAnimatorTraversal = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.File.StartsWith("Escaping", StringComparison.Ordinal) &&
                diagnostic.Message.Contains("AnimatorName", StringComparison.Ordinal));
            bool detectsCycle = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.File.StartsWith("Circular", StringComparison.Ordinal) &&
                diagnostic.Message.Contains("ciclo", StringComparison.OrdinalIgnoreCase));
            bool warnsUnsupportedBlock = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "WARN" && diagnostic.File.Contains("Circular", StringComparison.Ordinal) &&
                diagnostic.Message.Contains("cae al objeto vacío", StringComparison.Ordinal));
            bool detectsDuplicateProperty = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.File.StartsWith("Duplicate", StringComparison.Ordinal) &&
                diagnostic.Message.Contains("Propiedad JSON duplicada", StringComparison.Ordinal));
            bool rejectsInvalidPrimitive = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.File.StartsWith("InvalidPrimitive", StringComparison.Ordinal) &&
                diagnostic.Message.Contains("PrimitiveFlags", StringComparison.Ordinal)) &&
                diagnostics.Any(diagnostic =>
                    diagnostic.Severity == "ERROR" && diagnostic.File.StartsWith("InvalidPrimitive", StringComparison.Ordinal) &&
                    diagnostic.Message.Contains("color HTML", StringComparison.Ordinal));
            bool rejectsIncompleteLight = diagnostics.Any(diagnostic =>
                diagnostic.Severity == "ERROR" && diagnostic.File.StartsWith("InvalidLight", StringComparison.Ordinal) &&
                diagnostic.Message.Contains("ShadowType", StringComparison.Ordinal));
            if (!detectsOrphan || !detectsMissingProperty || !acceptsValidSchematic ||
                !rejectsAnimatorTraversal || !detectsCycle || !warnsUnsupportedBlock || !detectsDuplicateProperty ||
                !rejectsInvalidPrimitive || !rejectsIncompleteLight)
            {
                Console.Error.WriteLine("Self-test fallido: el validador no detectó todos los casos esperados.");
                foreach (ValidationDiagnostic diagnostic in diagnostics)
                    Console.Error.WriteLine($"{diagnostic.Severity}: {diagnostic.File}: {diagnostic.Message}");
                return 1;
            }

            Console.WriteLine("Self-test correcto: comprueba JSON, jerarquías, propiedades MER, colores, flags, rutas y bloques no implementados.");
            return 0;
        }
        finally
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, recursive: true);
        }
    }

    private enum ProjectMerBlockType
    {
        Empty = 0,
        Primitive = 1,
        Light = 2,
        Pickup = 3,
        Workstation = 4,
        Schematic = 5,
        Teleport = 6,
        Locker = 7,
        Text = 8,
        Interactable = 9,
        Waypoint = 10
    }
}
