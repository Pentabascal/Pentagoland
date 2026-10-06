namespace VVUP.ServerEvents.ServerEventsConfigs
{
    public class PeanutInfectionConfig
    {
        public string StartEventCassieMessage { get; set; } = "SCP 1 7 3 plague detected";
        public string StartEventCassieText { get; set; } = "Plaga de SCP-173 detectada (inicio del evento de infección de SCP-173).";
        public string EndEventCassieMessage { get; set; } = "SCP 1 7 3 plague contained";
        public string EndEventCassieText { get; set; } = "¿Plaga de SCP-173 contenida? (fin del evento de infección de SCP-173).";
    }
}