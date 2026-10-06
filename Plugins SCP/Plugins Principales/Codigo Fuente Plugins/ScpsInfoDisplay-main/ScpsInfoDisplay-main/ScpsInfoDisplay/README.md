# ScpsInfoDisplay (EXILED) ![Downloads](https://img.shields.io/github/downloads/bladuk/ScpsInfoDisplay/total.svg)
When SCP spawns, displays information about players who are in the SCP team. Possible integration with custom roles.

## How to install plugin?
Put ScpsInfoDisplay.dll under the release tab into %appdata%\EXILED\Plugins (Windows) or .config/EXILED/Plugins (Linux) folder.

## Default configs
```yaml
scpsinfodisplay:
# Is the plugin enabled?
  is_enabled: true
  # Display strings. Format: Role, display string.
  display_strings:
    Scp049: '<color=%roleColor%>SCP-049</color> | Vida: %health% | Hume Shield: %humeshield% | Zombis: %zombies%'
    Scp079: '<color=%roleColor%>SCP-079</color> | Vida: %health% | Hume Shield: %humeshield% | Generadores: %generators%%engaging%/3 | Nivel: %079level% | Exp: %079experience% | Energía: %079energy%'
    Scp096: '<color=%roleColor%>SCP-096</color> | Vida: %health% | Hume Shield: %humeshield% | Estado: %096state% | Objetivos: %096targets%'
    Scp106: '<color=%roleColor%>SCP-106</color> | Vida: %health% | Hume Shield: %humeshield% | Vigor: %106vigor%%'
    Scp173: '<color=%roleColor%>SCP-173</color> | Vida: %health% | Hume Shield: %humeshield% | Observación: %173stared%'
    Scp939: '<color=%roleColor%>SCP-939</color> | Vida: %health% | Hume Shield: %humeshield%'
    Scp3114: '<color=%roleColor%>SCP-3114</color> | Vida: %health% | Hume Shield: %humeshield% | Disfraz: %3114disguisetype%'
  # Custom roles integrations. Format: SessionVariable that marks that the player belongs to that role, display string.
  custom_roles_integrations: {}
  scp096_state_indicator:
    Docile: En calma
    Distressed: Enfureciéndose
    Enraged: Enfurecido
    Calming: Calmándose
  scp173_observation_indicators:
    Observed: 'ⓞ'
    Unobserved: '-'
  scp3114_disguise_display:
    ClassD: Clase-D
    Scientist: Científico
    FacilityGuard: Guardia de las instalaciones
    NtfCaptain: Capitán MTF
    NtfSergeant: Sargento MTF
    NtfSpecialist: Especialista MTF
    NtfPrivate: Soldado MTF
    ChaosConscript: Recluta CI
    ChaosMarauder: Asaltante CI
    ChaosRepressor: Represor CI
    ChaosRifleman: Fusilero CI
    Tutorial: Tutorial
  # Display string alignment. Possible values: left, center, right.
  text_alignment: right
  # Display text size (default 19).
  text_size: 19
  # Horizontal text offset, in RueI space units.
  text_horizontal_offset: 900
  # Vertical RueI position (0 bottom to 1000 top).
  text_vertical_position: 875
  # The player seeing the list will be highlighted with the special marker to the left. Leave it empty if disabled.
  players_marker: <color=%rolecolor%>You --></color>
  # Display debug messages in server console?
  debug: false
```

`%humeshield%` shows the current Hume Shield, rounded up to match the value synchronized by the game. `%arhealth%` remains available for artificial health points.

SCP information is sent as a RueI element instead of a native hint, so it no
longer replaces UltimateHUD or other RueI content. Its alignment follows the
screen resolution. Every SCP-team player,
including SCP-049-2, can see the SCP information panel while alive. Each SCP's
details are kept on one row; increase horizontal space or reduce `text_size` if
custom text is too wide. The active-round refresh continues for the lifetime of
the plugin and isolates per-player refresh failures, so a transient error does
not stop the display for every SCP. Its display position is fixed and does not
use the previous large `<voffset>` adjustment.
