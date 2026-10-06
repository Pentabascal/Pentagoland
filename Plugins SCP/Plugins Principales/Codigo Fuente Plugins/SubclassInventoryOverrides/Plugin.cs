using System;
using Exiled.API.Features;

namespace SubclassInventoryOverrides;

public sealed class Plugin : Plugin<Config>
{
    public override string Name => "Subclass Inventory Overrides";

    public override string Prefix => "subclass_inventory_overrides";

    public override string Author => "Copilot";

    public override Version Version => new(1, 0, 0);

    public override Version RequiredExiledVersion => new(9, 14, 2);

    public override void OnEnabled()
    {
        if (!LoadoutService.Initialize(Config))
        {
            Log.Error("Subclass Inventory Overrides could not initialize; see the loadout configuration errors above.");
            base.OnDisabled();
            return;
        }

        LoadoutService.Start();
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        LoadoutService.Stop();
        base.OnDisabled();
    }
}
