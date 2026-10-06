using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Pentagoland.ServerDoctor;

public static class YamlScalarEditor
{
    private static readonly Regex KeySegment = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);
    private static readonly Regex TypedScalar = new("^(?:true|false|null|~|-?(?:0|[1-9][0-9]*)(?:\\.[0-9]+)?)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static bool TrySetScalar(string yaml, string dottedKey, string value, out string updatedYaml, out string error)
    {
        updatedYaml = yaml;
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(dottedKey) || dottedKey.Split('.').Any(part => !KeySegment.IsMatch(part)))
        {
            error = "La clave debe usar segmentos simples separados por puntos, por ejemplo: plugin.enabled.";
            return false;
        }

        if (value.Any(character => char.IsControl(character) && character != '\t'))
        {
            error = "El valor no puede contener caracteres de control.";
            return false;
        }

        string[] wantedPath = dottedKey.Split('.');
        string newline = yaml.IndexOf("\r\n", StringComparison.Ordinal) >= 0 ? "\r\n" : "\n";
        string[] lines = Regex.Split(yaml, "\\r\\n|\\n|\\r");
        var parents = new List<(int Indent, string Key)>();
        var sequenceIndents = new List<int>();
        int matchCount = 0;
        string? preservedComment = null;

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#", StringComparison.Ordinal))
                continue;

            int indent = CountIndent(line);
            string trimmed = line.Substring(indent);
            while (sequenceIndents.Count > 0 && sequenceIndents[sequenceIndents.Count - 1] >= indent)
                sequenceIndents.RemoveAt(sequenceIndents.Count - 1);

            if (trimmed.StartsWith("- ", StringComparison.Ordinal) || trimmed == "-")
            {
                sequenceIndents.Add(indent);
                continue;
            }

            if (!TrySplitMapping(trimmed, out string key, out string rawValue, out int colonIndex))
                continue;

            while (parents.Count > 0 && parents[parents.Count - 1].Indent >= indent)
                parents.RemoveAt(parents.Count - 1);

            if (!IsSimpleKey(key))
                continue;

            var path = parents.Select(parent => parent.Key).Append(key).ToArray();
            int commentIndex = FindInlineComment(rawValue);
            string scalarValue = commentIndex >= 0 ? rawValue.Substring(0, commentIndex).TrimEnd() : rawValue;
            if (path.SequenceEqual(wantedPath, StringComparer.Ordinal))
            {
                if (sequenceIndents.Count > 0)
                {
                    error = "No se editan propiedades dentro de secuencias YAML.";
                    return false;
                }

                if (string.IsNullOrWhiteSpace(scalarValue))
                {
                    error = "La clave seleccionada es un bloque, no un valor escalar.";
                    return false;
                }

                if (!IsSupportedScalar(scalarValue))
                {
                    error = "Solo se pueden cambiar valores escalares YAML sencillos.";
                    return false;
                }

                matchCount++;
                preservedComment = commentIndex >= 0 ? rawValue.Substring(commentIndex) : null;
                string replacement = SerializeScalar(value);
                lines[i] = line.Substring(0, indent + colonIndex + 1) + " " + replacement +
                           (preservedComment is null ? string.Empty : " " + preservedComment.TrimStart());
                continue;
            }

            if (string.IsNullOrWhiteSpace(scalarValue))
                parents.Add((indent, key));
        }

        if (matchCount == 0)
        {
            error = "No se encontró una propiedad escalar con esa ruta.";
            return false;
        }

        if (matchCount > 1)
        {
            error = "La ruta coincide con más de una propiedad; no se modificó el archivo.";
            return false;
        }

        updatedYaml = string.Join(newline, lines);
        return true;
    }

    private static int CountIndent(string line)
    {
        int count = 0;
        while (count < line.Length && line[count] == ' ')
            count++;

        if (count < line.Length && line[count] == '\t')
            throw new FormatException("No se pueden editar archivos YAML con tabuladores de indentación.");

        return count;
    }

    private static bool TrySplitMapping(string line, out string key, out string value, out int colonIndex)
    {
        bool singleQuoted = false;
        bool doubleQuoted = false;
        bool escaped = false;

        for (int i = 0; i < line.Length; i++)
        {
            char current = line[i];
            if (doubleQuoted && current == '\\' && !escaped)
            {
                escaped = true;
                continue;
            }

            if (current == '\'' && !doubleQuoted)
                singleQuoted = !singleQuoted;
            else if (current == '"' && !singleQuoted && !escaped)
                doubleQuoted = !doubleQuoted;
            else if (current == ':' && !singleQuoted && !doubleQuoted && (i + 1 == line.Length || char.IsWhiteSpace(line[i + 1]) || line[i + 1] == '#'))
            {
                key = line.Substring(0, i).Trim();
                value = line.Substring(i + 1).Trim();
                colonIndex = i;
                return true;
            }

            escaped = false;
        }

        key = string.Empty;
        value = string.Empty;
        colonIndex = -1;
        return false;
    }

    private static bool IsSimpleKey(string key) => KeySegment.IsMatch(key);

    private static bool IsSupportedScalar(string value)
    {
        if (value.StartsWith("[", StringComparison.Ordinal) ||
            value.StartsWith("{", StringComparison.Ordinal) ||
            value.StartsWith("|", StringComparison.Ordinal) ||
            value.StartsWith(">", StringComparison.Ordinal) ||
            value.StartsWith("&", StringComparison.Ordinal) ||
            value.StartsWith("*", StringComparison.Ordinal) ||
            value.StartsWith("!", StringComparison.Ordinal))
            return false;

        int commentIndex = FindInlineComment(value);
        string scalar = commentIndex >= 0 ? value.Substring(0, commentIndex).TrimEnd() : value;
        if (scalar.Length == 0)
            return false;

        if (scalar.StartsWith("\"", StringComparison.Ordinal))
            return scalar.Length >= 2 && scalar.EndsWith("\"", StringComparison.Ordinal);
        if (scalar.StartsWith("'", StringComparison.Ordinal))
            return scalar.Length >= 2 && scalar.EndsWith("'", StringComparison.Ordinal);
        return true;
    }

    private static int FindInlineComment(string value)
    {
        bool singleQuoted = false;
        bool doubleQuoted = false;
        bool escaped = false;

        for (int i = 0; i < value.Length; i++)
        {
            char current = value[i];
            if (doubleQuoted && current == '\\' && !escaped)
            {
                escaped = true;
                continue;
            }

            if (current == '\'' && singleQuoted && i + 1 < value.Length && value[i + 1] == '\'')
            {
                i++;
                continue;
            }

            if (current == '\'' && !doubleQuoted)
                singleQuoted = !singleQuoted;
            else if (current == '"' && !singleQuoted && !escaped)
                doubleQuoted = !doubleQuoted;
            else if (current == '#' && !singleQuoted && !doubleQuoted && (i == 0 || char.IsWhiteSpace(value[i - 1])))
                return i;

            escaped = false;
        }

        return -1;
    }

    private static string SerializeScalar(string value)
    {
        string trimmed = value.Trim();
        if (TypedScalar.IsMatch(trimmed))
            return trimmed;

        var builder = new StringBuilder(trimmed.Length + 2).Append('"');
        foreach (char current in trimmed)
        {
            switch (current)
            {
                case '\\': builder.Append("\\\\"); break;
                case '"': builder.Append("\\\""); break;
                case '\t': builder.Append("\\t"); break;
                default: builder.Append(current); break;
            }
        }

        return builder.Append('"').ToString();
    }
}
