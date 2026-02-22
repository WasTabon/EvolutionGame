# README_Iteration_07_Audio.md

## Что изменилось с Iteration 6

### Новые скрипты
- `AudioConfig.cs` — ScriptableObject с полями для всех AudioClip: музыка меню/игры/game over, SFX поглощения (массив), роста, эволюции, смерти, кнопок UI
- `AudioManager.cs` — singleton DontDestroyOnLoad. Два AudioSource: музыка (loop + fade) и SFX (PlayOneShot). Читает и сохраняет громкость в PlayerPrefs

### Изменённые скрипты
- `PlayerController.cs` — `PlayAbsorptionSFX()` при поглощении, `PlayGrowSFX()` при росте, `PlayDeathSFX()` при смерти
- `EvolutionManager.cs` — `PlayEvolutionSFX()` при смене стадии
- `GameManager.cs` — `SetState()` теперь переключает музыку: Playing → gameMusic, GameOver → gameOverMusic, Menu → menuMusic
- `SettingsPanel.cs` — слайдеры теперь реально применяют громкость через `AudioManager.SetMusicVolume()` / `SetSFXVolume()`
- `MainMenuUI.cs` — `PlayButtonSFX()` при нажатии TAP TO PLAY
- `GameOverUI.cs` — `PlayButtonSFX()` при нажатии PLAY AGAIN и MENU

### Editor скрипты
- `SetupMainMenu_Iteration7.cs` — создаёт AudioConfig SO (если нет), добавляет AudioManager на MainMenu сцену
- `SetupGameScene_Iteration7.cs` — то же для Game сцены

---

## Как настроить

### 1. Открой MainMenu сцену
**EvolutionGame → Setup Main Menu Scene (Iteration 7)**

### 2. Открой Game сцену
**EvolutionGame → Setup Game Scene (Iteration 7)**

Оба скрипта создадут `Assets/EvolutionGame/Configs/AudioConfig.asset` если его нет.

### 3. Назначь AudioClip в AudioConfig
- Открой `Assets/EvolutionGame/Configs/AudioConfig.asset` в инспекторе
- Назначь свои аудио файлы в соответствующие поля
- Все поля опциональны — система не крашится если clip = null

### AudioClip структура
```
AudioConfig
├── Music
│   ├── Menu Music       ← фоновая музыка меню
│   ├── Game Music       ← фоновая музыка игры
│   └── Game Over Music  ← музыка на экране game over
├── SFX - Absorption
│   └── Absorption SFX[] ← массив, играет случайный (разнообразие)
├── SFX - Player
│   ├── Grow SFX         ← при росте после поглощения
│   ├── Evolution SFX    ← при смене стадии эволюции
│   └── Death SFX        ← при гибели игрока
└── SFX - UI
    └── Button Click SFX ← все кнопки
```

---

## Архитектурные детали

AudioManager живёт в **MainMenu сцене** и переживает переход в Game через DontDestroyOnLoad. На Game сцене editor скрипт тоже добавляет AudioManager — но только если его ещё нет (защита от дублирования). Это позволяет стартовать игру прямо с Game сцены в редакторе.

Смена музыки: fade out текущей за 0.5 сек → fade in новой за 0.5 сек. Если clip == null — музыка просто останавливается без краша.
