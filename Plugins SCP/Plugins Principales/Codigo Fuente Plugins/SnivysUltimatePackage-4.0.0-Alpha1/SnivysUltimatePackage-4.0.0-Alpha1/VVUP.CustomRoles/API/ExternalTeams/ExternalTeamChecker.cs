using System.Reflection;

namespace VVUP.CustomRoles.API.ExternalTeams
{
    public abstract class ExternalTeamChecker
    {
        public abstract void Init(Assembly assembly);

        public bool IsSpawnable
        {
            get
            {
                if (!PluginEnabled)
                    return false;

                return (bool)FieldInfo.GetValue(Instance);
            }
        }
        protected bool PluginEnabled { get; set; }
        protected FieldInfo FieldInfo { get; set; }
        protected object Instance { get; set; }
    }
}