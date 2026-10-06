namespace Axwabo.CommandSystem.Example.Translations;

public sealed class GreetingTranslations
{

    [GreetingTranslation(GreetingType.Hi)]
    public string Hi { get; set; } = "¡Hola!";

    [GreetingTranslation(GreetingType.HelloName)]
    public string Hello { get; set; } = "¡Hola, {0}!"; // string formatting is supported

    [GreetingTranslation(GreetingType.Welcome)]
    public string Welcome { get; set; } = "¡Te damos la bienvenida al servidor!";

    [GreetingTranslation(GreetingType.Gday)]
    public string Gday { get; set; } = "¡Buenas, colega!";

    [GreetingTranslation(GreetingType.LookingFancy)]
    public string LookingFancy { get; set; } = "¡Hoy tienes muy buena pinta, {0}!";

}
