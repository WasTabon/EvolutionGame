# README_Iteration_04_Evolution.md

## Что изменилось с Iteration 3

### Новые скрипты
- `EvolutionConfig.cs` — ScriptableObject с массивом из 5 стадий. Каждая стадия: название, порог scale, Material игрока, цвет trail, ширина trail, интенсивность emission
- `EvolutionManager.cs` — singleton, следит за scale игрока. При достижении порога: меняет материал и trail, вызывает анимацию перехода, обновляет HUD
- `EvolutionStageTransition.cs` — UI компонент: белая вспышка + название стадии появляется по центру и уходит вверх

### Изменённые скрипты
- `PlayerController.cs` — метод `Grow()` теперь уведомляет `EvolutionManager.OnPlayerScaleChanged()`

### Editor скрипт
- `SetupGameScene_Iteration4.cs` — создаёт `EvolutionConfig` SO с 5 стадиями и материалами, добавляет `EvolutionManager`, создаёт `StageTransition` UI на GameCanvas

---

## 5 стадий

| # | Название | Scale порог | Цвет |
|---|----------|-------------|------|
| 1 | Spark | 0.5 (старт) | Синий |
| 2 | Young Star | 1.2 | Голубой |
| 3 | Bright Star | 2.5 | Жёлтый |
| 4 | Supernova | 4.5 | Оранжевый |
| 5 | Galactic Core | 7.0 | Фиолетовый |

---

## Как настроить

### Открой сцену Game
- **EvolutionGame → Setup Game Scene (Iteration 4)**
- Сохрани сцену

Создаст:
- `Assets/EvolutionGame/Configs/EvolutionConfig.asset`
- `Assets/EvolutionGame/Materials/Player_Spark.mat` ... `Player_GalacticCore.mat`
- `EvolutionManager` на сцене
- `StageTransition` UI в GameCanvas

---

## Как тестировать

### Прогресс-бар
- Тонкая полоска под текстом стадии должна заполняться по мере роста игрока

### Переход стадий
- При достижении порога scale: белая вспышка на экране + название стадии появляется по центру и плавно уходит вверх
- Игрок делает scale pulse (DOPunchScale)
- Материал игрока меняется на цвет новой стадии
- Trail меняет цвет и ширину

### Быстрый тест
Для быстрой проверки перехода временно уменьши пороги в `EvolutionConfig` в инспекторе. Например, Young Star = 0.8, Bright Star = 1.2 и т.д.

### Текст HUD
- При переходе стадии текст под счётом обновляется на новое название

---

## Ожидаемый результат
- Игрок начинает синим, постепенно меняет цвет через голубой → жёлтый → оранжевый → фиолетовый
- Каждый переход ощущается как событие — вспышка, анимация
- Прогресс-бар даёт игроку понимание как далеко до следующей стадии
