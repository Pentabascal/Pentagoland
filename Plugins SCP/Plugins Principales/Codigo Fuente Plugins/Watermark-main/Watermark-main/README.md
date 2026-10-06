# Watermark ![GitHub Downloads (all assets, all releases)](https://img.shields.io/github/downloads/polyshed-dev/Watermark/total)
Simple [EXILED](https://github.com/ExMod-Team/EXILED) plugin that displays a permanent hint on screen using the [RueI](https://github.com/pawslee/RueI) hint framework. It can be useful as a watermark for server owners.

The watermark is a persistent RueI element with a private tag, so it composes with other RueI HUD elements instead of replacing their hints. It is also added to players who are already connected when the plugin is enabled and removed when the plugin is disabled. The default vertical position is 5 (near the bottom edge); adjust `position` if another HUD element uses the same screen area.

Plugins that bypass RueI and send raw game hints can temporarily replace the visible hint. RueI redraws its persistent elements after that hint expires; Watermark does not repeatedly overwrite raw hints, since doing so would interfere with their intended display.
# Depencencies
- EXILED `9.14.2`: https://github.com/ExMod-Team/EXILED
- RueI hint framework `3.1.2`: https://github.com/pawslee/RueI
# How to install
1. Install RueI `3.1.2` in the LabAPI `plugins/global` folder.
2. Put `Watermark.dll` in your EXILED plugins folder.
3. Restart your server to activate the plugin.
# Contact
Please contact me if the plugin is broken/has issues/doesn't work as intended.

Discord: [polyshed.](https://discord.com/users/783962692150624316)
