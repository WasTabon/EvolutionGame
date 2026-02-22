# README_Iteration_08_Models.md

## Что изменилось с Iteration 7

### Новые скрипты
- `ModelConfig.cs` — ScriptableObject. Центральное место для всех Mesh и Material ссылок: три типа объектов (Small/Medium/Large) + 5 стадий игрока. Заполняется вручную через инспектор
- `ModelInitializer.cs` — применяет модели из ModelConfig в WorldObjectConfig и EvolutionConfig при старте сцены через Awake. Работает только если поля в ModelConfig заполнены (null-safe)

### Изменённые скрипты
- `EvolutionConfig.cs` — добавлено поле `playerMesh` в `EvolutionStageData`
- `EvolutionManager.cs` — `ApplyStage()` теперь меняет Mesh игрока (если назначен) + обновляет emission цвет материала согласно стадии
- `AbsorptionEffect.cs` — `Play()` принимает опциональный `Color?`. Частицы берут цвет из материала поглощённого объекта
- `PlayerController.cs` — передаёт цвет MeshRenderer поглощённого объекта в `AbsorptionEffect.Play()`
- `ParallaxStarfield.cs` — оптимизация: один shared Material на слой вместо отдельного на каждую звезду. Убраны per-frame MaterialPropertyBlock вызовы

### Editor скрипт
- `SetupGameScene_Iteration8.cs` — создаёт `ModelConfig.asset`, добавляет `ModelInitializer` на сцену со всеми ссылками на конфиги, усиливает Bloom/ColorAdjustments для реальных 3D моделей

---

## Как подключить Synty POLYGON Sci-Fi Space

### 1. Запусти editor скрипт
**EvolutionGame → Setup Game Scene (Iteration 8)**

Создаст `Assets/EvolutionGame/Configs/ModelConfig.asset`

### 2. Открой ModelConfig в инспекторе

Назначь Synty меши и материалы:

**World Objects:**
```
Small Mesh     → небольшой астероид или частица (напр. SM_Prop_Asteroid_01)
Small Material → соответствующий Synty материал
Medium Mesh    → средний астероид (напр. SM_Prop_Asteroid_03)
Medium Material
Large Mesh     → крупный объект (напр. SM_Env_Planet_01)
Large Material
```

**Player Stage Models (5 стадий):**
```
Stage 0 - Spark         → маленький энергетический шар / кристалл
Stage 1 - Young Star    → чуть крупнее, другой меш
Stage 2 - Bright Star   → звезда среднего размера
Stage 3 - Supernova     → взрывающийся объект
Stage 4 - Galactic Core → крупнейший объект в паке
```

### 3. Проверь WorldObjectConfig
`Assets/EvolutionGame/Configs/WorldObject_Small/Medium/Large.asset`
- ModelInitializer перезапишет `mesh` и `material` из ModelConfig при старте
- Можно назначить напрямую в WorldObjectConfig — ModelInitializer не тронет если поле в ModelConfig пустое

### 4. EvolutionConfig
`Assets/EvolutionGame/Configs/EvolutionConfig.asset`
- Поля `Player Mesh` и `Player Material` для каждой стадии заполняются из ModelConfig через ModelInitializer
- Можно заполнить напрямую в EvolutionConfig — ModelInitializer не перезаписывает если StageModel.mesh == null

---

## Как тестировать

- При старте сцены ModelInitializer применит модели — игрок и объекты должны выглядеть как Synty меши
- При достижении новой стадии эволюции меняется меш и материал игрока
- Частицы AbsorptionEffect при поглощении синего объекта — синие, при поглощении красного — красные
- Bloom усилен — emissive материалы Synty светятся ярче

---

## Если Synty меши ещё не готовы
Игра полностью работает без назначенных моделей — все поля null-safe. Модели можно добавить в любой момент.
