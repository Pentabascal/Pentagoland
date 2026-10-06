# Configuración rápida del plugin SCPHispanoUtilities

Este archivo está pensado para que el desarrollador adapte el plugin sin tocar la lógica principal.

## 0. Identidad del servidor

```csharp
public string ServerName { get; set; } = "Pentagoland";
public string ServerTagline { get; set; } = "Servidor hispano de SCP: Secret Laboratory";
public string CommandPrefix { get; set; } = "!";
```

## 1. Activar o desactivar funciones

```csharp
public bool WelcomeEnabled { get; set; } = true;
public bool EnableSpanishRulesMessage { get; set; } = true;
public bool DeathAnnouncementsEnabled { get; set; } = true;
public bool RoundAnnouncementsEnabled { get; set; } = true;
public bool EnableExternalLinkFilter { get; set; } = true;
public bool EnableSpamProtection { get; set; } = true;
```

## 2. Mensajes personalizados

```csharp
public string WelcomeMessage { get; set; } = "Bienvenido a nuestro servidor hispano...";
public string RulesMessage { get; set; } = "Reglas: respeta a todos...";
public string RoundStartMessage { get; set; } = "Servidor hispano activo...";
public string RoundEndMessage { get; set; } = "La ronda ha terminado...";
```

## 3. Duraciones de mensajes

```csharp
public float WelcomeDuration { get; set; } = 8f;
public float RulesBroadcastDuration { get; set; } = 10f;
public float RoundStartBroadcastDuration { get; set; } = 8f;
public float RoundEndBroadcastDuration { get; set; } = 8f;
```

## 4. Moderación y reportes

```csharp
public int ReportCooldownSeconds { get; set; } = 30;
public int ReportStaffMessageDuration { get; set; } = 8;
public int PlayerInfoDuration { get; set; } = 6;
```

## 5. Puntos del servidor

```csharp
public int DefaultRewardPoints { get; set; } = 10;
public int KillRewardPoints { get; set; } = 10;
public int RoundWinRewardPoints { get; set; } = 25;
public int RoundLoseRewardPoints { get; set; } = 5;
```

## 6. Recomendaciones

- Mantén el filtro de enlaces activado en servidores públicos.
- Ajusta ReportCooldownSeconds para evitar spam de reportes.
- Cambia los mensajes para que la comunidad sienta identidad hispana.
- Si usas custom plugins, mantén los nombres de comandos únicos para evitar conflicto.

## 7. Comandos incluidos

- !help / !ayuda
- !info
- !report <jugador> <motivo>
- !puntos
- !rules / !reglas

## 8. Compatibilidad

Este plugin está estructurado para ser compatible con otros plugins EXILED siempre que:

- no reutilices los mismos nombres de comandos
- no sobreescribas eventos de otros plugins sin necesidad
- uses nombres de clases y archivos únicos
- mantengas el flujo de eventos ligero y sin bloqueos largos
