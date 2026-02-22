using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupGameScene_Iteration2
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 2)")]
    static void Setup()
    {
        EnsureSceneTransition();
        EnsureStarfield();
        EnsureProgressBar();
        EnsureOnboardingHint();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 2] Game Scene additions complete!");
    }

    static void EnsureSceneTransition()
    {
        if (Object.FindObjectOfType<SceneTransition>() != null) return;
        GameObject go = new GameObject("SceneTransition");
        go.AddComponent<SceneTransition>();
        Undo.RegisterCreatedObjectUndo(go, "Create SceneTransition");
    }

    static Canvas GetGameCanvas()
    {
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") return c;
        Debug.LogWarning("[Iteration 2] GameCanvas not found! Run Iteration 1 setup first.");
        return null;
    }

    static void EnsureStarfield()
    {
        Canvas canvas = GetGameCanvas();
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

        StarfieldBackground sfb = sf.AddComponent<StarfieldBackground>();
        sfb.starCount = 60;
        sfb.minSpeed = 5f;
        sfb.maxSpeed = 20f;

        Undo.RegisterCreatedObjectUndo(sf, "Create Starfield");
    }

    static void EnsureProgressBar()
    {
        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;

        Transform hudTransform = canvas.transform.Find("HUD");
        if (hudTransform == null)
        {
            Debug.LogWarning("[Iteration 2] HUD not found! Run Iteration 1 setup first.");
            return;
        }

        if (hudTransform.Find("ProgressBarRoot") != null) return;

        GameHUD gameHud = hudTransform.GetComponent<GameHUD>();

        // Progress bar — thin bar under stage text, anchored top-center
        GameObject barRoot = new GameObject("ProgressBarRoot");
        barRoot.transform.SetParent(hudTransform, false);
        RectTransform barRootRect = barRoot.AddComponent<RectTransform>();
        barRootRect.anchorMin = new Vector2(0.15f, 1f);
        barRootRect.anchorMax = new Vector2(0.85f, 1f);
        barRootRect.anchoredPosition = new Vector2(0f, -172f);
        barRootRect.sizeDelta = new Vector2(0f, 6f);

        // Background track
        Image trackImg = barRoot.AddComponent<Image>();
        trackImg.color = new Color(1f, 1f, 1f, 0.12f);

        // Fill
        GameObject fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(barRoot.transform, false);
        RectTransform fillRect = fillGo.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = Vector2.one;
        fillRect.offsetMin = Vector2.zero;
        fillRect.offsetMax = Vector2.zero;
        Image fillImg = fillGo.AddComponent<Image>();
        fillImg.color = new Color(1f, 1f, 1f, 0.8f);
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0f;

        if (gameHud != null)
        {
            using (var so = new SerializedObject(gameHud))
            {
                so.FindProperty("progressBarFill").objectReferenceValue = fillImg;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(gameHud);
        }

        Undo.RegisterCreatedObjectUndo(barRoot, "Create ProgressBar");
    }

    static void EnsureOnboardingHint()
    {
        Canvas canvas = GetGameCanvas();
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

        // Semi-dark overlay so hint is readable over gameplay
        GameObject bg = new GameObject("Bg");
        bg.transform.SetParent(hintGo.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.45f);
        bgImg.raycastTarget = false;

        // Hint text — centered
        GameObject labelGo = new GameObject("Label");
        labelGo.transform.SetParent(hintGo.transform, false);
        RectTransform labelRect = labelGo.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.1f, 0.38f);
        labelRect.anchorMax = new Vector2(0.9f, 0.62f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = labelGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "Drag to move\n\nAbsorb smaller objects\nAvoid bigger ones";
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.lineSpacing = 8f;

        // Tap to continue
        GameObject tapGo = new GameObject("TapToContinue");
        tapGo.transform.SetParent(hintGo.transform, false);
        RectTransform tapRect = tapGo.AddComponent<RectTransform>();
        tapRect.anchorMin = new Vector2(0.1f, 0.25f);
        tapRect.anchorMax = new Vector2(0.9f, 0.32f);
        tapRect.offsetMin = Vector2.zero;
        tapRect.offsetMax = Vector2.zero;
        TextMeshProUGUI tapTmp = tapGo.AddComponent<TextMeshProUGUI>();
        tapTmp.text = "TAP TO START";
        tapTmp.fontSize = 16;
        tapTmp.alignment = TextAlignmentOptions.Center;
        tapTmp.color = new Color(1f, 1f, 1f, 0.55f);

        using (var so = new SerializedObject(hint))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(hint);
        Undo.RegisterCreatedObjectUndo(hintGo, "Create OnboardingHint");
    }
}
