namespace Pentagoland.ServerDoctor;

public interface IConfigEditorAdapter
{
    string Name { get; }
    bool Supports(string rootName, string relativePath, string extension);
    bool TrySetValue(string content, string key, string value, out string updatedContent, out string error);
}

public sealed class YamlScalarConfigEditor : IConfigEditorAdapter
{
    public string Name => "YAML scalar";

    public bool Supports(string rootName, string relativePath, string extension) =>
        extension.Equals(".yml", System.StringComparison.OrdinalIgnoreCase) ||
        extension.Equals(".yaml", System.StringComparison.OrdinalIgnoreCase);

    public bool TrySetValue(string content, string key, string value, out string updatedContent, out string error) =>
        YamlScalarEditor.TrySetScalar(content, key, value, out updatedContent, out error);
}

public sealed class ScpslTextConfigEditor : IConfigEditorAdapter
{
    public string Name => "SCP:SL config scalar";

    public bool Supports(string rootName, string relativePath, string extension)
    {
        if ((!rootName.Equals("scpsl-config", System.StringComparison.OrdinalIgnoreCase) &&
             !rootName.Equals("scpsl-user-config", System.StringComparison.OrdinalIgnoreCase)) ||
            !extension.Equals(".txt", System.StringComparison.OrdinalIgnoreCase))
            return false;

        string normalizedPath = relativePath.Replace('\\', '/');
        int separator = normalizedPath.LastIndexOf('/');
        string fileName = separator < 0 ? normalizedPath : normalizedPath.Substring(separator + 1);
        return fileName.Equals("config_gameplay.txt", System.StringComparison.OrdinalIgnoreCase);
    }

    public bool TrySetValue(string content, string key, string value, out string updatedContent, out string error) =>
        YamlScalarEditor.TrySetScalar(content, key, value, out updatedContent, out error);
}
