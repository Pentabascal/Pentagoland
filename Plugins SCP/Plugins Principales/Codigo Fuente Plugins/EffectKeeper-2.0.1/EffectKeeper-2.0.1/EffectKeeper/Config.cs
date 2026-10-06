using System.ComponentModel;

namespace EffectKeeper;

#if EXILED
    using Exiled.API.Interfaces;
    using Exiled.API.Enums;
#else
using CustomPlayerEffects;
#endif
    
#if EXILED
    public class Config : IConfig
#else
public class Config
#endif
{
#if EXILED
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; }
#endif

    public bool DeathCancels { get; set; } = true;

#if EXILED
        public List<EffectType> AllowedEffects { get; set; } = new()
        {
            EffectType.Scp207,
        };

        public List<EffectCategory> AllowedCategories { get; set; } = new()
        {
            EffectCategory.Movement,
            EffectCategory.Positive,
        };

        [Description("Se considera que estos efectos pueden fallar al cambiar de rol; no se recomienda modificarlos.")]
        public List<EffectType> ReapplyEffects { get; set; } = new()
        {
            EffectType.AntiScp207,
            EffectType.Ghostly,
            EffectType.Scp1344,
        };
#else
    public List<string> AllowedEffects { get; set; } = new()
    {
        "Scp207",
    };
        
    public List<StatusEffectBase.EffectClassification> AllowedCategories { get; set; } = new()
    {
        StatusEffectBase.EffectClassification.Positive,
    };

    [Description("Se considera que estos efectos pueden fallar al cambiar de rol; no se recomienda modificarlos.")]
    public List<string> ReapplyEffects { get; set; } = new()
    {
        nameof(Scp1344),
        nameof(AntiScp207),
        nameof(Ghostly),
    };
#endif
}