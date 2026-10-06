using System.Collections.Generic;

namespace Pentagoland.ServerDoctor;

public sealed class ServerDoctorConfig
{
    public string AdminPermission { get; set; } = "pentagoland.serverdoctor";
    public string ServerRootPath { get; set; } = string.Empty;
    public bool AllowConfigChanges { get; set; }
    public List<string> WritableRoots { get; set; } = [];
    public List<string> PluginScanRoots { get; set; } =
        ["labapi-plugins", "labapi-user-plugins", "labapi-server-plugins", "exiled-plugins", "exiled-server-plugins"];
    public List<string> DependencyScanRoots { get; set; } =
        ["labapi-dependencies", "labapi-user-dependencies", "labapi-server-dependencies"];
    public List<AdditionalRoot> AdditionalRoots { get; set; } = [];
    public int MaximumBackupCount { get; set; } = 200;
    public int MaximumConfigFileBytes { get; set; } = 1048576;
}

public sealed class AdditionalRoot
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}
