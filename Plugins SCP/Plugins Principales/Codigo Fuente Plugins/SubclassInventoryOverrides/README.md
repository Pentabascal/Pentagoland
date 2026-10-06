# Subclass Inventory Overrides

This EXILED plugin reconciles inventories after every SubclassFramework assignment. It rebuilds the framework's native `Items`, `ExtraItems`, and `Ammo` loadout, and restores the role's captured spawn inventory for subclasses that only add extra items. This prevents equipment from a previous subclass assignment from carrying over.

Configure entries in `Configs/Plugins/subclass_inventory_overrides/7777.yml`:

- `subclass_id`: the subclass ID used by SubclassFramework.
- `replace_inventory`: when true, the listed items and ammo are the complete loadout. When false, apply the framework's native subclass loadout first, then append the listed items and ammo.
- `items`: vanilla `ItemType` names, or `custom:<id>` for a registered EXILED CustomItem.
- `ammo`: reserve ammo keyed by `AmmoType` names.

The default `demoledor` entry keeps the framework's Chaos Repressor inventory and adds custom item ID 46 (ADATS). Its built-in role weapon, keycard, two HE grenades, combat armor, medkit, and ammunition are preserved.

The default `francotirador_mtf` entry follows the framework sniper kit: MTF operative keycard, light armor, radio, custom item ID 58 (EL-95), medkit, and 50 rounds of 5.56 mm ammunition.

Other subclasses use their native framework inventory automatically, including declared extra items and reserve ammo. The two assignment-time item grants in the framework (Demoledor and Infiltrado) are preserved during reconciliation.

Custom items are not spawned unless their ID is registered. Invalid item or ammo names invalidate that loadout entry and are reported in the server log.
