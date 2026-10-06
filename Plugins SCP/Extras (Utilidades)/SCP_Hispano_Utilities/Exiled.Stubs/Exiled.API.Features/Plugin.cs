using System;
using Exiled.API.Interfaces;

namespace Exiled.API.Features
{
    public abstract class Plugin<TConfig> where TConfig : IConfig, new()
    {
        public TConfig Config { get; protected set; } = new TConfig();

        public abstract string Name { get; }
        public abstract string Author { get; }
        public abstract Version Version { get; }
        public abstract Version RequiredExiledVersion { get; }

        public virtual void OnEnabled()
        {
            Config ??= new TConfig();
            if (!Config.IsEnabled)
                return;
        }

        public virtual void OnDisabled()
        {
        }
    }
}
