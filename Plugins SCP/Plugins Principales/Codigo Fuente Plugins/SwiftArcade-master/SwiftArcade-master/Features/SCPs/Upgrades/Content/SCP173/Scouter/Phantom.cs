using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerRoles.Spectating;
using SwiftArcadeMode.Utils.Interfaces;
using UnityEngine;

namespace SwiftArcadeMode.Features.SCPs.Upgrades.Content.SCP173.Scouter
{
    public class Phantom(UpgradePathPerkBase parent) : UpgradeBase<Scouter>(parent)
    {
        public override string Name => "Espectro";

        public override string Description => "Aparece un SCP-173 fantasma cuando empiezas a correr a toda velocidad.";

        ReferenceHub phantom;

        public override void Init()
        {
            base.Init();
            Scp173Events.BreakneckSpeedChanged += OnBreakneckSpeedChanged;
            PlayerEvents.Hurt += OnHurt;
            PlayerEvents.ChangedRole += OnChangedRole;
        }

        public override void Remove()
        {
            base.Remove();
            Scp173Events.BreakneckSpeedChanged -= OnBreakneckSpeedChanged;
            PlayerEvents.Hurt -= OnHurt;
            PlayerEvents.ChangedRole -= OnChangedRole;

            DeletePhantom(true);
        }

        private void OnHurt(LabApi.Events.Arguments.PlayerEvents.PlayerHurtEventArgs ev)
        {
            if (ev.Player.ReferenceHub != phantom)
                return;

            DeletePhantom(false, ev.Attacker);
        }

        private void OnChangedRole(LabApi.Events.Arguments.PlayerEvents.PlayerChangedRoleEventArgs ev)
        {
            if (ev.Player != Player || ev.OldRole == ev.NewRole.RoleTypeId)
                return;

            DeletePhantom(true);
        }

        private void OnBreakneckSpeedChanged(LabApi.Events.Arguments.Scp173Events.Scp173BreakneckSpeedChangedEventArgs ev)
        {
            if (ev.Player != Player || !ev.Active)
                return;

            Vector3 pos = Player.Position;

            if (phantom == null)
            {
                phantom = DummyUtils.SpawnDummy(Player.DisplayName + " (Phantom)");
                phantom.serverRoles.NetworkHideFromPlayerList = true;
                SpectatableVisibilityManager.SetHidden(phantom, true);
            }

            Timing.CallDelayed(0.1f, () =>
            {
                phantom.roleManager.ServerSetRole(RoleTypeId.Scp173, RoleChangeReason.RemoteAdmin);
                phantom.TryOverridePosition(pos);
            });
        }

        public void DeletePhantom(bool destroy = false, Player attacker = null)
        {
            if (phantom == null)
                return;

            if (phantom.roleManager.CurrentRole.RoleTypeId == RoleTypeId.Scp173)
            {
                SendMessage("¡Fantasma destruido!" + (attacker == null ? "" : " Atacante: " + attacker.DisplayName));
                TimedGrenadeProjectile.SpawnActive(phantom.GetPosition(), ItemType.GrenadeFlash, Player, 0.1f);

                if (Room.TryGetRoomAtPosition(phantom.GetPosition(), out Room room))
                    room.LightController.FlickerLights(5f);

                phantom.roleManager.ServerSetRole(RoleTypeId.Filmmaker, RoleChangeReason.RemoteAdmin);
            }

            if (destroy)
                Timing.CallDelayed(0.1f, () => NetworkServer.Destroy(phantom.gameObject));
        }
    }
}
