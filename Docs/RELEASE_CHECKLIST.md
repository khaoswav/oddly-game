# ODDLY Release Checklist

## Project baseline

- Unity Editor: **6000.3.0f1** (Unity 6.3).
- Main scene: generated at `Assets/Scenes/Prototype.unity`.
- Runtime package dependency currently declared: `com.unity.ugui` 2.0.0.
- Target platforms: macOS and Windows.
- Languages: Turkish and English.

## Generate and validate the scene

1. Open the repository in Unity 6000.3.0f1.
2. Wait until script compilation and asset import finish.
3. Run **Oddly → Release → Validate Generated Prototype**.
4. Confirm the Console reports that generated scene validation passed.
5. Open `Assets/Scenes/Prototype.unity` and enter Play Mode.

The validation command rebuilds the generated greybox scene, then checks:
- The final epilogue and post-choice memory room.
- The ninth-record gate, Unwritten Ending and final epilogue triggers.
- The Last Echo reward.
- Exactly eight echo interactions and three final-choice triggers.
- A first-person CharacterController, FirstPersonController, FallRecovery, player camera, AudioListener and wired InteractionController.
- All eight echo pillars have unique chapter IDs, trigger colliders, visual references and status labels.
- All three final choices have unique ending indices and a non-null FinalDecisionPuzzle reference.
- The memory-room trigger references all eight visual pillars and its status label; The Last Page has its status label.
- Required finale/archive trigger objects have enabled trigger colliders.
- The generated scene is enabled in Build Settings.

The generated scene is intentionally not stored as a hand-authored scene asset in the repository. It is created by `PrototypeSceneBuilder` inside the Unity Editor.

## Batch-mode validation

Replace the executable and project paths with the local Unity installation and repository path:

```bash
/path/to/Unity -batchmode -quit -projectPath "/path/to/oddly-game" -executeMethod Oddly.Editor.OddlyReleaseTools.ValidateGeneratedPrototype -logFile "Logs/oddly-validation.log"
```

A successful process exit and the validation success log are required before packaging.

## Standalone builds

Install the matching Unity Build Support modules first.

- macOS: **Oddly → Release → Build macOS**
  - Output: `Builds/macOS/ODDLY.app`
  - Batch mode: `-executeMethod Oddly.Editor.OddlyReleaseTools.BuildMacOS`
- Windows x86-64: **Oddly → Release → Build Windows**
  - Output: `Builds/Windows/ODDLY.exe`
  - Batch mode: `-executeMethod Oddly.Editor.OddlyReleaseTools.BuildWindows`

The build commands regenerate and validate the scene before calling Unity's BuildPipeline. A failed BuildReport raises an Editor error.

## Play Mode regression checklist

- [ ] First launch creates the prototype scene and starts Chapter 01.
- [ ] Main-story chapter transitions, puzzle exits and checkpoints work.
- [ ] Hidden Chapters 01–08 can be entered and completed.
- [ ] Each of the three original endings can be selected on separate runs.
- [ ] Ending history accumulates across replay and restores on relaunch.
- [ ] The post-ending epilogue reports the selected ending and hidden-room count.
- [ ] The archive branch opens at its required progression threshold.
- [ ] All eight memory echoes can be heard and remain discovered after replay.
- [ ] The Last Echo reward unlocks only after all eight rooms and echoes.
- [ ] The ninth record remains sealed until all three endings, eight rooms, eight echoes, archive discovery and Last Echo claim are present.
- [ ] The ninth-record gate opens and The Unwritten Ending unlock persists.
- [ ] The Unwritten Ending presentation restores after relaunch.
- [ ] The Last Page epilogue appears only after the alternate ending is completed.
- [ ] The Last Page reports ending history, hidden-room progress and replay count.
- [ ] Replay increments its counter and preserves ending/archive history.
- [ ] Explicit full campaign reset clears campaign, ending, echo, reward and final-epilogue records.

## Platform QA

- [ ] macOS Apple Silicon build launches and completes the full finale flow.
- [ ] macOS Intel compatibility is verified if an Intel build is shipped.
- [ ] Windows x86-64 build launches and completes the full finale flow.
- [ ] Keyboard/mouse controls, cursor lock and UI interaction are checked on both platforms.
- [ ] Turkish and English text fit all signs and subtitle surfaces.
- [ ] Audio levels, transitions and missing/placeholder audio assets are reviewed.
- [ ] Performance, memory use, accessibility and save migration are checked.
- [ ] Clean-install and upgrade-from-existing-save paths are tested.

## Current release blockers

- Unity Editor compilation and generated-scene validation have not yet been run in this development environment.
- Play Mode regression has not yet been performed.
- macOS and Windows player builds have not yet been produced or tested.
- The repository does not currently configure a Unity CI workflow. A licensed Unity build runner and platform modules are needed for reliable automated player builds.
- The project still needs its authored/original final music and sound-design pass, plus release visuals and store assets.

Do not mark a platform as verified until its player has been built and launched on that platform.
