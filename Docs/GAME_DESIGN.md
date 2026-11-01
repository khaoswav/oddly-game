# ODDLY — Game Design Document
**Version:** 1.0  
**Genre:** First-person perspective puzzle / surreal comedy  
**Platforms:** Windows and macOS  
**Languages:** Turkish and English  
**Target playtime:** approximately 2 hours for the main story; approximately 3 hours including hidden chapters  
**Players:** Single-player

## High concept
A charmingly strange puzzle adventure in which the world bends to perspective, architecture refuses to behave, and two bickering voices comment on the player's decisions. The game teases the player without making solutions unfair.

## Creative pillars
1. **Perception is a tool:** scale, distance, framing and spatial relationships are puzzle verbs.
2. **The world is an unreliable narrator:** rooms, signs, doors and menus can become part of a puzzle.
3. **Comedy reacts to the player:** repeated actions, hesitation, exploration and clever solutions unlock contextual dialogue.
4. **Cute on the surface, strange underneath:** warm materials, playful shapes, gentle music and occasional liminal unease.
5. **Fair misdirection:** jokes may lie, but puzzle logic must remain learnable and solvable.

## Originality boundary
The project takes broad inspiration from perspective puzzles and fourth-wall comedy. It must not reproduce another game's levels, signature props, dialogue, character designs, audio, UI layouts or source code. Every puzzle, room composition, script and character identity is developed specifically for ODDLY.

## Cast
- **Narrator:** charming, overconfident, sarcastic and occasionally impressed.
- **SYSTEM-02:** procedural, glitch-prone and increasingly exasperated with the Narrator.
Their arguments are scripted and triggered contextually.

## Core loop
Explore → notice an inconsistency → manipulate an object or viewpoint → test a hypothesis → solve the room → receive a character reaction → discover a new rule.

## Core mechanics
- First-person movement and inspection.
- Pick up, carry and place selected objects. While held, moving an object closer or farther changes its physical world scale in proportion to camera distance, preserving its apparent on-screen size. On release, the new world scale remains, turning depth into a core puzzle verb.
- Perspective-dependent spatial puzzles.
- Impossible-room transitions and environmental changes.
- Paired doorway loops that relocate the player within a continuous room, with signage that changes in response to traversal.
- Meta puzzles involving signs, instructions and in-world interfaces.
- The first meta puzzle teaches players to inspect a warning before deliberately disobeying it to unlock the exit.
- Chapter 1 begins with a short three-beat bilingual welcome sequence before control is handed to the player.
- The first unlocked exit opens into a narrow corridor. A seemingly helpful EXIT sign changes after the player approaches, triggering a bilingual character reaction, then the end wall lifts to reveal a larger waiting room.
- The waiting room introduces a second camera-projected frame puzzle and a separate exit, requiring the player to use depth movement, persistent world scale, apparent size and viewpoint together.
- Beyond it, a dual-scale relay chamber requires two separate objects to be resized through perspective and placed on distinct small and large pads at the same time. The player must plan the two scale states independently before the next exit unlocks.
- The next chamber uses forced perspective as architecture: two fragments at different depths must overlap in the camera view from a marked floor position. The game checks the actual screen-space alignment and viewing angle, then gives a restrained bilingual hint if the player is close but not aligned. The relay puzzle also waits for both dropped objects to physically settle before accepting their placements.
- Chapter 2 begins beyond that exit with a physical gap between two platforms. A broad plank starts at ordinary scale on a pedestal; the player must use depth-based scaling to enlarge it, place it across the gap, and let its Rigidbody settle before the far exit unlocks. The puzzle checks real placement and scale rather than granting a solution from proximity alone. Its chapter sign and character reactions are localized in Turkish and English.
- Chapter 3, Wrong Dimension, changes the spatial verb: a wall blocks the player but contains a narrow slot. The player must resize a cube so it physically fits through the opening, carry it to a landing platform on the other side, and release it so physics can settle it before the exit unlocks. The room has its own localized chapter entry, checkpoint, hints and completion banter.
- Chapter 4, System Error, turns the two-character banter into a playable conflict. The Narrator and SYSTEM-02 display contradictory three-step switch sequences; a corrupted record preserves the intended LEFT / RIGHT / LEFT order. The player must read the room, choose which voice to trust, and interact with the two physical switches in sequence. Incorrect input resets progress with contextual bilingual reactions.
- Chapter 5, Don't Trust Your Eyes, makes the exit a gaze-reversal puzzle. The player sees the exit ahead, but it only unlocks after standing on a floor marker and deliberately turning away from it for a short dwell. The room's instructions contradict the obvious visual instinct, with localized hints and character reactions.
- Chapter 6, You Again?, uses run-local behaviour telemetry as puzzle content. The game counts object drops, failed interactions/door attempts and idle episodes. Three physical choices display the player's counts; selecting the most frequent habit unlocks the exit. The answer is computed from the current run with deterministic tie-breaking, so it is not a fixed walkthrough answer.
- Chapter 7, The Room That Laughs, listens to drops and failed interactions while the player is inside the chamber. Each reaction shifts a prop, rotates it, slides a wall panel and pulses the room light. Five reactions unlock the exit, with escalating bilingual character commentary. The room stops listening when the player leaves.
- Chapter 8, The Loop, sends the player back to the start of a corridor after each traversal. Three passes rearrange props, rotate their orientation, shift the room-light colour and update the bilingual sign. On the third pass the loop breaks and the far exit unlocks.
- Chapter 9, The Wrong Audience, stages a deliberately awkward performance. The player enters the spotlight and earns a standing ovation by turning their back on the audience and holding still for 2.2 seconds. The audience turns away, the spotlight cools and the exit unlocks. Progress is shown on a bilingual stage sign.
- Chapter 10, The Unfinished Game, turns the production process into a playable space. Three TODO floor markers must be visited in the order 2 → 3 → 1. Stepping on a wrong marker resets progress; each correct marker updates its label, and completing all three unlocks the exit. The production board and dialogue are localized.
- Chapter 11, The Patch Notes, presents three physical update consoles. The player must apply releases in the non-chronological order 3.0 → 1.0 → 2.0. Each patch changes its console's scale, rotation or height and shifts the room lighting. Applying a patch out of order rolls all changes back; completing the sequence installs version 11.0 and unlocks the exit.
- Chapter 12, The Last Save, presents three archived room snapshots. Save 01 and Save 03 restore corrupted versions that rearrange the snapshot artifacts and shift the room lighting; Save 02 restores the last stable pre-crash state and unlocks the real exit. The clue is visible in the room, and the selection feedback is bilingual.
- Chapter 13, The Control Room, asks the player to route three rival channels into the stable SYSTEM-02 / Narrator / SYSTEM-02 configuration. Each channel cycles through Narrator, SYSTEM-02 and Offline; the live status board, reactive lighting and dialogue respond to every change.
- Behavior tracking for local, contextual banter.

- Hidden Chapter 04, The Room That Remembers Tomorrow, samples the player's run-local behaviour history on entry. The dominant habit (dropping objects, failed interactions/locked-door attempts, or long idle episodes) determines which of three bridges rises across the gap. The other routes retract below the floor. A clean run defaults to WAIT, ties resolve deterministically, and reaching the far platform unlocks the exit.
- Hidden Chapter 03, The Moment Between Blinks, extends the hidden route with three viewpoint stations. At each station, the player must stand on its floor mark, wait for the recurring green pulse and aim directly at a displaced echo target. The pulse window is brief, the target angles vary in height and depth, and each successful alignment advances the bilingual status and lighting. All three echoes unlock the exit.
- Hidden Chapter 02, The Room That Holds Its Breath, continues beyond the first hidden room's exit. Three pads must be completed in order; the player must stop moving and remain still for 1.8 seconds on each. Movement resets the current pad's dwell, while each completed pause changes the room light and advances the bilingual status. Completing all three opens the exit.
- Hidden Chapter 01, The Room That Forgot You, is an optional branch concealed behind a gate in the finale chamber. Three small lost fragments open the side passage. Inside, the player must face away from the route and walk backwards across three sequential marks; each successful step changes the room light and advances the bilingual status board. The exit unlocks only after all three reverse-walk steps.
- Chapter 14, The Final Decision, closes the main story with a three-way physical choice: trust the Narrator, trust SYSTEM-02, or reject both. The selected ending changes the chamber lighting and relics, unlocks the final exit, and is stored locally as the player's ending record. Each branch has distinct Turkish and English dialogue.

## Campaign structure and playtime
The main story target is approximately **2 hours**, with **about 1 additional hour** for hidden chapters, for roughly **3 hours** to see the full planned experience. These are design targets, not padding requirements. Rooms should stay surprising and mechanically distinct; the game should not stretch its runtime with empty corridors or repeated versions of the same puzzle.

The campaign is planned as **12–14 connected story sequences**, rather than a rigid set of identical-sized levels. The current story arc is:
1. **Welcome, Idiot** — onboarding, the first dishonest instruction and the first fourth-wall joke.
2. **Size Does Matter** — depth-based scale, a physical bridge and the consequences of changing object size.
3. **Wrong Dimension** — a player-blocking wall, a resizeable slot and object transfer across an impossible boundary.
4. **System Error** — instructions contradict each other and SYSTEM-02 begins overriding the Narrator.
5. **Don't Trust Your Eyes** — forced perspective, false geometry and routes that only exist from one viewpoint.
6. **You Again?** — the game remembers repeated habits and turns them into callbacks and puzzle clues.
7. **The Game Is Lying** — the interface, signage and level rules become part of the puzzle.
8. **Out of Bounds** — spaces beyond the intended play area become the intended route.
9. **Memory Leak** — earlier rooms return with altered rules and remembered objects.
10. **The Other Side** — perspective tricks begin working in reverse.
11. **Control Room** — the two voices disagree about who is actually in control.
12. **Wrong Way** — a multi-mechanic finale with choices that shape the ending.
13–14. **Optional connective sequences** — pacing and story beats may expand to 14 sequences where the finished experience needs them.

Plan **5–8 hidden chapters/rooms** for optional discoveries, plus optional replay challenges. Hidden content is designed to add approximately one hour. At least three ending variations are planned, subject to narrative implementation. The exact sequence count can change during production, but the 2-hour main-story and 3-hour full-experience targets remain the scope guardrails.

## Language and voice
All player-facing text and dialogue must exist in Turkish and English. The two language versions should be adapted for natural comedic timing rather than translated word-for-word. Voice acting is a production requirement: two distinct human-sounding performances per language, with emotional range, breath, pauses and comedic timing. Synthetic text-to-speech must not be presented as finished human performance.

## Audio direction
Warm, whimsical, playful instrumental score: felt piano, marimba, soft plucked strings and gentle ambient textures. Add comic stings, tactile object sounds and occasional near-silence. Only original or properly licensed commercial-use tracks and effects may ship.

## Accessibility and comfort
- Adjustable mouse sensitivity and field of view, saved between sessions.
- Subtitles always available.
- Separate music, ambience, dialogue and effects volume.
- Rebindable controls planned.
- Avoid rapid full-screen flashes and forced camera shake.

## First playable milestone
A single greybox room with first-person movement, one resizeable object, a placement goal, an exit door, bilingual subtitles and reactive dialogue. This proves the interaction and narrative loop before full chapter production.
