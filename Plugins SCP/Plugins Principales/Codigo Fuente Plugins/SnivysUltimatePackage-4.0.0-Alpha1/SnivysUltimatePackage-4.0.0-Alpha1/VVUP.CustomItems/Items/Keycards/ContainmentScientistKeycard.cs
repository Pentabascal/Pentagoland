/*using System.Collections.Generic;
using Exiled.API.Enums;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.Features;
using UnityEngine;
using YamlDotNet.Serialization;

namespace SnivysUltimatePackage.Custom.Items.Keycards
{
    [CustomItem(ItemType.KeycardCustomSite02)]
    public class ContainmentScientistKeycard : CustomKeycard
    {
        [YamlIgnore] 
        public override ItemType Type { get; set; } = ItemType.KeycardCustomSite02;
        public override uint Id { get; set; } = 35;
        public override string Name { get; set; } = "<color=#FFFF7C>Tarjeta de científico ingeniero de contención</color>";
        public override string Description { get; set; } = "Tarjeta que permite abrir puertas con el nivel de autorización de un científico ingeniero de contención.";
        public override float Weight { get; set; } = 1;
        public override SpawnProperties SpawnProperties { get; set; }
        public override KeycardPermissions Permissions { get; set; } = KeycardPermissions.Checkpoints |
   KeycardPermissions.ContainmentLevelOne |
   KeycardPermissions.ContainmentLevelTwo |
   KeycardPermissions.ContainmentLevelThree |
   KeycardPermissions.AlphaWarhead |
   KeycardPermissions.Intercom;
        public override string KeycardLabel { get; set; } = "Containment Engineer Keycard";
        public override Color32? TintColor { get; set; } = Color.gray;
        public override Color32? KeycardLabelColor { get; set; } = Color.green;
        public override Color32? KeycardPermissionsColor { get; set; } = Color.black;
    }
}*/