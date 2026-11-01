# ODDLY
### A Very Wrong Little World

ODDLY is an original first-person puzzle-comedy game for macOS and Windows. It combines perspective-based spatial puzzles, impossible architecture, fourth-wall jokes and a reactive two-voice cast.

**Languages:** Turkish and English, including dialogue and subtitles.  
**Target platforms:** Windows and macOS.  
**Engine:** Unity 6.3 LTS (URP planned for the visual production pass).  
**Current milestone:** First playable greybox prototype.

## First playable prototype

The first vertical slice is intentionally small and playable:
- First-person movement and mouse look.
- Pick up, resize and place a perspective object.
- Scale-sensitive object placement with bilingual size feedback.
- A camera-projected perspective frame puzzle.
- Paired impossible doorways that loop through the same room and react with changing signs.
- A bilingual meta puzzle built around a forbidden red button and a misleading instruction.
- Per-event dialogue cooldowns, randomized banter and narrator/system speaker handoff.
- Two-character dialogue system with Turkish and English lines, contextual reactions and varied banter.
- Runtime-created prototype room, HUD and title screen.

The prototype scene is generated automatically by the Unity Editor bootstrap when the project is opened. If it has not appeared, use **Oddly → Create / Repair Prototype Scene**.

## Open the project
1. Install Unity Hub and Unity **6.3 LTS** with the macOS or Windows build support you need.
2. Clone or download this repository.
3. In Unity Hub, choose **Add project from disk** and select this folder.
4. Wait for scripts to compile. The editor bootstrap creates `Assets/Scenes/Prototype.unity`.
5. Open the Prototype scene and press Play.

## Controls
| Input | Action |
|---|---|
| WASD | Move |
| Mouse | Look |
| E | Pick up / interact |
| Mouse wheel | Resize held object |
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
