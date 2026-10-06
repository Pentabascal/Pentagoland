namespace VVUP.CustomRoles.API.ExternalTeams
{
    public static class CustomTeamAPI
    {
        public static bool SerpentsHandSpawnable => SerpentsHandCheck.IsSpawnable;
        public static bool UiuSpawnable => UiuCheck.IsSpawnable;
        
        public static readonly ExternalTeamChecker SerpentsHandCheck = new SerpentsHandCheck();
        public static readonly ExternalTeamChecker UiuCheck = new UiuCheck();
    }
}