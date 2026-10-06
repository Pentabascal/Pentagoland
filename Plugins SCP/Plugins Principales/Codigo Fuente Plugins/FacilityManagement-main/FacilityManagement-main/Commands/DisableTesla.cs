using CommandSystem;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesla = Exiled.API.Features.TeslaGate;

namespace FacilityManagement.Commands
{
    [CommandHandler(typeof(GameConsoleCommandHandler))]
    public class DisableTesla : ICommand, IUsageProvider
    {
        public string Command { get; } = "disabletesla";
        public string[] Aliases { get; } = new string[] { "dt" };

        public string Description { get; } = "Desactiva todas las puertas Tesla durante el tiempo indicado. Usa -1 para desactivarlas indefinidamente y 0 para volver a activarlas.";
        public string[] Usage { get; } = new string[]
        {
            "Duración",
        };

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if(!sender.CheckPermission(PlayerPermissions.FacilityManagement, out response))
            {
                return false;
            }
            if (!float.TryParse(arguments.ElementAtOrDefault(0), out float time))
            {
                response = $"Duración no válida: {arguments.ElementAtOrDefault(0)}";
                return false;
            }
            if (time < 0)
                time = float.MaxValue;
            foreach (Tesla tesla in Tesla.List)
            {
                tesla.InactiveTime = time;
            }
            return true;
        }
    }
}
