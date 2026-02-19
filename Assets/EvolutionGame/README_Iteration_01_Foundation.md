# README_Iteration_01_Foundation.md

## Что изменилось
Первая итерация — полный фундамент игры с нуля.

## Новые скрипты

### Gameplay
- `GameManager.cs` — singleton, DontDestroyOnLoad, состояния Menu/Playing/GameOver, переходы между сценами
- `ScoreManager.cs` — счёт текущей сессии
- `PlayerController.cs` — движение (WASD + ЛКМ на ПК, touch hold на мобиле), инерция, поглощение, рост, смерть
- `WorldObject.cs` — поведение объектов мира, анимация поглощения (DOTween)
- `SpawnManager.cs` — спавн/деспавн объектов вокруг игрока
- `CameraFollow.cs` — плавное следование камеры за игроком (top-down)

### ScriptableObjects
- `PlayerConfig.cs` — параметры игрока + поля для Mesh/Material (заглушки заменяются вашими 3D моделями)
- `WorldObjectConfig.cs` — параметры объектов мира + поля для Mesh/Material

### UI
- `MainMenuUI.cs` — главное меню с анимациями DOTween
- `GameHUD.cs` — HUD во время игры (счёт, стадия)
- `GameOverUI.cs` — экран смерти (счёт, рекорд, кнопки)

### Editor
- `SetupMainMenu_Iteration1.cs` — автонастройка сцены MainMenu
- `SetupGameScene_Iteration1.cs` — автонастройка сцены Game

---

## Как настроить

### 1. Требования
- Unity 2022.3.62f
- URP настроен в проекте (Universal Render Pipeline Asset назначен в Project Settings → Graphics)
- DOTween Free импортирован из Asset Store и инициализирован (Tools → DOTween Utility Panel → Setup DOTween)

### 2. Скопировать файлы
Скопируй содержимое архива в папку `Assets/` своего Unity проекта, сохраняя структуру папок.

### 3. Создать сцены
В Unity создай две сцены:
- `Assets/Scenes/MainMenu.unity`
- `Assets/Scenes/Game.unity`

Добавь обе в Build Settings (File → Build Settings → Add Open Scenes).
**Важно:** MainMenu должна быть первой (индекс 0).

### 4. Настроить сцену MainMenu
- Открой сцену `MainMenu`
- В меню Unity: **EvolutionGame → Setup Main Menu Scene (Iteration 1)**
- Сохрани сцену (Ctrl+S)

### 5. Настроить сцену Game
- Открой сцену `Game`
- В меню Unity: **EvolutionGame → Setup Game Scene (Iteration 1)**
- Сохрани сцену (Ctrl+S)

### 6. Проверь URP Renderer
- В Project Settings → Graphics → Scriptable Render Pipeline Settings укажи свой URP Asset
- В URP Asset убедись что включён **Post Processing**
- На камере в сцене Game должен быть включён **Post Processing** в компоненте Camera

---

## Как тестировать

### Запуск из MainMenu
1. Открой сцену `MainMenu`
2. Нажми Play
3. Должно появиться меню с кнопкой "ИГРАТЬ"
4. Нажми — переход в игровую сцену

### Игровая сцена
- **WASD** или **ЛКМ (hold)** — движение
- Маленькие серые сферы (scale ~0.3) — можно поглощать сразу
- Средние сферы (scale ~0.75) — поглотить нельзя пока не подрастёшь
- Большие сферы (scale ~1.6) — убивают игрока при касании
- При смерти появляется Game Over экран

### Тест прямо из Game сцены
Можно запустить сцену `Game` напрямую — GameManager инициализируется автоматически.

---

## Ожидаемый результат

- Тёмная космическая атмосфера (синий/фиолетовый фон, туман, Bloom, Vignette)
- Игрок — голубая сфера с плавным инерционным движением
- Объекты спавнятся и двигаются в случайных направлениях
- Поглощение маленьких: сфера анимированно "всасывается" к игроку
- Игрок плавно вырастает после поглощения
- При столкновении с большим объектом — игрок сжимается и появляется Game Over
- HUD показывает счёт с анимацией при изменении

---

## Замена заглушек на 3D модели (Synty)

Когда будешь готов подключить модели из POLYGON Space Pack:

1. В Project окне найди `Assets/EvolutionGame/Configs/`
2. Открой `PlayerConfig` → задай **Mesh** и **Material** из своих моделей
3. Открой `WorldObject_Small`, `WorldObject_Medium`, `WorldObject_Large` → то же самое
4. Изменения применятся автоматически при следующем запуске игры
