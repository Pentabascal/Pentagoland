namespace VVUP.CustomItems.Configs
{
    public class SsssConfig
    {
        public bool SsssEnabled { get; set; } = true;
        public bool UseHints { get; set; } = true;
        public float TextDisplayDuration { get; set; } = 5f;
        public string Header { get; set; } = "Vicious Vikki's Custom Items.";
        public int DetonateC4Id { get; set; } = 10001;
        public int CustomItemTextId { get; set; } = 1;
        public string DetonateHint { get; set; } = "Pulsa la tecla asignada para detonar tus cargas C4 o F4.";
        public string DetonateSsssText { get; set; } = "Detonar C4/F4";
        public string SsssNoDeployed { get; set; } = "No has colocado ninguna carga C4/F4";
        public string SsssDetonatorNeeded { get; set; } = "Debes tener equipado el detonador";
        public string SsssTooFarAway { get; set; } = "Estás demasiado lejos de tus cargas C4/F4; acércate.";
        public string SsssDetonateActivationMessage { get; set; } = "Detonando C4/F4";
        public int GrenadeLauncherForceModeId { get; set; } = 10002;
        public string GrenadeLauncherForceModeSsssText { get; set; } = "Cambiar fuerza del ADATS";
        public string GrenadeLauncherForceModeHint { get; set; } = "Alterna entre fuerza completa y media del ADATS";
        public int GrenadeLauncherLaunchModeId { get; set; } = 10003;
        public string GrenadeLauncherLaunchModeSsssText { get; set; } = "Cambiar modo de lanzamiento del ADATS";
        public string GrenadeLauncherLaunchModeHint { get; set; } = "Alterna entre los modos de impacto y rodadura del ADATS";
        public int LaserGunRedId { get; set; } = 10004;
        public int LaserGunGreenId { get; set; } = 10005;
        public int LaserGunBlueId { get; set; } = 10006;
        public string LaserGunColorText { get; set; } = "Color del haz Helios";
        public string LaserGunColorTextRed { get; set; } = "(Rojo)";
        public string LaserGunColorTextGreen { get; set; } = "(Verde)";
        public string LaserGunColorTextBlue { get; set; } = "(Azul)";
        public string LaserGunColorHint { get; set; } = "Introduce un valor entre 0 y 255. Los valores negativos o superiores a 255 se elegirán al azar.";
    }
}