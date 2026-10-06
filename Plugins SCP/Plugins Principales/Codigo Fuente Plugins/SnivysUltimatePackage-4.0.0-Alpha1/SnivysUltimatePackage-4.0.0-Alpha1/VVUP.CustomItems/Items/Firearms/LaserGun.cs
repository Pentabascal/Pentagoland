using AdminToys;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features.Toys;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.Firearms.Extensions;
using MEC;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UserSettings.ServerSpecific;
using VVUP.Base;
using ApplyEffects = VVUP.CustomItems.ApplyEffects;
using VVUP.Base.API;
using RandomNumber = VVUP.Base.API.GetRandomNumber;

namespace VVUP.CustomItems.Items.Firearms
{
    [CustomItem(ItemType.GunE11SR)]
    public class LaserGun : VVUP.CustomItems.API.ValidatedCustomWeapon, ICustomItemGlow
    {
        public override ItemType Type { get; set; } = ItemType.GunE11SR;
        public override uint Id { get; set; } = 41;
        public override string Name { get; set; } = "<color=#FF0000>Haz Helios X-57</color>";
        public override string Description { get; set; } = "¡Dispara rayos láser!";
        public override float Weight { get; set; } = 2;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 1,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 20,
                    Location = SpawnLocationType.InsideHidChamber,
                },
                new ()
                {
                    Chance = 20,
                    Location = SpawnLocationType.Inside079Armory,
                },
            },
        };

        [Description("Tiempo que permanece cada láser en pantalla.")]
        public float LaserVisibleTime { get; set; } = 0.24f;


        [Description("Brillo del láser.")]
        public float LaserBrightness { get; set; } = 16f;

        [Description("Legacy setting retained for existing configs; one laser is now rendered for every shot.")]
        public int LaserVisualEveryNthShot { get; set; } = 1;

        [Description("Ancho y grosor del haz; el tercer componente se conserva por compatibilidad.")]
        public Vector3 LaserScale { get; set; } = new Vector3(0.1f, 0.1f, 0.03f);

        [Description("Componente roja del láser. El valor debe estar entre 0 y 1.")]
        public List<float> LaserColorRed { get; set; } = new List<float>()
        {
            0.86f,
            1,
            0,
            0.55f,
            0.97f,
        };
        [Description("Componente verde del láser. El valor debe estar entre 0 y 1.")]

        public List<float> LaserColorGreen { get; set; } = new List<float>()
        {
            0.08f,
            0.27f,
            0.84f,
            0.65f,
            0.5f,
            0.36f,
            0,
            0.97f,
        };

        [Description("Componente azul del láser. El valor debe estar entre 0 y 1.")]
        public List<float> LaserColorBlue { get; set; } = new List<float>()
        {
            0.24f,
            0,
            0.31f,
            1,
            0.96f,
        };

        [Description("Legacy spiral settings retained for existing configs; the laser is now a single beam.")]
        public float SpiralRadius { get; set; } = 0.15f;

        public float SpiralTurns { get; set; } = 3f;

        public float SpiralParticleSize { get; set; } = 0.15f;

        public float SpiralDensity { get; set; } = 8f;

        [Description("Si contiene efectos, los aplicará al acertar.")]
        public List<ApplyEffects> EffectsToApply { get; set; }= new List<ApplyEffects>
        {
            new()
            {
                EffectType = EffectType.Burned,
                Intensity = 1,
                Duration = 3,
            }
        };
        
        public bool HasCustomItemGlow { get; set; } = true;
        public Color CustomItemGlowColor { get; set; } = new Color32(255, 0, 0, 127);
        public float GlowRange { get; set; } = 0.35f;
        public float GlowIntensity { get; set; } = 0.3f;
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        public Vector3 GlowOffset { get; set; } = Vector3.zero;

        private const float MinimumLaserThickness = 0.08f;
        private const float MinimumLaserVisibleTime = 0.15f;
        private const float BrightnessIncrease = 1.3f;

        protected override void OnShot(ShotEventArgs ev)
        {
            Vector3 origin;
            if (BarrelTipExtension.TryFindWorldmodelBarrelTip(ev.Player.CurrentItem.Serial, out BarrelTipExtension ext))
                origin = ext.WorldspacePosition;
            else
                origin = ev.Player.CameraTransform.position;

            Vector3 shotVector = ev.Position - origin;
            float distance = shotVector.magnitude;
            if (distance <= 0f || float.IsNaN(distance) || float.IsInfinity(distance))
                return;

            Vector3 direction = shotVector / distance;

            Color color = GetLaserColor(ev.Player);

            Vector3 laserSize = new Vector3(
                Mathf.Max(MinimumLaserThickness, Mathf.Abs(LaserScale.x)),
                Mathf.Max(MinimumLaserThickness, Mathf.Abs(LaserScale.y)),
                distance);
            Vector3 laserPosition = origin + direction * (distance * 0.5f);
            Quaternion quaternion = Quaternion.LookRotation(direction);

            // One OnShot callback represents one bullet; create exactly one beam for it.
            Primitive centerBeam = Primitive.Create(
                PrimitiveType.Cube,
                (PrimitiveFlags)2,
                new Vector3?(laserPosition),
                new Vector3?(quaternion.eulerAngles),
                new Vector3?(laserSize),
                true,
                new Color?(color)
            );

            Timing.RunCoroutine(LaserFade(centerBeam));

        }
        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (!IsMatchingWeapon(ev))
                return;

            if (!EffectsToApply.IsEmpty())
            {
                foreach (var effect in EffectsToApply)
                {
                    Log.Debug(
                        $"VVUP Custom Items: Laser Gun, applying {effect.EffectType} with intensity {effect.Intensity} and duration {effect.Duration} to {ev.Player.Nickname}");
                    ev.Player.EnableEffect(effect.EffectType, effect.Intensity, effect.Duration);
                }
            }

            base.OnHurting(ev);
        }

        private Color GetLaserColor(Player player)
        {
            Color color = new Color();
            Log.Debug($"VVUP Custom Items: Getting laser color for {player.Nickname} from SSSS settings if they have one defined");
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunRedId, out var redSetting) 
                && int.TryParse(redSetting.SyncInputText, out int r) && r is > -1 and < 256)
            {
                color.r = r / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS red value: {r}");
            }
            else
            {
                color.r = LaserColorRed[RandomNumber.GetRandomInt(LaserColorRed.Count)];
                Log.Debug($"VVUP Custom Items: Using random red value: {color.r}");
            }
            
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunGreenId, out var greenSetting)
                && int.TryParse(greenSetting.SyncInputText, out int g) && g is > -1 and < 256)
            {
                color.g = g / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS green value: {g}");
            }
            else
            {
                color.g = LaserColorGreen[RandomNumber.GetRandomInt(LaserColorGreen.Count)];
                Log.Debug($"VVUP Custom Items: Using random green value: {color.g}");
            }
            
            if (ServerSpecificSettingsSync.TryGetSettingOfUser<SSPlaintextSetting>(
                    player.ReferenceHub, Plugin.Instance.Config.SsssConfig.LaserGunBlueId, out var blueSetting) 
                && int.TryParse(blueSetting.SyncInputText, out int b) && b is > -1 and < 256)
            {
                color.b = b / 255f;
                Log.Debug($"VVUP Custom Items: Using SSSS blue value: {b}");
            }
            else
            {
                color.b = LaserColorBlue[RandomNumber.GetRandomInt(LaserColorBlue.Count)];
                Log.Debug($"VVUP Custom Items: Using random blue value: {color.b}");
            }
            
            Color.RGBToHSV(color, out float h, out float s, out float v);
            Color finalColor = Color.HSVToRGB(h, 1f, 1f);

            Log.Debug($"VVUP Custom Items: Final RGB values for {player.Nickname}: R:{finalColor.r}, G:{finalColor.g}, B:{finalColor.b}");

            return finalColor;
        }
        IEnumerator<float> LaserFade(Primitive Laser)
        {
            Color color = new Color(Laser.Color.r, Laser.Color.g, Laser.Color.b, 1f);
            float visibleTime = Mathf.Max(MinimumLaserVisibleTime, LaserVisibleTime);
            float fadeStepDuration = visibleTime / 20f;

            for (int i = 20; i > 0; i--)
            {
                float brightness = (LaserBrightness * BrightnessIncrease * (i / 20f));
                Color newColor = new Color(color.r * brightness, color.g * brightness, color.b * brightness, 1f);

                Laser.Color = newColor;

                yield return Timing.WaitForSeconds(fadeStepDuration);
            }

            Laser.Destroy();
        }
    }
}