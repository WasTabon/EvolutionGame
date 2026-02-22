using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupGameScene_Iteration6
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 6)")]
    static void Setup()
    {
        EnsureComboSystem();
        EnsurePopupPool();
        EnsureComboIndicatorInHUD();
        EnsureStageReachedInGameOver();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 6] Score & Combo system setup complete!");
    }

    static void EnsureComboSystem()
    {
        if (Object.FindObjectOfType<ComboSystem>() != null) return;
        GameObject go = new GameObject("ComboSystem");
        go.AddComponent<ComboSystem>();
        Undo.RegisterCreatedObjectUndo(go, "Create ComboSystem");
    }

    static Canvas GetGameCanvas()
    {
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") return c;
        Debug.LogWarning("[Iteration 6] GameCanvas not found! Run Iteration 1 setup first.");
        return null;
    }

    static void EnsurePopupPool()
    {
        if (Object.FindObjectOfType<ScorePopupPool>() != null) return;

        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;

        if (canvas.transform.Find("ScorePopupPool") != null) return;

        // Pool root on canvas
        GameObject poolRoot = new GameObject("ScorePopupPool");
        poolRoot.transform.SetParent(canvas.transform, false);
        RectTransform poolRect = poolRoot.AddComponent<RectTransform>();
        poolRect.anchorMin = Vector2.zero;
        poolRect.anchorMax = Vector2.one;
        poolRect.offsetMin = Vector2.zero;
        poolRect.offsetMax = Vector2.zero;

        ScorePopupPool pool = poolRoot.AddComponent<ScorePopupPool>();

        // Create prefab-like child that acts as template
        GameObject popupGo = new GameObject("FloatingScoreText_Template");
        popupGo.transform.SetParent(poolRoot.transform, false);

        RectTransform popupRect = popupGo.AddComponent<RectTransform>();
        popupRect.sizeDelta = new Vector2(160f, 50f);
        popupRect.anchorMin = new Vector2(0.5f, 0.5f);
        popupRect.anchorMax = new Vector2(0.5f, 0.5f);

        TextMeshProUGUI tmp = popupGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "+10";
        tmp.fontSize = 28;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        FloatingScoreText fst = popupGo.AddComponent<FloatingScoreText>();
        using (var so = new SerializedObject(fst))
        {
            so.FindProperty("text").objectReferenceValue = tmp;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        popupGo.SetActive(false);

        using (var so = new SerializedObject(pool))
        {
            so.FindProperty("prefab").objectReferenceValue = fst;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(pool);
        Undo.RegisterCreatedObjectUndo(poolRoot, "Create ScorePopupPool");
    }

    static void EnsureComboIndicatorInHUD()
    {
        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;

        Transform hudTransform = canvas.transform.Find("HUD");
        if (hudTransform == null)
        {
            Debug.LogWarning("[Iteration 6] HUD not found!");
            return;
        }

        if (hudTransform.Find("ComboIndicator") != null) return;

        GameHUD gameHud = hudTransform.GetComponent<GameHUD>();

        // Combo indicator — bottom center
        GameObject comboRoot = new GameObject("ComboIndicator");
        comboRoot.transform.SetParent(hudTransform, false);
        RectTransform comboRect = comboRoot.AddComponent<RectTransform>();
        comboRect.anchorMin = new Vector2(0.5f, 0f);
        comboRect.anchorMax = new Vector2(0.5f, 0f);
        comboRect.anchoredPosition = new Vector2(0f, 120f);
        comboRect.sizeDelta = new Vector2(160f, 50f);

        CanvasGroup comboCG = comboRoot.AddComponent<CanvasGroup>();
        comboCG.alpha = 0f;
        comboCG.interactable = false;
        comboCG.blocksRaycasts = false;

        TextMeshProUGUI comboTmp = comboRoot.AddComponent<TextMeshProUGUI>();
        comboTmp.text = "x1";
        comboTmp.fontSize = 34;
        comboTmp.fontStyle = FontStyles.Bold;
        comboTmp.alignment = TextAlignmentOptions.Center;
        comboTmp.color = Color.white;

        if (gameHud != null)
        {
            using (var so = new SerializedObject(gameHud))
            {
                so.FindProperty("comboText").objectReferenceValue = comboTmp;
                so.FindProperty("comboCG").objectReferenceValue = comboCG;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(gameHud);
        }

        Undo.RegisterCreatedObjectUndo(comboRoot, "Create ComboIndicator");
    }

    static void EnsureStageReachedInGameOver()
    {
        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;

        Transform panelTransform = canvas.transform.Find("GameOverPanel");
        if (panelTransform == null)
        {
            Debug.LogWarning("[Iteration 6] GameOverPanel not found!");
            return;
        }

        GameOverUI gameOverUI = panelTransform.GetComponent<GameOverUI>();
        if (gameOverUI == null) return;

        Transform card = panelTransform.Find("Card");
        if (card == null) return;

        if (card.Find("StageReached") != null) return;

        // Stage reached label — between title and score
        GameObject stageLabelGo = new GameObject("StageReachedLabel");
        stageLabelGo.transform.SetParent(card, false);
        RectTransform stageLabelRect = stageLabelGo.AddComponent<RectTransform>();
        stageLabelRect.anchorMin = new Vector2(0f, 1f);
        stageLabelRect.anchorMax = new Vector2(1f, 1f);
        stageLabelRect.anchoredPosition = new Vector2(0f, -100f);
        stageLabelRect.sizeDelta = new Vector2(0f, 24f);
        TextMeshProUGUI stageLabelTmp = stageLabelGo.AddComponent<TextMeshProUGUI>();
        stageLabelTmp.text = "REACHED";
        stageLabelTmp.fontSize = 14;
        stageLabelTmp.alignment = TextAlignmentOptions.Center;
        stageLabelTmp.color = new Color(1f, 1f, 1f, 0.45f);
        stageLabelTmp.characterSpacing = 4f;

        GameObject stageNameGo = new GameObject("StageReached");
        stageNameGo.transform.SetParent(card, false);
        RectTransform stageNameRect = stageNameGo.AddComponent<RectTransform>();
        stageNameRect.anchorMin = new Vector2(0f, 1f);
        stageNameRect.anchorMax = new Vector2(1f, 1f);
        stageNameRect.anchoredPosition = new Vector2(0f, -126f);
        stageNameRect.sizeDelta = new Vector2(0f, 32f);

        // Stage reached uses legacy Text to stay consistent with existing GameOverUI fields
        Text stageText = stageNameGo.AddComponent<Text>();
        stageText.text = "SPARK";
        stageText.fontSize = 24;
        stageText.fontStyle = FontStyle.Bold;
        stageText.alignment = TextAnchor.MiddleCenter;
        stageText.color = new Color(0.6f, 0.85f, 1f);
        stageText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        using (var so = new SerializedObject(gameOverUI))
        {
            so.FindProperty("stageReachedText").objectReferenceValue = stageText;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(gameOverUI);
        Undo.RegisterCreatedObjectUndo(stageLabelGo, "Create StageReachedLabel");
        Undo.RegisterCreatedObjectUndo(stageNameGo, "Create StageReached");
    }
}
