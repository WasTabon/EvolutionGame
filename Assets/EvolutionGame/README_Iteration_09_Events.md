# README_Iteration_09_Events.md

## Что изменилось с Iteration 8

### Новые скрипты
- `GameEventManager.cs` — singleton, запускает случайное событие каждые 25-45 сек. После завершения события ждёт следующего. Три типа: StarStorm / GravitationalWave / Hunter
- `StarStormEvent.cs` — умножает скорость спавна в 3 раза на 10 сек. Баннер: "STAR STORM — Incoming debris field!"
- `GravitationalWaveEvent.cs` — притягивает игрока к центру (0,0,0) нарастающей силой на 6 сек. Синусоидальный профиль: нарастает → пик → спадает
- `HunterEvent.cs` — спавнит крупного красного врага (scale 2.2), преследует игрока 15 сек. Если игрок вырос до 85% размера Hunter → Hunter убегает. При столкновении — смерть
- `HazardousObject.cs` — объект которого нельзя поглотить. Мигает красным при приближении игрока (radius 3.5). При контакте — смерть. Используется как строительный блок для будущих уровней
- `EventAnnouncementUI.cs` — баннер появляется сверху с slide-in анимацией, держится 2 сек, уходит вверх. Цветная полоска-акцент меняется под тип события

### Изменённые скрипты
- `PlayerController.cs` — в `Update()` добавлена гравитационная сила от `GravitationalWaveEvent`. Добавлен публичный метод `ForceKill()` — вызывается от HazardousObject, HunterContactKiller
- `SpawnManager.cs` — добавлены `SetSpawnMultiplier(float)` и поле `spawnMultiplier`. Spawn interval и max objects масштабируются на множитель

### Editor скрипт
- `SetupGameScene_Iteration9.cs` — добавляет GameEventManager, GravitationalWaveEvent, EventAnnouncement UI на GameCanvas

---

## Как настроить

**EvolutionGame → Setup Game Scene (Iteration 9)**

---

## Как тестировать

Для быстрого теста временно уменьши `minInterval` и `maxInterval` в `GameEventManager` до 5 / 10.

### Star Storm
- Поле заполняется объектами значительно быстрее
- Баннер с синей полоской появляется сверху
- Через 10 сек интенсивность возвращается к норме

### Gravitational Wave
- Игрок начинает притягиваться к центру карты
- Сила нарастает к середине события и спадает
- Оранжевый баннер сверху

### Hunter
- Появляется крупный красный шар рядом с игроком
- Преследует игрока пока тот маленький
- Если вырасти достаточно — начинает убегать
- Через 15 сек исчезает с анимацией
- Красный баннер сверху

### HazardousObject (используется программно)
```csharp
GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
go.AddComponent<HazardousObject>();
```
Красно мигает при приближении, убивает при касании.

---

## Архитектурные детали

`GameEventManager` содержит все три компонента события как `AddComponent` в Awake. Только одно событие активно одновременно — `eventRunning` флаг. `GravitationalWaveEvent` имеет собственный singleton для доступа из PlayerController без прямой зависимости.
