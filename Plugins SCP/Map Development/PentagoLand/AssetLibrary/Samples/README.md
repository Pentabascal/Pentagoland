# Native ProjectMER samples

`Schematics/PentagolandConsole/PentagolandConsole.json` is an original, primitive-only MER schematic. It uses built-in networked cube and sphere toys rather than a custom mesh or game files.

`Schematics/PentagolandRelayTerminal/PentagolandRelayTerminal.json` is a coordinated, primitive-only approximation of the Blender relay-terminal asset. Both schematics are starting models, not verified production placements.

`PentagolandSecurityAlarm`, `PentagolandContainmentCabinet` and `PentagolandSecurityBollard` are primitive-only facility prop approximations coordinated with the Blender asset pack.

The utility set adds `PentagolandAccessReader`, `PentagolandSecurityCamera`, `PentagolandCeilingLight`, `PentagolandWallVent`, `PentagolandFirstAidCase`, `PentagolandPowerJunction`, `PentagolandCargoCrate` and `PentagolandPipeSegment` for general corridor and room dressing.

Eight additional, Blender-coordinated examples cover `PentagolandSlidingBulkhead`, `PentagolandWallSign`, `PentagolandFireExtinguisher`, `PentagolandCeilingSensor`, `PentagolandFloorDrain`, `PentagolandCableTray`, `PentagolandWarningBeacon` and `PentagolandServerRack`. These are decorative static props made from stock MER primitives. The warning beacon also includes a steady point light; none of the other models implies an interaction, animation, or gameplay behavior.

Twenty more schematic examples extend the set: `PentagolandAirlockControlPanel`, `PentagolandWallFireHose`, `PentagolandBiohazardWasteBin`, `PentagolandSpecimenCanister`, `PentagolandDeconShower`, `PentagolandFloorCableCover`, `PentagolandCeilingSpeaker`, `PentagolandWallClock`, `PentagolandSecurityPillar`, `PentagolandWorkbench`, `PentagolandToolCart`, `PentagolandIndustrialFan`, `PentagolandPipeValve`, `PentagolandPressureGauge`, `PentagolandCryoPod`, `PentagolandObservationWindow`, `PentagolandBlastDoorControl`, `PentagolandHandRailSection`, `PentagolandUtilityLocker` and `PentagolandHazardBarrel`. Each uses a reduced set of silhouette primitives to keep its network-object count lower than the corresponding Blender model.

Twenty-one further props extend the set: `PentagolandChemicalSafetyShower`, `PentagolandBiometricScanner`, `PentagolandWallIntercom`, `PentagolandMedicalSupplyTrolley`, `PentagolandAirScrubber`, `PentagolandPortableWorkLight`, `PentagolandFloorWarningBeacon`, `PentagolandWallFirstAidDispenser`, `PentagolandPipeElbow`, `PentagolandWallCableReel`, `PentagolandWallMonitor`, `PentagolandEmergencyPhone`, `PentagolandCeilingSprinkler`, `PentagolandWallJunctionBox`, `PentagolandStorageDrum`, `PentagolandWallBollard`, `PentagolandHazardPlacard`, `PentagolandVentilationDuct`, `PentagolandRadiationMeter`, `PentagolandCeilingAccessPanel` and `PentagolandDecontaminationCanister`. They are static decorative approximations, not functioning equipment.

The final ten additions are `PentagolandElevatorCallPanel`, `PentagolandCeilingEmergencyLight`, `PentagolandWallHydrantCabinet`, `PentagolandFloorServiceHatch`, `PentagolandChemicalStorageLocker`, `PentagolandLabCentrifuge`, `PentagolandPortableGenerator`, `PentagolandArchiveTerminal`, `PentagolandDeconSink` and `PentagolandSecureStorageSafe`.

Ten facility-support props extend the set: `PentagolandGasDetector`, `PentagolandWallPatchPanel`, `PentagolandPortableDeconUnit`, `PentagolandCeilingDomeCamera`, `PentagolandEmergencyShutoffValve`, `PentagolandSpecimenTransportCase`, `PentagolandWallChargingStation`, `PentagolandMobileMedicalMonitor`, `PentagolandIndustrialWaterPump` and `PentagolandMobileEvidenceCart`.

Five later additions bring the portable set to 87 schematics: `PentagolandEmergencyRelayConsole`, `PentagolandContainmentObservationBay`, `PentagolandResearchAnnex`, `PentagolandDecontaminationSuite` and `PentagolandSecurityCheckpoint`. The last three room modules include open sub-areas, static dressing, lighting and probabilistic vanilla pickups. They are placement-ready starting layouts, not moving doors or interactive gameplay systems; review orientation, loot, collision and spacing in staging.

To try a sample, copy its folder into the server's `LabAPI/configs/ProjectMER/Schematics/` directory. On a staging server, use the in-game Remote Admin command `mapeditor create <name>`, aiming at a suitable surface, or provide explicit coordinates as `mapeditor create <name> <x> <y> <z>`. The local source requires the `mpr.create` permission. Inspect position, scale, orientation, collision and visibility in game. Do not save or deploy a map until it has been reviewed.

The schematic parts use visible-only primitives to avoid changing player collision. Each part is still a networked toy; the new examples keep to roughly 3-7 parts each, but avoid placing large numbers without a staging performance check. They are static decorations, not interactive props or physical simulations.

Validate the JSON locally before copying it:

```powershell
dotnet run --project ..\PentagolandMERTools.csproj -- .\Schematics
```

The matching editable Blender models and exports are in `../Assets/`. They are not loaded by the native MER schematics.
