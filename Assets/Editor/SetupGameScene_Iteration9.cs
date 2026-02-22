using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupGameScene_Iteration9
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 9)")]
    static void Setup()
    {
        EnsureGameEventManager();
        EnsureGravitationalWaveEvent();
        EnsureEventAnnouncementUI();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 9] Events & Gameplay Depth setup complete!");
    }

    static void EnsureGameEventManager()
    {
        if (Object.FindObjectOfType<GameEventManager>() != null) return;
        GameObject go = new GameObject("GameEventManager");
        go.AddComponent<GameEventManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create GameEventManager");
    }

    static void EnsureGravitationalWaveEvent()
    {
        if (Object.FindObjectOfType<GravitationalWaveEvent>() != null) return;
        GameObject go = new GameObject("GravitationalWaveEvent");
        go.AddComponent<GravitationalWaveEvent>();
        Undo.RegisterCreatedObjectUndo(go, "Create GravitationalWaveEvent");
    }

    static Canvas GetGameCanvas()
    {
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") return c;
        Debug.LogWarning("[Iteration 9] GameCanvas not found! Run Iteration 1 setup first.");
        return null;
    }

    static void EnsureEventAnnouncementUI()
    {
        Canvas canvas = GetGameCanvas();
        if (canvas == null) return;
        if (canvas.transform.Find("EventAnnouncement") != null) return;

        GameObject root = new GameObject("EventAnnouncement");
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = new Vector2(0f, 1f);
        rootRect.anchorMax = new Vector2(1f, 1f);
        rootRect.pivot = new Vector2(0.5f, 1f);
        rootRect.anchoredPosition = new Vector2(0f, 0f);
        rootRect.sizeDelta = new Vector2(0f, 90f);

        CanvasGroup cg = root.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        EventAnnouncementUI ui = root.AddComponent<EventAnnouncementUI>();

        // Panel — slides in from top
        GameObject panelGo = new GameObject("Panel");
        panelGo.transform.SetParent(root.transform, false);
        RectTransform panelRect = panelGo.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.05f, 0f);
        panelRect.anchorMax = new Vector2(0.95f, 1f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        Image panelImg = panelGo.AddComponent<Image>();
        panelImg.color = new Color(0f, 0f, 0f, 0.75f);

        // Accent line — top edge
        GameObject accentGo = new GameObject("AccentLine");
        accentGo.transform.SetParent(panelGo.transform, false);
        RectTransform accentRect = accentGo.AddComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0f, 1f);
        accentRect.anchorMax = new Vector2(1f, 1f);
        accentRect.pivot = new Vector2(0.5f, 1f);
        accentRect.anchoredPosition = Vector2.zero;
        accentRect.sizeDelta = new Vector2(0f, 3f);
        Image accentImg = accentGo.AddComponent<Image>();
        accentImg.color = new Color(0.4f, 0.8f, 1f);

        // Subtitle — small label above title
        GameObject subtitleGo = new GameObject("Subtitle");
        subtitleGo.transform.SetParent(panelGo.transform, false);
        RectTransform subtitleRect = subtitleGo.AddComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0f, 1f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.anchoredPosition = new Vector2(0f, -20f);
        subtitleRect.sizeDelta = new Vector2(0f, 22f);
        TextMeshProUGUI subtitleTmp = subtitleGo.AddComponent<TextMeshProUGUI>();
        subtitleTmp.text = "EVENT";
        subtitleTmp.fontSize = 13;
        subtitleTmp.fontStyle = FontStyles.Bold;
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.color = new Color(1f, 1f, 1f, 0.5f);
        subtitleTmp.characterSpacing = 5f;

        // Title
        GameObject titleGo = new GameObject("Title");
        titleGo.transform.SetParent(panelGo.transform, false);
        RectTransform titleRect = titleGo.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.anchoredPosition = new Vector2(0f, -52f);
        titleRect.sizeDelta = new Vector2(0f, 38f);
        TextMeshProUGUI titleTmp = titleGo.AddComponent<TextMeshProUGUI>();
        titleTmp.text = "STAR STORM";
        titleTmp.fontSize = 28;
        titleTmp.fontStyle = FontStyles.Bold;
        titleTmp.alignment = TextAlignmentOptions.Center;
        titleTmp.color = Color.white;

        using (var so = new SerializedObject(ui))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("panel").objectReferenceValue = panelRect;
            so.FindProperty("titleText").objectReferenceValue = titleTmp;
            so.FindProperty("subtitleText").objectReferenceValue = subtitleTmp;
            so.FindProperty("accentLine").objectReferenceValue = accentImg;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(ui);
        Undo.RegisterCreatedObjectUndo(root, "Create EventAnnouncement UI");
    }
}
