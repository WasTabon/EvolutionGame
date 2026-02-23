using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupGameScene_Iteration10
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 10)")]
    static void Setup()
    {
        EnsureBalanceConfig();
        EnsureSessionTimer();
        WireBalanceConfig();
        EnsureSessionTimeInHUD();
        EnsureSessionTimeInGameOver();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 10] Balance & Polish setup complete!");
    }

    static GameBalanceConfig GetOrCreateBalanceConfig()
    {
        string path = "Assets/EvolutionGame/Configs/GameBalanceConfig.asset";
        GameBalanceConfig cfg = AssetDatabase.LoadAssetAtPath<GameBalanceConfig>(path);
        if (cfg != null) return cfg;

        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Configs"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "Configs");

        cfg = ScriptableObject.CreateInstance<GameBalanceConfig>();
        AssetDatabase.CreateAsset(cfg, path);
        AssetDatabase.SaveAssets();
        Debug.Log("[Iteration 10] GameBalanceConfig.asset created at " + path);
        return cfg;
    }

    static void EnsureBalanceConfig()
    {
        GetOrCreateBalanceConfig();
    }

    static void EnsureSessionTimer()
    {
        if (Object.FindObjectOfType<SessionTimer>() != null) return;
        GameObject go = new GameObject("SessionTimer");
        go.AddComponent<SessionTimer>();
        Undo.RegisterCreatedObjectUndo(go, "Create SessionTimer");
    }

    static void WireBalanceConfig()
    {
        GameBalanceConfig cfg = GetOrCreateBalanceConfig();

        PlayerController pc = Object.FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            using (var so = new SerializedObject(pc))
            {
                so.FindProperty("balanceConfig").objectReferenceValue = cfg;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(pc);
        }

        SpawnManager sm = Object.FindObjectOfType<SpawnManager>();
        if (sm != null)
        {
            using (var so = new SerializedObject(sm))
            {
                so.FindProperty("balanceConfig").objectReferenceValue = cfg;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(sm);
        }

        GameEventManager gem = Object.FindObjectOfType<GameEventManager>();
        if (gem != null)
        {
            using (var so = new SerializedObject(gem))
            {
                so.FindProperty("balanceConfig").objectReferenceValue = cfg;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(gem);
        }

        ComboSystem cs = Object.FindObjectOfType<ComboSystem>();
        if (cs != null)
        {
            using (var so = new SerializedObject(cs))
            {
                so.FindProperty("balanceConfig").objectReferenceValue = cfg;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(cs);
        }
    }

    static Canvas GetGameCanvas()
    {
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") return c;
        Debug.LogWarning("[Iteration 10] GameCanvas not found!");
        return null;
    }

    static void EnsureSessionTimeInHUD()
    {
        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;

        Transform hud = canvas.transform.Find("HUD");
        if (hud == null) { Debug.LogWarning("[Iteration 10] HUD not found!"); return; }
        if (hud.Find("SessionTimer") != null) return;

        GameHUD gameHud = hud.GetComponent<GameHUD>();

        GameObject timerGo = new GameObject("SessionTimer");
        timerGo.transform.SetParent(hud, false);
        RectTransform rect = timerGo.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(16f, -16f);
        rect.sizeDelta = new Vector2(100f, 28f);

        TextMeshProUGUI tmp = timerGo.AddComponent<TextMeshProUGUI>();
        tmp.text = "0:00";
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.color = new Color(1f, 1f, 1f, 0.45f);

        if (gameHud != null)
        {
            using (var so = new SerializedObject(gameHud))
            {
                so.FindProperty("sessionTimerText").objectReferenceValue = tmp;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(gameHud);
        }

        Undo.RegisterCreatedObjectUndo(timerGo, "Create SessionTimer HUD");
    }

    static void EnsureSessionTimeInGameOver()
    {
        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;

        Transform panel = canvas.transform.Find("GameOverPanel");
        if (panel == null) { Debug.LogWarning("[Iteration 10] GameOverPanel not found!"); return; }

        Transform card = panel.Find("Card");
        if (card == null) return;
        if (card.Find("SessionTime") != null) return;

        GameOverUI gameOverUI = panel.GetComponent<GameOverUI>();

        GameObject labelGo = new GameObject("SessionTimeLabel");
        labelGo.transform.SetParent(card, false);
        RectTransform labelRect = labelGo.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(0.5f, 0f);
        labelRect.anchoredPosition = new Vector2(0f, 48f);
        labelRect.sizeDelta = new Vector2(0f, 22f);
        TextMeshProUGUI labelTmp = labelGo.AddComponent<TextMeshProUGUI>();
        labelTmp.text = "TIME";
        labelTmp.fontSize = 13;
        labelTmp.alignment = TextAlignmentOptions.Center;
        labelTmp.color = new Color(1f, 1f, 1f, 0.45f);
        labelTmp.characterSpacing = 4f;

        GameObject timeGo = new GameObject("SessionTime");
        timeGo.transform.SetParent(card, false);
        RectTransform timeRect = timeGo.AddComponent<RectTransform>();
        timeRect.anchorMin = new Vector2(0f, 0f);
        timeRect.anchorMax = new Vector2(0.5f, 0f);
        timeRect.anchoredPosition = new Vector2(0f, 22f);
        timeRect.sizeDelta = new Vector2(0f, 30f);
        Text timeTxt = timeGo.AddComponent<Text>();
        timeTxt.text = "0:00";
        timeTxt.fontSize = 24;
        timeTxt.fontStyle = FontStyle.Bold;
        timeTxt.alignment = TextAnchor.MiddleCenter;
        timeTxt.color = Color.white;
        timeTxt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        if (gameOverUI != null)
        {
            using (var so = new SerializedObject(gameOverUI))
            {
                so.FindProperty("sessionTimeText").objectReferenceValue = timeTxt;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
            EditorUtility.SetDirty(gameOverUI);
        }

        Undo.RegisterCreatedObjectUndo(labelGo, "Create SessionTimeLabel");
        Undo.RegisterCreatedObjectUndo(timeGo, "Create SessionTime");
    }
}
