using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.CustomRoles.API.Features;
using MEC;

namespace VVUP.CustomRoles.Abilities.Active
{
    [CustomAbility]
    public class HealingMist : ActiveAbility
    {
        private readonly List<CoroutineHandle> coroutines = new ();

        public override string Name { get; set; } = "Niebla curativa";

        public override string Description { get; set; } =
            "Emite una breve nube de sustancias químicas que cura y protege a tus aliados durante unos instantes.";

        public override float Duration { get; set; } = 15f;

        public override float Cooldown { get; set; } = 180f;

        [Description("Salud restaurada por cada segundo que la habilidad está activa.")]
        public float HealAmount { get; set; } = 6;

        [Description("Cantidad de vida artificial otorgada al terminar la habilidad.")]
        public ushort ProtectionAmount { get; set; } = 45;

        protected override void AbilityUsed(Player player)
        {
            ActivateMist(player);
        }

        protected override void UnsubscribeEvents()
        {
            foreach (CoroutineHandle handle in coroutines)
                Timing.KillCoroutines(handle);
            base.UnsubscribeEvents();
        }

        private void ActivateMist(Player ply)
        {
            foreach (Player player in Player.List)
            {
                if (player.Role.Side == ply.Role.Side && player != ply)
                    coroutines.Add(Timing.RunCoroutine(DoMist(ply, player)));
            }
        }

        private IEnumerator<float> DoMist(Player activator, Player player)
        {
            for (int i = 0; i < Duration; i++)
            {
                if (player.Health + HealAmount >= player.MaxHealth ||
                    (player.Position - activator.Position).sqrMagnitude > 144f)
                    continue;

                player.Health += HealAmount;

                yield return Timing.WaitForSeconds(0.75f);
            }

            if ((activator.Position - player.Position).sqrMagnitude < 144f)
                player.ArtificialHealth += ProtectionAmount;
        }
    }
}