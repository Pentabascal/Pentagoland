![UltimateHUD](https://github.com/user-attachments/assets/624b8a88-7269-452a-a507-f87ca6363179)<br><br><br>
[![downloads](https://img.shields.io/github/downloads/Vretu-Dev/UltimateHUD/total?style=for-the-badge&logo=icloud&color=%233A6D8C)](https://github.com/Vretu-Dev/UltimateHUD/releases/latest)
![Latest](https://img.shields.io/github/v/release/Vretu-Dev/UltimateHUD?style=for-the-badge&label=Latest%20Release&color=%23D91656)

## Downloads:
| Framework | Version    |  Release                                                              |
|:---------:|:----------:|:----------------------------------------------------------------------:|
| Exiled    | ≥ 9.11.X    | [⬇️](https://github.com/Vretu-Dev/UltimateHUD/releases)        |
| LabAPI    | ≥ 1.1.4      | [⬇️](https://github.com/Vretu-Dev/UltimateHUD/releases) |

## Requirements:
- #### RueI [V3.1.X](https://github.com/pawslee/RueI/releases)
- #### 0Harmony.dll 2.2.2
> [!IMPORTANT]
> Place the RueI.dll file in the LabAPI plugin folder. <br>
Place the 0Harmony.dll file in the LabAPI dependencies folder.

> [!CAUTION]
> Plugins using RueI V2 will not work.
## Features:
- Clock
- TPS
- RoundTime
- Player Info: Nick, role, kills and assigned subclass
- Spectator Info: Observed player's nick, role, kills and assigned subclass
- Players count
- Spectators count
- Engage generators count
- Warhead status

## Credits:
- Thanks [@NamelessSCP](https://github.com/NamelessSCP) for using the [SpectatorList](https://github.com/NamelessSCP/SpectatorList-SL) idea.<br>


## Config:
<details>
<summary><b>7777.yml</b></summary><br>

```yaml
is_enabled: true
debug: false
# Clock Settings:
enable_clock: true
clock: '<align=left><space=37><color={color}><size=18><b>Hora:</b> {time}</size></color></align>'
# UTC Time Zone | 2 = UTC+2
time_zone: 2
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
clock_visual: 'BOTH'
clock_y_cordinate: 980
# TPS Settings:
enable_tps: true
tps: '<align=right><color={color}><size=18><b>TPS:</b> {tps}/{maxTps}</size></color></align>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
tps_visual: 'BOTH'
tps_y_cordinate: 980
# ROUND TIME Settings:
enable_round_time: true
round_time: '<align=left><space=5><color={color}><size=18><b>Duración de la ronda:</b> {round_time}</size></color></align>'
# GAMEPLAY = only for players | SPECTATOR = only for spectators | BOTH = spectator & gameplay
round_time_visual: 'BOTH'
round_time_y_cordinate: 960
# Player HUD Settings:
enable_player_hud: true
# You can use {displayname} instead of {nickname}
player_hud: '<size=22><color=#D6FFFB><b>Nombre:</b></color> <color=#FFFFFF>{nickname}</color> <b>|</b> <color=#D6FFFB><b>Rol:</b></color> {role} <b>|</b> <color=#D6FFFB><b>Kills:</b></color> <color=yellow>{kills}</color>{subclass}</size>'
player_hud_y_cordinate: 25
# Spectator List:
enable_spectator_list: true
spectator_list_header: "<align=right><size=28><color={color}>\U0001F465 Spectators ({count})</color></size></align>"
spectator_list_players: '<align=right><size=28><color={color}>• {nickname}</color></size></align>'
hidden_for_roles:
- Overwatch
spectator_list_y_cordinate: 800
# Ammo Counter:
enable_ammo_counter: true
weapon_name: '<align=center><size=28><color={color}>{weapon}</color></size></align>'
ammo_counter: '<align=center><size=28><b><color={color}>{current} / {max}</color></b></size></align>'
ammo_counter_y_cordinate: 150
# Spectator HUD Settings:
enable_spectator_hud: true
# You can use {displayname} instead of {nickname}
spectator_hud: '<size=22><color=#7DD3FC><b>OBSERVANDO A:</b></color> <color=#FFFFFF>{nickname}</color> <color=#94A3B8>|</color> <color=#C4B5FD><b>ROL:</b></color> {role} <color=#94A3B8>|</color> <color=#7DD3FC><b>BAJAS:</b></color> <color=#FDE68A>{kills}</color>{subclass}</size>'
spectator_hud_y_cordinate: 160
# If true, will not show the spectated player's nickname when they are a skeleton (SCP-3114). This mimics the behavior of the base-game spectator UI.
hide_skeleton_nickname: true
# Spectator Map Info:
enable_spectator_map_info: true
spectator_map_info: '<size=27><b>Generators:</b> <color=orange>{engaged}/{maxGenerators}</color> <b>| Warhead:</b> <color={warheadColor}>{warheadStatus}</color></size>'
map_info_y_cordinate: 110
# Spectator Server Info:
enable_spectator_server_info: true
spectator_server_info: '<size=20><color=#7DD3FC><b>JUGADORES:</b></color> <color=#FBBF24>{players}/{maxPlayers}</color> <color=#94A3B8>|</color> <color=#7DD3FC><b>ESPECTADORES:</b></color> <color=#FBBF24>{spectators}</color></size>'
server_info_y_cordinate: 110
```

</details>

Spectator player information is horizontally centered at vertical offset 160. Server and map information are joined into one centered row at offset 110 to prevent overlap; either section can still be disabled independently, and the remaining section keeps its own configured vertical position. Horizontal placement comes from the configured rich-text alignment; the ammo counter uses centered text without negative spacing.

## Showcase:
### Gameplay
![image](https://github.com/user-attachments/assets/8595f42f-7ffe-4443-bb54-b02407b8ac42)

### Spectator
![image](https://github.com/user-attachments/assets/51255713-8c8e-41f5-a474-8d84aa37b7e8)
