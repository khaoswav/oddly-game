# ODDLY
### A Very Wrong Little World

ODDLY is an original first-person puzzle-comedy game for macOS and Windows. It combines perspective-based spatial puzzles, impossible architecture, fourth-wall jokes and a reactive two-voice cast.

**Languages:** Turkish and English, including dialogue and subtitles.  
**Target platforms:** Windows and macOS.  
**Engine:** Unity 6.3 LTS (URP planned for the visual production pass).  
**Target playtime:** approximately 2 hours main story, approximately 3 hours with hidden content.
**Campaign scope:** 12–14 connected story sequences plus 5–8 hidden chapters/rooms.
**Current milestone:** First playable greybox prototype.

## First playable prototype

The first vertical slice is intentionally small and playable:
- First-person movement and mouse look.
- Pick up and move perspective objects through depth; their world scale changes with camera distance while apparent size stays approximately constant in hand. Drop them to keep their new scale for spatial puzzles.
- Scale-sensitive object placement with bilingual size feedback.
- A camera-projected perspective frame puzzle.
- Paired impossible doorways that loop through the same room and react with changing signs.
- A bilingual meta puzzle built around a forbidden red button and a misleading instruction.
- Turkish/English dialogue from two distinct characters, with per-event cooldowns, randomized banter and speaker handoff.
- Three-beat Turkish/English Chapter 1 opening sequence.
- A traversable exit corridor with a one-time false-exit reveal, animated hidden passage and waiting room.
- A second forced-perspective frame puzzle in the waiting room, unlocking its own exit.
- A new two-object relay chamber: move a cube and sphere through depth to create two different world scales, then place each on its matching pad to unlock the next door.
- A forced-perspective sightline chamber: stand on a marked viewpoint and align the camera with separated architectural fragments to reveal the next exit. The puzzle checks real viewport alignment and gives bilingual correction hints.
- Chapter 2 opens with a real gap between platforms. Enlarge a proportion-preserving plank through depth, place it across the gap and let its physics settle before the exit unlocks.
- Chapter 3's Wrong Dimension chamber blocks the player with a wall but lets a correctly resized object pass through a narrow slot to a landing platform on the other side.
- Chapter 4's System Error chamber turns the Narrator and SYSTEM-02's argument into a playable sequence puzzle: their instructions conflict, and the player must identify the reliable recorded order to unlock the exit.
- Chapter 5's Don't Trust Your Eyes chamber turns the obvious route into a trick: stand on the floor marker, look away from the exit, and wait for the hidden unlock.
- Chapter 6's You Again? chamber remembers the player's run. It counts drops, failed interactions and idle episodes, then asks the player to identify their most common habit.
- Chapter 7's Room That Laughs reacts to drops and failed interactions: props shift, a wall panel slides, the light pulses, and five reactions open the exit.
- Chapter 8's Loop returns the player to the corridor entrance three times while subtly rearranging the room. The third traversal breaks the loop and opens the real exit.
- Chapter 9's Wrong Audience puts the player on a theatre stage. Turning away from the audience and holding still earns an absurd standing ovation and opens the exit.
- Chapter 2 and Chapter 3 have separate checkpoints, localized entry beats and fall recovery.
- The full campaign target is approximately 2 hours for the main story and approximately 3 hours including 5–8 hidden chapters/rooms.
- The dual-scale relay only accepts objects once they have physically settled on their pads.
- Persistent title-screen options for mouse sensitivity and field of view.
- Runtime-created prototype room, HUD and title screen.

The prototype scene is generated automatically by the Unity Editor bootstrap when the project is opened. If it has not appeared, use **Oddly → Create / Repair Prototype Scene**. To regenerate an already-created scene after pulling gameplay updates, use **Oddly → Rebuild Prototype Scene**.

## Open the project
1. Install Unity Hub and Unity **6.3 LTS** with the macOS or Windows build support you need.
2. Clone or download this repository.
3. In Unity Hub, choose **Add project from disk** and select this folder.
4. Wait for scripts to compile. The editor bootstrap creates `Assets/Scenes/Prototype.unity`.
5. Open the Prototype scene and press Play.

## Current playable progression

- Chapter 1 prototype route: opening test room, false exit corridor, waiting-room perspective frame, dual-scale relay and forced-perspective sightline chamber.
- Chapter 2: a physical platform gap that must be crossed with a depth-resized plank.
- Chapter 3 opening: transfer a resized object through a player-blocking wall slot and land it beyond the barrier.
- Chapter 4 opening: resolve a three-step switch sequence while the two characters give conflicting instructions.
- Chapter 5 opening: stand on the marked spot and turn away from the exit to reveal the route.
- All current chapter text and puzzle hints are authored in Turkish and English.

## Controls
| Input | Action |
|---|---|
| WASD | Move |
| Mouse | Look |
| E | Pick up / interact |
| Mouse wheel | Move held object closer / farther; world scale changes with perspective |
| Left click | Drop held object |
| Escape | Release cursor / pause gameplay input |
| Left click (cursor released) | Resume and capture mouse |

## Repository layout
- `Assets/Oddly/Scripts`: gameplay systems.
- `Assets/Oddly/Editor`: prototype scene generator.
- `Assets/Oddly/Localization`: bilingual dialogue source.
- `Docs`: design and production notes.
- `ThirdPartyNotices.md`: third-party asset and license register.

## Important production notes
- All gameplay code in this initial milestone is original project code, not copied from Superliminal or FEVER MEME.
- No third-party game assets, sounds or music are bundled yet.
- Temporary prototype visuals use Unity primitives and built-in resources.
- Final human-quality voice performances and a properly licensed music pack are planned production deliverables. The prototype uses text subtitles and placeholder UI feedback; it does not pretend to contain finished voice acting.
- Keep source and license records for every external asset added later.

## Build status
This repository is source code, not a prebuilt Mac/Windows application. A Unity Editor installation is required to generate and run the project locally. Automated editor/build validation will be added as the project grows.
