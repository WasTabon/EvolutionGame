# README_Iteration_06_ScoreCombo.md

## Что изменилось с Iteration 5

### Новые скрипты
- `ComboSystem.cs` — singleton. Отслеживает серию поглощений. Каждое поглощение в течение 2.5 сек повышает множитель: x1 → x1.5 → x2 → x2.5 → x3. Пауза дольше 2.5 сек — сброс
- `FloatingScoreText.cs` — TMP pop-up над точкой поглощения. Показывает очки и множитель, улетает вверх и исчезает. Цвет зависит от множителя
- `ScorePopupPool.cs` — pool для FloatingScoreText. Создаёт 10 объектов при старте, переиспользует их

### Изменённые скрипты
- `ScoreManager.cs` — `AddScore(float, Vector3)` теперь запрашивает множитель у ComboSystem, применяет его к очкам и вызывает ScorePopupPool. Добавлены `GetBestScore()` и `SaveBestScore()` через PlayerPrefs
- `PlayerController.cs` — передаёт `worldPos` в `AddScore`
- `GameHUD.cs` — подписан на `ComboSystem.OnComboChanged` / `OnComboReset`. Показывает комбо-индикатор внизу экрана. Вызывает `ScoreManager.SaveBestScore()` при GameOver
- `GameOverUI.cs` — анимированный счётчик очков (число накручивается за 1.2 сек), показывает достигнутую стадию эволюции
- `EvolutionManager.cs` — добавлены публичные методы `GetCurrentStageIndex()` и `GetCurrentStageName()`

### Editor скрипт
- `SetupGameScene_Iteration6.cs` — добавляет ComboSystem, ScorePopupPool с template объектом, ComboIndicator в HUD, StageReached текст в GameOverPanel

---

## Как настроить

### Открой сцену Game
- **EvolutionGame → Setup Game Scene (Iteration 6)**
- Сохрани сцену

---

## Как тестировать

### Комбо
- Поглощай объекты быстро подряд — комбо индикатор внизу экрана покажет x1.5, x2, x2.5, x3
- Перестань поглощать на 2.5 сек — индикатор исчезнет, комбо сбросится

### Floating score
- При каждом поглощении над объектом появляется очки
- При активном комбо: цвет pop-up меняется (зелёный → жёлтый → оранжевый → фиолетовый)
- При высоком комбо текст крупнее

### Game Over
- Счёт накручивается анимацией за ~1.2 сек
- Под заголовком GAME OVER показывается достигнутая стадия
- Рекорд сохраняется в PlayerPrefs и отображается корректно
