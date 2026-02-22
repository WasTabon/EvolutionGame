using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupMainMenu_Iteration2
{
    [MenuItem("EvolutionGame/Setup Main Menu Scene (Iteration 2)")]
    static void Setup()
    {
        EnsureSceneTransition();
        EnsureStarfield();
        EnsureSettingsButton();
        EnsureSettingsPanel();
        EnsureOnboardingHint();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 2] Main Menu additions complete!");
    }

    static void EnsureSceneTransition()
    {
        if (Object.FindObjectOfType<SceneTransition>() != null) return;
        GameObject go = new GameObject("SceneTransition");
        go.AddComponent<SceneTransition>();
        Undo.RegisterCreatedObjectUndo(go, "Create SceneTransition");
    }

    static Canvas GetMainCanvas()
    {
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "MainMenuCanvas") return c;
        Debug.LogWarning("[Iteration 2] MainMenuCanvas not found! Run Iteration 1 setup first.");
        return null;
    }

    static void EnsureStarfield()
    {
        Canvas canvas = GetMainCanvas();
        if (canvas == null) return;
        if (canvas.transform.Find("Starfield") != null) return;

        GameObject sf = new GameObject("Starfield");
        sf.transform.SetParent(canvas.transform, false);
        sf.transform.SetAsFirstSibling();

        RectTransform rect = sf.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        sf.AddComponent<StarfieldBackground>();
        Undo.RegisterCreatedObjectUndo(sf, "Create Starfield");
    }

    static void EnsureSettingsButton()
    {
        Canvas canvas = GetMainCanvas();
        if (canvas == null) return;

        Transform menuRoot = canvas.transform.Find("MenuRoot");
        if (menuRoot == null)
        {
            Debug.LogWarning("[Iteration 2] MenuRoot not found!");
            return;
        }

        if (menuRoot.Find("SettingsButton") != null) return;

        GameObject btnGo = new GameObject("SettingsButton");
        btnGo.transform.SetParent(menuRoot, false);

        RectTransform rect = btnGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.anchoredPosition = new Vector2(-20f, -20f);
        rect.sizeDelta = new Vector2(60f, 60f);

        Image img = btnGo.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.12f);

        Button btn = btnGo.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(1f, 1f, 1f, 0.12f);
        cb.highlightedColor = new Color(1f, 1f, 1f, 0.22f);
        cb.pressedColor = new Color(1f, 1f, 1f, 0.06f);
        cb.fadeDuration = 0.08f;
        btn.colors = cb;

        GameObject label = new GameObject("Icon");
        label.transform.SetParent(btnGo.transform, false);
        RectTransform labelRect = label.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = label.AddComponent<TextMeshProUGUI>();
        tmp.text = "⚙";
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 1f, 1f, 0.8f);

        btn.onClick.AddListener(() =>
        {
            SettingsPanel sp = Object.FindObjectOfType<SettingsPanel>();
            if (sp != null) sp.Show();
        });

        Undo.RegisterCreatedObjectUndo(btnGo, "Create SettingsButton");
    }

    static void EnsureSettingsPanel()
    {
        Canvas canvas = GetMainCanvas();
        if (canvas == null) return;
        if (canvas.transform.Find("SettingsPanel") != null) return;

        GameObject panelGo = new GameObject("SettingsPanel");
        panelGo.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        CanvasGroup cg = panelGo.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        SettingsPanel sp = panelGo.AddComponent<SettingsPanel>();

        // Dimmed bg
        GameObject bg = new GameObject("Bg");
        bg.transform.SetParent(panelGo.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.6f);
        Button bgBtn = bg.AddComponent<Button>();
        bgBtn.onClick.AddListener(() => sp.Hide());
        ColorBlock cbBg = bgBtn.colors;
        cbBg.normalColor = Color.white;
        cbBg.highlightedColor = Color.white;
        cbBg.pressedColor = Color.white;
        bgBtn.colors = cbBg;
        bgBtn.transition = Selectable.Transition.None;

        // Card
        GameObject card = new GameObject("Card");
        card.transform.SetParent(panelGo.transform, false);
        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.08f, 0.3f);
        cardRect.anchorMax = new Vector2(0.92f, 0.75f);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0.05f, 0.03f, 0.12f, 0.97f);

        // Title
        GameObject titleGo = new GameObject("Title");
        titleGo.transform.SetParent(card.transform, false);
        RectTransform titleRect = titleGo.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -44f);
        titleRect.sizeDelta = new Vector2(0f, 50f);
        TextMeshProUGUI titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "SETTINGS";
        titleTmp.fontSize = 28;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;

        // Music label
        CreateSettingsLabel(card.transform, "MusicLabel", "MUSIC", new Vector2(0f, -110f));

        // Music slider
        Slider musicSlider = CreateSlider(card.transform, "MusicSlider", new Vector2(0f, -155f));

        // SFX label
        CreateSettingsLabel(card.transform, "SFXLabel", "SFX", new Vector2(0f, -200f));

        // SFX slider
        Slider sfxSlider = CreateSlider(card.transform, "SFXSlider", new Vector2(0f, -245f));

        // Close button
        GameObject closeGo = new GameObject("CloseButton");
        closeGo.transform.SetParent(card.transform, false);
        RectTransform closeRect = closeGo.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(0.15f, 0f);
        closeRect.anchorMax = new Vector2(0.85f, 0f);
        closeRect.anchoredPosition = new Vector2(0f, 44f);
        closeRect.sizeDelta = new Vector2(0f, 50f);
        Image closeImg = closeGo.AddComponent<Image>();
        closeImg.color = new Color(1f, 1f, 1f, 0.12f);
        Button closeBtn = closeGo.AddComponent<Button>();
        ColorBlock cbClose = closeBtn.colors;
        cbClose.normalColor = new Color(1f, 1f, 1f, 0.12f);
        cbClose.highlightedColor = new Color(1f, 1f, 1f, 0.22f);
        cbClose.pressedColor = new Color(1f, 1f, 1f, 0.06f);
        cbClose.fadeDuration = 0.08f;
        closeBtn.colors = cbClose;

        GameObject closeLabelGo = new GameObject("Label");
        closeLabelGo.transform.SetParent(closeGo.transform, false);
        RectTransform closeLabelRect = closeLabelGo.AddComponent<RectTransform>();
        closeLabelRect.anchorMin = Vector2.zero;
        closeLabelRect.anchorMax = Vector2.one;
        closeLabelRect.offsetMin = Vector2.zero;
        closeLabelRect.offsetMax = Vector2.zero;
        TextMeshProUGUI closeTmp = closeLabelGo.AddComponent<TextMeshProUGUI>();
        closeTmp.text = "CLOSE";
        closeTmp.fontSize = 22;
        closeTmp.fontStyle = FontStyles.Bold;
        closeTmp.alignment = TextAlignmentOptions.Center;
        closeTmp.color = Color.white;

        using (var so = new SerializedObject(sp))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("musicSlider").objectReferenceValue = musicSlider;
            so.FindProperty("sfxSlider").objectReferenceValue = sfxSlider;
            so.FindProperty("closeButton").objectReferenceValue = closeBtn;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(sp);
        Undo.RegisterCreatedObjectUndo(panelGo, "Create SettingsPanel");
    }

    static void CreateSettingsLabel(Transform parent, string name, string text, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(0f, 26f);
        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 1f, 1f, 0.55f);
    }

    static Slider CreateSlider(Transform parent, string name, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.08f, 1f);
        rect.anchorMax = new Vector2(0.92f, 1f);
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(0f, 36f);

        Slider slider = go.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 1f;

        // Background
        GameObject bg = new GameObject("Background");
        bg.transform.SetParent(go.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0.25f);
        bgRect.anchorMax = new Vector2(1f, 0.75f);
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(1f, 1f, 1f, 0.12f);

        // Fill area
        GameObject fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(go.transform, false);
        RectTransform fillAreaRect = fillArea.AddComponent<RectTransform>();
        fillAreaRect.anchorMin = new Vector2(0f, 0.25f);
        fillAreaRect.anchorMax = new Vector2(1f, 0.75f);
        fillAreaRect.offsetMin = new Vector2(5f, 0f);
        fillAreaRect.offsetMax = new Vector2(-15f, 0f);

        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImg = fill.AddComponent<Image>();
        fillImg.color = new Color(1f, 1f, 1f, 0.85f);

        // Handle
        GameObject handleArea = new GameObject("Handle Slide Area");
        handleArea.transform.SetParent(go.transform, false);
        RectTransform handleAreaRect = handleArea.AddComponent<RectTransform>();
        handleAreaRect.anchorMin = Vector2.zero;
        handleAreaRect.anchorMax = Vector2.one;
        handleAreaRect.offsetMin = new Vector2(10f, 0f);
        handleAreaRect.offsetMax = new Vector2(-10f, 0f);

        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(handleArea.transform, false);
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.sizeDelta = new Vector2(24f, 24f);
        Image handleImg = handle.AddComponent<Image>();
        handleImg.color = Color.white;

        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImg;
        slider.direction = Slider.Direction.LeftToRight;

        return slider;
    }

    static void EnsureOnboardingHint()
    {
        Canvas canvas = GetMainCanvas();
        if (canvas == null) return;
        if (canvas.transform.Find("OnboardingHint") != null) return;

        GameObject hintGo = new GameObject("OnboardingHint");
        hintGo.transform.SetParent(canvas.transform, false);
        RectTransform rect = hintGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        CanvasGroup cg = hintGo.AddComponent<CanvasGroup>();
        OnboardingHint hint = hintGo.AddComponent<OnboardingHint>();

        // Label
        GameObject labelGo = new GameObject("Label");
        labelGo.transform.SetParent(hintGo.transform, false);
        RectTransform labelRect = labelGo.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.1f, 0.08f);
        labelRect.anchorMax = new Vector2(0.9f, 0.18f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        TextMeshProUGUI tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "Drag to move · Absorb smaller objects · Avoid bigger ones\nTap to continue";
        tmp.fontSize = 17;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = new Color(1f, 1f, 1f, 0.75f);

        using (var so = new SerializedObject(hint))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(hint);
        Undo.RegisterCreatedObjectUndo(hintGo, "Create OnboardingHint");
    }
}
