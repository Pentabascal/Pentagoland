using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Interfaces;
using UnityEngine;
using VVUP.Base.API;
using VVUP.Base.EventHandlers;

namespace VVUP.Base
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        [Description("Este módulo apenas contiene mensajes de depuración; sirve principalmente como base para los demás.")]
        public bool Debug { get; set; } = false;
        
        [Description("Activa los efectos de brillo compatibles para objetos personalizados que no implementan ICustomItemGlow.")]
        public bool EnableCompatibilityGlow { get; set; } = false;
        
        [Description("Permite definir colores de brillo para objetos personalizados mediante su ID si el complemento de origen no ofrece esta opción. Los objetos de VVUP.CI usan un sistema propio y se ignoran aquí.")]
        public List<CustomItemGlowConfig> CustomItemGlow { get; set; } = new List<CustomItemGlowConfig>
        {
            new CustomItemGlowConfig
            {
                CustomItemId = 0,
                R = 1,
                G = 0, 
                B = 0,
                A = 0,
                GlowRange = 0.25f,
                Intensity = 1f,
            },
        };
        [Description("A veces se producen errores al aplicar el brillo a objetos que aparecen en el suelo al comenzar la ronda. Este retraso debería ayudar a evitarlos.")]
        public float DelayBeforeColorsAppliesToItemsAlreadyOnGroundAtRoundStart { get; set; } = 1f;
    }

    public class CustomItemGlowConfig
    {
        public uint CustomItemId { get; set; }
        [Description("Los valores RGBA deben estar entre 0 y 1.")]
        public float R { get; set; } = 1;
        public float G { get; set; } = 1;
        public float B { get; set; } = 1;
        public float A { get; set; } = 1;
        [Description("El alcance del brillo puede tener cualquier valor, aunque normalmente se recomienda un valor entre 0 y 1.")]
        public float GlowRange { get; set; } = 0.25f;
        [Description("La intensidad debe estar entre 0 y 1.")]
        public float Intensity { get; set; } = 0.25f;
        [Description("Opciones del tipo de sombra: None (ninguna), Soft (suave) y Hard (marcada).")]
        public ICustomItemGlow.GlowShadowType ShadowType { get; set; } = ICustomItemGlow.GlowShadowType.None;
        [Description("Los desplazamientos permiten ajustar la posición del brillo. Son útiles en objetos cuyo centro visual no coincide con su centro, como el E-11.")]
        public float OffsetX { get; set; } = 0f;
        public float OffsetY { get; set; } = 0f;
        public float OffsetZ { get; set; } = 0f;
    
        public Color GetColor() => new Color(
            Mathf.Clamp(R, 0f, 1f), 
            Mathf.Clamp(G, 0f, 1f), 
            Mathf.Clamp(B, 0f, 1f), 
            Mathf.Clamp(A, 0f, 1f));
    
        public Vector3 GetOffset() => new Vector3(OffsetX, OffsetY, OffsetZ);
    }
}