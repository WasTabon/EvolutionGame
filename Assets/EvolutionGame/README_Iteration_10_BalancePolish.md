# README_Iteration_10_BalancePolish.md

## Что изменилось с Iteration 9

### Новые скрипты
- `GameBalanceConfig.cs` — ScriptableObject, все числовые параметры баланса в одном месте: скорости игрока, пороги поглощения/смерти, параметры спавна, комбо, все параметры трёх событий. Тюнишь без лезть в код
- `SessionTimer.cs` — singleton, подписан на `GameManager.OnStateChanged`. При Playing — запускает таймер, при GameOver — останавливает. `GetFormatted()` возвращает строку "M:SS". `OnTick` event для обновления UI в реальном времени

### Изменённые скрипты
- `GameOverUI.cs` — добавлено поле `sessionTimeText` (Text), показывает время сессии из SessionTimer
- `GameHUD.cs` — добавлено поле `sessionTimerText` (TMP), обновляется каждый тик через `SessionTimer.OnTick`. Маленький, полупрозрачный, верхний левый угол
- `PlayerController.cs` — добавлено поле `balanceConfig`. Если назначен — использует его значения вместо PlayerConfig для: baseSpeed, minSpeed, speedScalePenalty, inertiaSmoothing, absorptionThreshold, deathThreshold
- `SpawnManager.cs` — добавлено поле `balanceConfig`. В `Start()` перезаписывает spawnRadius, despawnRadius, spawnInterval, maxObjects
- `GameEventManager.cs` — добавлено поле `balanceConfig`. В `Awake()` прокидывает параметры во все три события
- `ComboSystem.cs` — добавлено поле `balanceConfig`. В `Awake()` читает `comboResetTime`

### Editor скрипты
- `SetupGameScene_Iteration10.cs` — создаёт `GameBalanceConfig.asset`, добавляет `SessionTimer`, прокидывает ссылки на балансный конфиг во все системы, добавляет SessionTimer в HUD и GameOver UI
- `SetupMainMenu_Iteration10.cs` — два чеклиста в меню: "Final Checklist" для MainMenu сцены и "Final Checklist Game Scene" для Game сцены. Выводит ✓ / ✗ по каждой системе

---

## Как настроить

### Game сцена
**EvolutionGame → Setup Game Scene (Iteration 10)**

Создаст `GameBalanceConfig.asset` и SessionTimer, прокинет ссылки.

### Финальная проверка
- MainMenu сцена: **EvolutionGame → Final Checklist (Iteration 10)**
- Game сцена: **EvolutionGame → Final Checklist Game Scene (Iteration 10)**

Смотри Console — красные ✗ указывают на незаполненные ссылки.

---

## GameBalanceConfig — параметры

| Секция | Параметр | Дефолт | Описание |
|--------|----------|--------|---------|
| Player Movement | baseSpeed | 5 | Базовая скорость |
| Player Movement | minSpeed | 1.5 | Минимальная скорость при большом scale |
| Player Movement | speedScalePenalty | 0.4 | Замедление за единицу роста |
| Player Movement | inertiaSmoothing | 8 | Плавность ускорения |
| Absorption | absorptionThreshold | 0.9 | Объект < 90% размера → поглощаем |
| Absorption | deathThreshold | 1.1 | Объект > 110% → смерть |
| Spawn | baseSpawnInterval | 0.8 | Интервал спавна на 1й стадии |
| Spawn | baseMaxObjects | 35 | Макс объектов на 1й стадии |
| Combo | comboResetTime | 2.5 | Сек без поглощения → сброс комбо |
| Events | eventMinInterval | 25 | Мин пауза между событиями |
| Events | eventMaxInterval | 45 | Макс пауза между событиями |

---

## Рекомендации по тюнингу

**Игра слишком сложная:**
- Увеличь `absorptionThreshold` до 0.95 (поглощаем объекты почти своего размера)
- Уменьши `deathThreshold` до 1.05

**Движение вялое:**
- Увеличь `baseSpeed` до 6-7
- Уменьши `inertiaSmoothing` до 5 (резче)

**События слишком частые:**
- Увеличь `eventMinInterval` / `eventMaxInterval`

**Star Storm слишком хаотичный:**
- Уменьши `starStormMultiplier` до 2

---

## Итог проекта — что реализовано за 10 итераций

1. Core gameplay — движение, поглощение, спавн, GameManager
2. UI Foundation — переходы сцен, звёздный фон, настройки, онбординг
3. Game Feel — trail, particles, camera shake, улучшенная анимация поглощения
4. Эволюция — 5 стадий, смена материала/меша, прогресс-бар, UI анонс
5. Динамика мира — Object Pool, DifficultyManager, ParallaxStarfield, scatter поведение
6. Очки и комбо — ComboSystem, FloatingScoreText, улучшенный GameOver
7. Аудио — AudioManager, AudioConfig, музыка с fade, SFX на все действия
8. 3D модели — ModelConfig, ModelInitializer, подключение Synty паков
9. События — StarStorm, GravitationalWave, Hunter, EventAnnouncementUI
10. Баланс — GameBalanceConfig, SessionTimer, финальные чеклисты
