using System.Collections.Generic;
using System.ComponentModel;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Items;
using Exiled.CustomItems.API.Features;
using Exiled.CustomRoles.API.Features;
using Exiled.Events.EventArgs.Player;
using JetBrains.Annotations;
using MEC;

namespace VVUP.CustomRoles.Abilities.Passive
{
    [CustomAbility]
    public class Addicted : PassiveAbility
    {
        public override string Name { get; set; } = "Adicción";
        public override string Description { get; set; } = "Provoca adicción a las pastillas y la adrenalina. Si tardas demasiado en volver a consumirlas, sufrirás efectos adversos.";
        [Description("Mensaje que se mostrará al jugador cuando sufra síntomas de abstinencia.")]
        public string MessageOnWithdrawal { get; set; } = "¡Sientes cómo unos terribles síntomas de abstinencia recorren tu cuerpo!";

        [Description("Mensaje que se mostrará al jugador cuando consuma un objeto que mitigue la adicción.")]
        public string MessageOnUse { get; set; } = "Por ahora, te encuentras mejor.";

        public float MessageDuration { get; set; } = 5f;
        public bool UseHints { get; set; } = true;
        
        [Description("Tiempo, en segundos, que tarda en comenzar el síndrome de abstinencia.")]
        public float TimeBeforeWithdrawals { get; set; } = 90f;
        [Description("Intervalo, en segundos, entre los efectos del síndrome de abstinencia.")]
        public float WithdrawalInterval { get; set; } = 15f;
        [Description("Daño que recibe el jugador en cada intervalo de abstinencia.")]
        public float WithdrawalAmount { get; set; } = 5f;

        [Description(
            "¿Debe detenerse la abstinencia al usar un objeto válido (cuando el jugador hace clic izquierdo) o al terminar de usarlo (después de la animación)?")]
        public bool SedateOnUsingItemComplete { get; set; } = true;
        
        [Description("Objetos normales que reducirán los efectos de la adicción.")]
        [CanBeNull]
        public List<ItemType> AddictedItems { get; set; } = new List<ItemType>
        {
            ItemType.Adrenaline,
            ItemType.Painkillers,
        };

        [Description("ID de los objetos personalizados que reducirán los efectos de la adicción.")]
        [CanBeNull]
        public List<uint> CustomAddictedItems { get; set; } = new List<uint>
        {
            23,
            34,
        };
        
        private Dictionary<Player, CoroutineHandle> addictedPlayers = new Dictionary<Player, CoroutineHandle>();
        protected override void AbilityAdded(Player player)
        {
            if (SedateOnUsingItemComplete)
                Exiled.Events.Handlers.Player.UsingItemCompleted += OnUsingItemCompleted;
            else
                Exiled.Events.Handlers.Player.UsingItem += OnUsingItem;
            
            CoroutineHandle handle = Timing.RunCoroutine(WithdrawalCoroutine(player));
            addictedPlayers.Add(player, handle);
            base.AbilityAdded(player);
        }
        
        protected override void AbilityRemoved(Player player)
        {
            if (SedateOnUsingItemComplete)
                Exiled.Events.Handlers.Player.UsingItemCompleted -= OnUsingItemCompleted;
            else
                Exiled.Events.Handlers.Player.UsingItem -= OnUsingItem;
            if (addictedPlayers.ContainsKey(player))
            {
                Timing.KillCoroutines(addictedPlayers[player]);
                addictedPlayers.Remove(player);
            }
            base.AbilityRemoved(player);
        }

        private void OnUsingItemCompleted(UsingItemCompletedEventArgs ev)
        {
            if (Check(ev.Player) && ShouldSedateWithdrawal(ev.Item))
            {
                ResetWithdrawal(ev.Player);
            }
        }

        private void OnUsingItem(UsingItemEventArgs ev)
        {
            if (Check(ev.Player) && ShouldSedateWithdrawal(ev.Item))
            {
                ResetWithdrawal(ev.Player);
            }
        }
        
        private IEnumerator<float> WithdrawalCoroutine(Player player)
        {
            Log.Debug($"VVUP Custom Abilities: Addicted, Starting withdrawal coroutine for {player.Nickname}, waiting {TimeBeforeWithdrawals} seconds");
            yield return Timing.WaitForSeconds(TimeBeforeWithdrawals);
            
            while (player.IsAlive)
            {
                player.Hurt(WithdrawalAmount);
                if (!string.IsNullOrWhiteSpace(MessageOnWithdrawal))
                    if (UseHints)
                        player.ShowHint(MessageOnWithdrawal, MessageDuration);
                    else
                        player.Broadcast((ushort)MessageDuration, MessageOnWithdrawal);
                
                yield return Timing.WaitForSeconds(WithdrawalInterval);
            }
        }
        
        private bool ShouldSedateWithdrawal(Item item)
        {
            return (AddictedItems != null && AddictedItems.Contains(item.Type)) || 
                   (CustomAddictedItems != null && CustomItem.TryGet(item, out CustomItem customItem) && 
                    CustomAddictedItems.Contains(customItem.Id));
        }

        private void ResetWithdrawal(Player player)
        {
            if (addictedPlayers.ContainsKey(player))
            {
                Log.Debug($"VVUP Custom Abilities: Addicted, {player.Nickname} used an addicted item, resetting withdrawal timer.");
                Timing.KillCoroutines(addictedPlayers[player]);
                CoroutineHandle newHandle = Timing.RunCoroutine(WithdrawalCoroutine(player));
                addictedPlayers[player] = newHandle;
        
                if (!string.IsNullOrWhiteSpace(MessageOnUse))
                    if (UseHints)
                        player.ShowHint(MessageOnUse, MessageDuration);
                    else
                        player.Broadcast((ushort)MessageDuration, MessageOnUse);
            }
        }
    }
}