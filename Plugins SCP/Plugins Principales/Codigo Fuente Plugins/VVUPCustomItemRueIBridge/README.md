# VVUP Custom Item RueI Bridge

Routes plugin hints through independent RueI overlays so they can appear
alongside UltimateHUD instead of replacing the shared native hint. VVUP
CustomItems pickup and selection notifications retain their configured message
templates, visibility flags, and durations. The SCP Proximity Chat toggle
notice is centered at vertical position 335, styled as 25-point bold light red,
and displayed for 2.5 seconds.
On every player connection, the bridge also displays
`¡Bienvenido a Pentagoland!` right-aligned at vertical position 750 in size 36
for 5 seconds. Custom-item notifications use a separate lane at vertical
position 650; ragdoll-cleanup messages use a separate lane at position 500.

The bridge routes EXILED `Player.ShowHint`, LabAPI `Player.SendHint`, and
direct native `HintDisplay.Show` plugin calls through RueI. The native hook
uses the already-formatted hint parameters, covering plugins that bypass those
two wrappers. CustomItems, EffectDisplay, SubclassFramework, Proximity Chat,
and the welcome message use separate positions and tags. Other plugin hints
share a default overlay; concurrent messages from distinct calling methods are
composed there, repeated messages from the same method update in place, and
each message expires independently. Game-originated hints and RueI's own
rendering calls are left untouched.

Requires EXILED 9.14.2, RueI 3.1.2, and the EXILED CustomItems plugin. Build with:

```powershell
dotnet build .\VVUPCustomItemRueIBridge.csproj -c Release
```
