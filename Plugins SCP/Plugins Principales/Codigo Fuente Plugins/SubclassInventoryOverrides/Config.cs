using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;

namespace SubclassInventoryOverrides;

public sealed class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;

    public bool Debug { get; set; } = false;

    [Description("Loadout definitions keyed by SubclassFramework subclass ID.")]
    public List<LoadoutDefinition> Loadouts { get; set; } = new()
    {
        new LoadoutDefinition
        {
            SubclassId = "demoledor",
            ReplaceInventory = false,
            Items = new List<string>
            {
                "custom:46",
            },
        },
        new LoadoutDefinition
        {
            SubclassId = "francotirador_mtf",
            ReplaceInventory = true,
            Items = new List<string>
            {
                "KeycardMTFOperative",
                "ArmorLight",
                "Radio",
                "custom:58",
                "Medkit",
            },
            Ammo = new Dictionary<string, ushort>
            {
                ["Nato556"] = 50,
            },
        },
    };
}

public sealed class LoadoutDefinition
{
    [Description("The case-insensitive ID exposed by SubclassFramework.")]
    public string SubclassId { get; set; } = string.Empty;

    [Description("If true, this list is the complete inventory. If false, keep the subclass's native inventory and append these items.")]
    public bool ReplaceInventory { get; set; } = true;

    [Description("Vanilla ItemType names, or custom:<id> for an item registered through EXILED CustomItems.")]
    public List<string> Items { get; set; } = new();

    [Description("Reserve ammo by AmmoType name, for example Nato9 or Nato556.")]
    public Dictionary<string, ushort> Ammo { get; set; } = new();
}
