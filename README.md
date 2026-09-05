# 2D Space Snake Game: Arcade Survival Edition

A modern, high-intensity 2D arcade survival title built in **Unity 6.3 LTS (Universal 2D)**.

Modernized and architected from an early bootcamp prototype into a responsive, zero-allocation, portfolio-grade arcade experience where **length is progression, protection, and risk**.

---

## 🚀 Game Concept & Core Philosophy

In traditional snake games, growing longer simply increases the risk of self-collision. In **Space Snake Game: Arcade Survival Edition**, snake length serves three dynamic tactical functions:

1. **Progression & Score Multipliers**: As the snake consumes energy orbs, its score and survival wave multipliers accelerate.
2. **Defensive Armor**: Asteroids impacting trailing body segments sacrifice that segment while shielding the snake's head from fatal destruction, buying precious survival time.
3. **Tactical Fuel (Boost & Invulnerability)**: By tapping `[SPACE]` or `[SHIFT]`, the player can jettison the rearmost tail segment to trigger an instant **1.75x supersonic boost** and **1.2 seconds of invulnerability**, allowing the player to smash through incoming asteroid fields and escape fatal pinch points.

---

## 🎮 Controls

The game features **Unified Dual Input Handling**, seamlessly supporting both desktop keyboards and mobile/tablet touch controls.

| Action | Desktop / Keyboard | Mobile / Touch |
| :--- | :--- | :--- |
| **Steer Left / Right** | `A` / `D` or `Left Arrow` / `Right Arrow` | Virtual Joystick (Left drag) |
| **Tactical Boost** | `Spacebar` or `Left Shift` *(Requires > 2 segments)* | Hotkey / Boost Input |
| **Pause / Resume** | `Escape` or `P` | Pause Button (Top Right) |
| **Quick Restart** | `R` or `Return` (Game Over screen) | Replay Button |
| **Main Menu / Start** | `Return` or `Spacebar` | Start Button |
| **Toggle Music** | `M` (Main Menu) | Audio Toggle Button |

---

## ✨ Key Features & Mechanics

### 1. Kinematic Marker-Trail Movement
- The snake's head operates on responsive Rigidbody2D kinematic velocity.
- Trailing segments follow a position/rotation marker historical trail with zero drift or spring jitter.
- The movement feel is agile, allowing tight, precise maneuvering through dense cosmic debris.

### 2. Hazard Systems & Dynamic Waves
- **Falling Asteroids**: Asteroids hurtle down across cosmic flight paths. A direct collision with the snake's head triggers Game Over, while hits to body segments destroy the segment and inflict hit-pause.
- **Fast Comet Hazard Variant**: Fiery orange comets spawn at high speeds (8.5 u/s), forcing rapid reflexive avoidance.
- **Wall Collisions**: Crashing into the perimeter forcefields triggers instant game termination with impact feedback.

### 3. Scoring & Skill Rewarding
- **Survival Clock**: Passive score ticks (+10 pts/sec).
- **Energy Orbs**: Consuming energy balls extends snake length and awards +100 pts.
- **Near-Miss Bonus**: Skilled near-miss passes close to falling asteroids award +50 bonus points and an auditory cue.
- **Comet / Asteroid Smash**: Obliterating asteroids using the Tactical Boost invulnerability awards +150 bonus points.
- **Persistent High Scores**: High scores and best survival times persist via Unity `PlayerPrefs`.

### 4. Audio-Visual Polish & Game Feel ("Juice")
- **Dynamic Camera Shake**: Integrated procedural camera trauma on wall crashes, body segment explosions, and asteroid obliteration.
- **Time-Scale Hit-Stop**: A 40ms microfreeze occurs whenever an asteroid detonates against a body segment, giving tangible kinetic weight to impacts.
- **Visual Boost Telegraphing**: Snake segments pulse in radiant neon cyan during boost invulnerability, transitioning into an alert warning strobe before expiring.
- **2D Sound Architecture**: Pitch-randomized audio effects (0.9x - 1.15x) for pickups and explosions eliminate repetitive ear fatigue.

---

## 🛠 Technical Architecture & Modernization

This repository underwent a full architectural refactoring to transform legacy bootcamp code into a clean, modern Unity 6 codebase.

### Performance & Memory Optimizations
- **Zero Heap Allocations in Marker Loop**: Replaced the legacy reference-type `Marker` class with a value-type `struct Marker` and pre-allocated capacity lists, eliminating hundreds of per-frame GC allocations.
- **Cached Component Lookups**: Eliminated continuous `GetComponent<MarkerParts>()` inside movement update loops by caching references in a contiguous list.
- **Dirty-Checked UI Strings**: UI texts for survival time, score, energy balls, and wave countdown are dirty-checked, completely eliminating garbage collection string churn on idle frames.

### UI & Resolution Independence
- **CanvasScaler Configuration**: All canvas elements are scaled using `Scale With Screen Size` (1920x1080 Reference Resolution, Match 0.5).
- **Responsive Bounds**: Standardized UI anchors and pivots across HUD and Game Over modals.
- **Touch Zone Isolation**: Restricted virtual joystick drag detection to the lower 80% screen area, preventing accidental occlusion of the pause button on sub-1080p and mobile aspect ratios.

---

## 📦 Project Setup & Build Instructions

### Requirements
- **Unity Version**: Unity 6.3 LTS (or later Universal 2D release)
- **Active Input Handling**: `Both` (Input System Package & Legacy Input Manager)

### Scene Build Hierarchy
1. `Assets/Scenes/MainMenu.unity` (Build Index 0)
2. `Assets/Scenes/PlayScene.unity` (Build Index 1)

### Building Standalone Executable
1. Open Unity and select **File > Build Settings...**
2. Ensure `MainMenu` is at index 0 and `PlayScene` is at index 1.
3. Select your target platform (**Windows / macOS / Linux / WebGL / Android**).
4. Click **Build** and choose your destination directory.

---

## 📂 Codebase Overview

```
Assets/
├── Materials/         # 2D Sprite & UI materials
├── Prefabs/           # Snake segments, asteroids, energy balls
├── Resources/         # Audio clips (bomb, pick, game over, BGM)
├── Scenes/
│   ├── MainMenu.unity # Title screen with start & music controls
│   └── PlayScene.unity# Main arcade gameplay arena
└── Scripts/
    ├── CameraShake.cs     # Procedural unscaled camera trauma
    ├── DestroyAsteroid.cs # Hazard trajectories, comets, collisions & near-miss
    ├── EnergyBall.cs      # Collectible positioning, growth & score rewards
    ├── GameManager.cs     # Game loop, wave timers, scoring & high scores
    ├── MarkerParts.cs     # Zero-GC value-type struct position trail buffer
    ├── MenuController.cs  # Title scene navigation & desktop shortcuts
    ├── MovementJoystick.cs# Resolution-scaled touch joystick
    ├── SnakeHeadScript.cs # Head collision dispatch & safe singleton
    ├── SnakeManager.cs    # Kinematic movement, boost mechanic & body list
    └── SoundManager.cs    # 2D SFX dispatcher with pitch randomization
```

---

## 📄 License & Credits
- **Engine**: Unity 6.3 LTS
- **Font**: Starborn (Used for arcade retro aesthetics)
- **Architecture & Polish**: Modernized as a showcase portfolio piece.
