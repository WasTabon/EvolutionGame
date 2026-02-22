# README_Iteration_02_UIFoundation.md

## Что изменилось с Iteration 1

### Новые скрипты
- `SceneTransition.cs` — singleton DontDestroyOnLoad, плавный fade-in/out между сценами. Все переходы теперь через него
- `StarfieldBackground.cs` — анимированные звёзды на UI слое. Мерцают и медленно летят вниз. Работает на обеих сценах
- `SettingsPanel.cs` — панель настроек. Слайдеры Music/SFX сохраняются в PlayerPrefs (заглушка, аудио подключится в Iteration 7)
- `OnboardingHint.cs` — подсказка при первом запуске. Показывается один раз, тап — закрывает. Флаг в PlayerPrefs

### Изменённые скрипты
- `GameManager.cs` — StartGame / RestartGame / GoToMenu теперь используют SceneTransition если он есть
- `GameHUD.cs` — добавлено поле `progressBarFill` (Image), метод `SetProgress(float)` для обновления прогресс-бара

### Editor скрипты
- `SetupMainMenu_Iteration2.cs` — добавляет на Menu сцену: Starfield, кнопку Settings (⚙ топ-право), SettingsPanel, OnboardingHint
- `SetupGameScene_Iteration2.cs` — добавляет на Game сцену: SceneTransition, Starfield, прогресс-бар под stage text, OnboardingHint

---

## Как настроить

### 1. Открой сцену MainMenu
- **EvolutionGame → Setup Main Menu Scene (Iteration 2)**
- Сохрани сцену

### 2. Открой сцену Game
- **EvolutionGame → Setup Game Scene (Iteration 2)**
- Сохрани сцену

Скрипты только дополняют то что уже есть — ничего не пересоздают.

---

## Как тестировать

### Fade переходы
- MainMenu → нажать PLAY → должен быть плавный fade to black → fade in на Game сцене
- Game Over → PLAY AGAIN / MENU → то же самое

### Starfield
- На обеих сценах должны мерцать и лететь вниз белые точки-звёзды на фоне

### Settings
- В меню — кнопка ⚙ в правом верхнем углу
- Нажать → панель появляется с анимацией (scale + fade)
- Слайдеры двигаются, значения сохраняются
- Нажать CLOSE или на тёмный фон → панель закрывается

### Onboarding
- При первом запуске (или после очистки PlayerPrefs) на Game сцене показывается подсказка
- Тап → закрывается, игра начинается
- При повторном запуске подсказка не появляется

### Progress bar
- Тонкая белая полоска под текстом стадии (пока всегда пустая — заполнение подключится в Iteration 3 с системой эволюции)

---

## Ожидаемый результат
- Все переходы плавные, без резких прыжков между сценами
- Звёзды на фоне создают ощущение движения в космосе
- Settings открывается/закрывается с анимацией
- При первом запуске игрок видит подсказку управления
