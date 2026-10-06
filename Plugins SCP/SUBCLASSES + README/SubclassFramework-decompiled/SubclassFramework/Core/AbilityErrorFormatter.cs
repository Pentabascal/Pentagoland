using Exiled.API.Features;

namespace SubclassFramework.Core;

internal static class AbilityErrorFormatter
{
    public static void Show(Player player, string message, float duration = 3f)
    {
        SfUtils.Hint(
            player,
            $"<size=22>{message}</size>",
            duration);
    }
}
