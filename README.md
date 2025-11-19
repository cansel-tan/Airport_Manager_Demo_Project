# Airport Manager Demo Case

Concise technical brief for the Airport Manager mobility case. Focus is on demonstrating loop clarity (passenger flow → monetization → upgrade unlock) under a clean architecture layout.

## Snapshot

- Unity 2022.3.62f3 LTS, Android IL2CPP/ARM64 target (`AirportManager.apk` in repo root).
- Scenes: `MainScene` (terminal core loop), `PaintScene` (upgrade & cosmetics).
- Input: single thumb joystick (`Controls.Joystick`) driving `PlayerController` over `CharacterController`.

## Architecture

- **Domain** (`Gameplay`, `Devices`, `Utilities`): entities like `Passengers`, `Baggage`, `PlayerWallet`, `MoneyZoneWithFill`.
- **Application**: coordinators (`PassengerManager`, `PassengerBoardingManager`, `CameraSwitcher`, `ObjectiveArrowController`).
- **Presentation**: Unity scenes + UGUI/TextMeshPro for HUD, pad visuals, joystick.
- **Infrastructure**: packages (`com.unity.textmeshpro`, `com.unity.timeline`, `com.unity.ugui`) plus `ProjectSettings` for Android build profile.

## Loop Highlights

- Passenger routing via waypoint injection, boarding coroutine manages XRAY → plane path and revenue events.
- Bag handling stack/drop (`PlayerBaggageStack`, `BaggageDropZone`, `BaggageTruckLoader`) keeps spacing deterministic.
- Economy pads (`MoneyZoneWithFill`, `BoardMoneyZoneWithFill`) emit rewards, toggle visuals, and unlock upper-area triggers through `MainCameraController`.

## Build Notes

1. Open with Unity 2022.3.62f3, ensure Android platform and add `MainScene` + `PaintScene` in Build Settings.
2. Player Settings: IL2CPP, ARM64 only, no scripting defines required.
3. Produce `AirportManager.apk` and deploy to physical device for touch validation.

## APK

- Drive link for reviewers: https://drive.google.com/file/d/1emWsWyjJa3a7gQ_0warW-feyRtTf3Xxp/view?usp=sharing
