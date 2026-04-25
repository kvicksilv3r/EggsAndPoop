# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**EggsAndPoop** is a Unity mobile game (idle/casual) where players collect eggs over time, crack them open to hatch animals, and watch those animals roam a farm. The project lives at `EggsAndPoop/` (the Unity project root).

## Tone & Design Philosophy

The game aims to create **warm emotional connections** between the player and their animals. Every feature should ask: does this make the player feel something about their animals?

- Copy and UI text should feel cozy, cute, and a little whimsical — never technical or gamey. "Still a little shy. Give them some time..." not "Quirk unlocks in 2 days".
- Animals should feel like individuals with inner lives, not stats. Quirks, names, ages, and personalities exist to build attachment, not to add complexity.
- Reward patience and presence. Features like time-locked quirks exist to make returning players feel like their animals *grew* while they were gone.
- Avoid anything that feels like pressure. This is a feel-good idle game — no punishment mechanics, no urgency.

## Build & Development

This is a Unity project — there is no CLI build command. Open `EggsAndPoop/EggsAndPoop.sln` in Visual Studio or Rider for C# editing with IntelliSense. Build and run via the Unity Editor.

Save data is stored as JSON at `Application.persistentDataPath + "/SaveData.txt"`. During development, pressing **L** loads the save and **S** saves manually (see `DataController.cs`).

Scriptable assets use the `EAP/` menu path in the Unity Editor (`AnimalData`, `EggData`, `DropRates`, `InventoryConfig`).

## Architecture

All scripts live in `EggsAndPoop/Assets/Script/`.

### Event Bus — `GameManager` (singleton)
`GameManager` is a DontDestroyOnLoad singleton that owns all cross-system `UnityEvent`s. Systems communicate by subscribing to these events rather than calling each other directly. Key events:
- `m_SetupEgg` → prepare the egg-opening scene
- `m_EggCrackedOpen` → egg animation complete, trigger animal resolution
- `m_EggCrackedOpenPost` → animal registered, trigger save + UI refresh + animal spawn
- `m_EggContextOpened/Closed` → UI context change for the egg screen
- `m_AfkDataProcessed` → AFK egg math complete, used to show WelcomeBackScreen
- `m_NukeSaveData` → skips save on quit (used by `NukeAllSaveData` debug tool)

### Persistence — `DataController` / `DataIO` / `SaveData`
`DataController` coordinates save/load. `DataIO` reads/writes JSON via `JsonUtility` to `persistentDataPath`. `SaveData` is a plain serializable class — all persistent state lives here. Systems write into `SaveData` via `ModifySaveData(ref SaveData)` methods on `PlayerInventoryManager` and `EggTimer`.

### Inventory — `PlayerInventoryManager` (singleton)
Single source of truth for runtime inventory: `playerEggs` (list of `PlayerEggEntry`), `playerAnimals` (list of `PlayerAnimalEntry`), and unlocked egg types. Egg capacity is `InventoryConfig.baseMaxOwnedEggs + extendedEggCapacity`. Animal capacity is `InventoryConfig.baseMaxOwnedAnimals`.

### Egg Lifecycle
1. `EggTimer` calculates AFK-earned eggs (1 egg per `InventoryConfig.hoursForNewEgg` hours) and calls `PlayerInventoryManager.AddEggs()` on startup.
2. Player opens `GameStateController` to `GameContext.Egg` context → `EggInventoryUI` shows available eggs as `EggUiButton`s.
3. Clicking an egg calls `EggOpeningController.InitiateEggOpening(EggData)` → fires `m_SetupEgg`.
4. After crack animation, `m_EggCrackedOpen` fires → `EggOpeningController.EggCrackedOpen()` runs the full resolution chain: randomize animal via `AnimalRedeeming`, show visuals, register to inventory, remove egg.
5. `m_EggCrackedOpenPost` fires → triggers save, UI refresh, and `PhysicalAnimalController.InstantiateAnimals()`.

### Animal Randomization — `AnimalRedeeming` / `DropRates`
`AnimalRedeeming.OpenEgg()` picks a rarity by rolling against the `DropRates` scriptable object (array of `DropRatePair` with `dropRate` thresholds), then selects a random animal of that rarity from the matching `AnimalFamily` pool in `AnimalRoster`.

### Rosters — `AnimalRoster` / `EggRoster`
Both load all their scriptable objects at startup via `Resources.LoadAll<T>()` from `Resources/AnimalData/` and `Resources/EggData/` respectively. Look up animals by `AnimalEnum` identifier; eggs by `EggType`.

### Physical Animals — `PhysicalAnimal` / `PhysicalAnimalBehaviour` / `PhysicalAnimalController`
`PhysicalAnimalController` spawns prefabs for all owned animals (keyed by GUID). Each instantiated animal is a two-layer prefab: the generic `physicalAnimalPrefab` wrapper (holds `NavMeshAgent`, `PhysicalAnimal`, `PhysicalAnimalBehaviour`) with the species-specific prefab (from `AnimalData.prefab`) nested inside at the `visualHolster`. `PhysicalAnimalBehaviour` runs the `Frolicking` coroutine — a state machine cycling between Idle/Walk/Eat with configurable probabilities. Animals can be dragged with touch/mouse via Unity's `IDragHandler` interface. Position and forward direction are persisted in `PhysicalAnimalData` inside `PlayerAnimalEntry`.

### UI Context System — `GameStateController`
Manages a stack of `GameContext` enum values. Each context maps to a set of GameObjects to show/hide (`ContextMap` list). Back/Escape pops the stack. Current contexts: `Main`, `Settings`, `Home`, `Egg`.

### Data Flow on Startup (Bootstrapper order)
`Bootstrapper.bootstrapEvents` (configured in the scene) fires on `Start()`. Typical order: `DataController.CheckForSave()` → load or create fresh save → `PlayerInventoryManager.LoadInventory()` → `EggTimer.SetupTimer()` (calculates AFK eggs, fires `m_AfkDataProcessed`) → `WelcomeBackScreen.Display()`.
