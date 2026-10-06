using Exiled.API.Interfaces;

namespace VVUPCustomItemRueIBridge
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;

        public bool Debug { get; set; }
    }
}
