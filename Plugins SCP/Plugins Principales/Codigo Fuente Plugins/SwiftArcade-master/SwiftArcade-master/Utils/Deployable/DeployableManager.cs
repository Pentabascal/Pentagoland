using LabApi.Features.Console;
using System.Collections.Generic;

namespace SwiftArcadeMode.Utils.Deployable
{
    public static class DeployableManager
    {
        public static readonly List<DeployableBase> AllDeployables = [];

        public static void Tick()
        {
            for (int i = 0; i < AllDeployables.Count; i++)
                try
                {
                    AllDeployables[i].Tick();
                }
                catch (System.Exception ex)
                {
                    Logger.Error(ex);
                }
        }
    }
}
