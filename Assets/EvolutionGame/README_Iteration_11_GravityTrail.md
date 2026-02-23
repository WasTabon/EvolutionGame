# README_Iteration_11_GravityTrail.md

## Что изменилось с Iteration 10

### Новые скрипты
- `GravitySystem.cs` — singleton, хранит список активных `GravitationalBody`. Метод `GetForceAt(position, mass)` возвращает суммарную гравитационную силу в точке. Поддерживает притяжение и отталкивание, с falloff по расстоянию
- `GravitationalBody.cs` — компонент-маркер: `mass`, `radius`, `GravityType` (Attract/Repel). Авто-регистрируется в GravitySystem при Enable/Disable
- `OrbitalSpawner.cs` — при создании Солнца/большого тела спавнит Small и Medium WorldObjects на орбиты. Управляет их возвратом в пул при уничтожении
- `OrbitalObject.cs` — двигает объект по орбите вокруг `center` Transform с заданным угловым радиусом и скоростью. Можно остановить через `StopOrbit()`

### Изменённые скрипты
- `WorldObject.cs` — в Update применяет гравитацию из `GravitySystem.GetForceAt()` к вектору движения. Если на объекте есть активный `OrbitalObject` — пропускает собственное движение (орбита управляет позицией)
- `PlayerController.cs` — в Update добавляет гравитацию из `GravitySystem` к `velocity` игрока
- `PlayerTrail.cs` — полная переработка: ширина = `baseTrailWidth * playerScale * speedFactor`. При смене стадии эволюции цвет gradient синхронизируется с `trailColor` из `EvolutionConfig`. Подписан на `EvolutionManager.OnStageChanged`
- `AbsorptionEffect.cs` — при поглощении спавнит N светящихся micro-сфер с цветом поглощённого объекта. Сферы разлетаются в случайных направлениях (DOTween), уменьшаются и исчезают за ~2.2 сек. Количество и размер масштабируются от `scaleFactor` поглощённого объекта

---

## Как настроить

**EvolutionGame → Setup Game Scene (Iteration 11)**

Добавит GravitySystem на сцену. Iteration 12 добавит врагов с GravitationalBody.

---

## Как тестировать

### Trail
- Стартуй — trail должен быть тонким (минимальная ширина)
- Начни двигаться — trail расширяется пропорционально скорости И scale игрока
- При эволюции — trail мгновенно меняет цвет на цвет новой стадии

### Micro-сферы при поглощении
- Поглоти синий объект — вылетают синие светящиеся шары
- Поглоти красный — красные
- Чем крупнее поглощённый объект — тем больше сфер и тем они крупнее
- Сферы живут ~2.2 сек, уменьшаются и исчезают

### Гравитация
- Пока нет врагов с GravitationalBody — мир ведёт себя как прежде
- После Iteration 12 (Солнца, Чёрные дыры): все объекты и игрок будут тянуться к ним
