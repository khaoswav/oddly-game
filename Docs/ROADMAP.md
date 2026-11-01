# Production Roadmap

## Milestone 0 — Repository foundation
- [x] Create Unity project structure.
- [x] Add bilingual dialogue foundation.
- [x] Add first-person controller and interaction prototype.
- [x] Add editor-generated greybox scene.
- [ ] Verify import and play mode in Unity 6.3 LTS.
- [ ] Add automated editor build validation.

## Milestone 1 — First playable vertical slice
- [x] Prevent movement and object interaction while the mouse cursor is released; support click-to-resume.
- [ ] Fix and validate all prototype interactions.
- [x] Add scale-sensitive object placement with readable too-large / too-small feedback.
- [x] Add camera-projected, apparent-size perspective frame puzzles to the first room and waiting room.
- [x] Preserve apparent size when picking up objects from different distances.
- [x] Move held objects along camera depth; dynamically scale world size to preserve apparent size, retaining the new scale on drop.
- [x] Add a two-object, two-scale relay chamber with independent placement pads and a linked exit.
- [x] Add a marked-viewpoint sightline chamber where separated architectural fragments resolve into a single visual symbol, with screen-space alignment checks and contextual bilingual hints.
- [x] Require both relay objects to settle physically on their pads before the exit unlocks.
- [x] Add Chapter 2's physical perspective bridge: a depth-resized plank must span a real platform gap, settle on both ledges and unlock the far exit.
- [x] Add a localized Chapter 2 entry beat, title sign and reactive bilingual bridge hints.
- [x] Add a Chapter 2 checkpoint, player fall recovery and automatic reset for dropped props that fall out of the playable space.
- [ ] Expand perspective-dependent placement into larger multi-step forced-perspective room tricks, object combinations and linked follow-on rooms.
- [x] Add a paired impossible-room doorway loop with reactive bilingual dialogue and changing signs.
- [x] Add a bilingual notice-and-forbidden-button meta puzzle that unlocks the exit.
- [x] Add per-event dialogue cooldowns and varied bilingual character banter with speaker handoff.
- [x] Add event-driven Narrator / SYSTEM-02 reactions to pickup habits, repeated drops, failed interactions, door spam and prolonged idle behaviour.
- [x] Add escalating bilingual reaction tiers with a short delayed handoff so character commentary does not overwrite immediate interaction feedback.
- [x] Add a persistent options menu for mouse sensitivity and field of view, alongside saved language selection.
- [x] Add a three-beat bilingual Chapter 1 opening sequence with a deliberate control handoff.
- [x] Extend the unlocked exit into a traversable corridor with a bilingual false-exit reveal and animated passage into a waiting room.
- [x] Add a persistent three-bus audio routing foundation for music, ambience and one-shot effects, with saved volume levels and music crossfades.
- [x] Add an original procedurally synthesized ambient bed and contextual door, reveal and environmental-surprise sound cues.
- [ ] Compose the original music score and complete the authored/recorded sound-design pass.
- [x] Add full-screen bilingual cinematic chapter cards with fade transitions for the main story and all eight hidden chapters.
- [x] Persist current/furthest campaign chapter, hidden-room visits and the selected ending.
- [x] Persist chapter-entry checkpoints and restore the latest safe location on relaunch.
- [x] Add an ending-specific campaign completion card and campaign-progress reset API.
- [x] Add a one-shot corridor set-piece where a lying exit sign changes its story, props animate into impossible positions and lighting shifts around the player.
- [x] Smooth door openings and animate Chapter 8 loop prop/light rearrangements instead of snapping the environment instantly.
- [x] Add a Chapter 8-to-9 encore corridor with a localized sign, rising/spinning props and reactive lighting.
- [ ] Build more distinct room-to-room transitions and surprising set-piece events so progression feels like a continuous surreal comedy adventure, not a row of repeated puzzle rooms.

## Milestone 2 — Visual identity
- [ ] Establish URP lighting and post-processing.
- [ ] Create original modular room kit.
- [ ] Create original props and interaction animations.
- [ ] Create title/menu motion design.

## Milestone 3 — Narrative and audio
- [ ] Complete Turkish and English script bible.
- [ ] Record/commission two distinct voice performances in both languages.
- [ ] Add dialogue event and emotion metadata.
- [ ] Add licensed/original music and SFX with asset register.

## Milestone 4 — Two-hour main story campaign
- [x] Build the Chapter 2 physical perspective bridge and recovery checkpoint.
- [x] Build the first Chapter 3 Wrong Dimension slot-transfer puzzle with a separate checkpoint and bilingual feedback.
- [x] Build the Chapter 4 System Error sequence puzzle with contradictory character instructions, physical switches, bilingual reactions and a separate checkpoint.
- [x] Build the Chapter 5 Don't Trust Your Eyes gaze-reversal puzzle with a marked viewpoint, hidden-exit unlock condition, bilingual hints and a separate checkpoint.
- [x] Build the Chapter 6 You Again? adaptive habit puzzle, recording drops, retries and idle episodes and deriving the correct choice from the player's current run.
- [x] Build the Chapter 7 Room That Laughs reaction chamber, with player-action listeners, moving props, a shifting wall panel, light pulses, escalating dialogue and a reaction-based exit unlock.
- [x] Build Chapter 8 The Loop, a three-pass spatial reset that changes room props, lighting and signage before breaking the loop and unlocking the exit.
- [x] Build Chapter 9 The Wrong Audience stage puzzle, with a spotlight zone, audience silhouettes, bilingual live progress sign, back-to-audience performance condition and standing-ovation exit reward.
- [x] Build Chapter 10 The Unfinished Game, with three TODO markers, ordered traversal, wrong-step reset, bilingual production board and exit unlock.
- [x] Build Chapter 11 The Patch Notes, with three physical update consoles, non-chronological patch order, world-state changes, rollback on conflicts and bilingual version status.
- [x] Build Chapter 12 The Last Save, with three restorable snapshots, corrupted-state prop and lighting changes, a discoverable stable-save clue and a bilingual exit unlock.
- [x] Build Chapter 13 The Control Room, with three cyclic ownership consoles, live channel routing, reactive props and lighting, bilingual character conflict and a stable configuration exit unlock.
- [x] Build the 14th main-story sequence and three-way final decision chamber.
- [ ] Complete pacing and content needed for approximately 2 hours of main-story playtime.
- [x] Build Hidden Chapter 01, The Room That Forgot You, with three collectible fragments, a concealed side gate and a three-stage backwards-walking challenge.
- [x] Build Hidden Chapter 02, The Room That Holds Its Breath, with a three-pad sequential stillness challenge, movement-sensitive timing, reactive lighting and bilingual feedback.
- [x] Build Hidden Chapter 03, The Moment Between Blinks, with three pulse-timed sightline stations, varied target angles, reactive lighting and bilingual dialogue.
- [x] Build Hidden Chapter 04, The Room That Remembers Tomorrow, with behaviour-history sampling and a dynamically selected physical bridge route.
- [x] Build Hidden Chapter 05, The Room That Borrows Your Hands, with inverted movement controls, an alternating slalom route, a checkpoint and bilingual control-return feedback.
- [x] Build 8 hidden chapters/rooms targeting approximately 1 additional hour.
- [x] Build Hidden Chapter 06, The Room That Exists When Watched, with three gaze-reactive barriers that appear and collide only while observed, plus checkpoint, bilingual hints and exit progression.
- [x] Add three distinct ending variations with localized dialogue, reactive finale visuals and a locally saved ending choice.
- [x] Build Hidden Chapter 07, The Room That Steals Your Shadow, with a mirrored shadow actor, three alternating light-pool targets, dynamic status, reactive lighting and bilingual dialogue.
- [x] Build Hidden Chapter 08, The Room That Remembers You, with a persistent completion archive, saved-finale recall puzzle, three interactive memory panels and bilingual feedback.
- [ ] Add alternate solutions and broader narrative callbacks.
- [ ] Add optional replay challenges and achievements.
- [ ] Add persistent campaign saves and chapter checkpoints.

## Milestone 5 — Release
- [ ] macOS Intel/Apple Silicon compatibility testing.
- [ ] Windows build and controller testing.
- [ ] Performance, accessibility and localization QA.
- [ ] Steam store assets and release candidate.
