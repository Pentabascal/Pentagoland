using Exiled.API.Features.DamageHandlers;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Player;

namespace VVUP.CustomItems.API
{
    public abstract class ValidatedCustomWeapon : CustomWeapon
    {
        protected bool IsMatchingWeapon(HurtingEventArgs ev)
        {
            return ev.Attacker != null &&
                   ev.DamageHandler?.CustomBase is FirearmDamageHandler firearmDamage &&
                   firearmDamage.Item.Type == Type &&
                   Check(firearmDamage.Item);
        }

        protected override void OnHurting(HurtingEventArgs ev)
        {
            if (!IsMatchingWeapon(ev))
                return;

            base.OnHurting(ev);
        }
    }
}
