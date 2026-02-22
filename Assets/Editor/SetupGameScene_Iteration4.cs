using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetupGameScene_Iteration4
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 4)")]
    static void Setup()
    {
        EnsureEvolutionConfig();
        EnsureEvolutionManager();
        EnsureStageTransitionUI();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 4] Evolution system setup complete!");
    }

    static void EnsureEvolutionConfig()
    {
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Configs"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "Configs");
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Materials"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "Materials");

        string path = "Assets/EvolutionGame/Configs/EvolutionConfig.asset";
        EvolutionConfig cfg = AssetDatabase.LoadAssetAtPath<EvolutionConfig>(path);
        if (cfg == null)
        {
            cfg = ScriptableObject.CreateInstance<EvolutionConfig>();
            AssetDatabase.CreateAsset(cfg, path);
        }

        cfg.stages = new EvolutionStageData[]
        {
            CreateStage("Spark",        0.5f,  new Color(0.4f,  0.7f,  1.0f), 1.0f, 0.6f),
            CreateStage("Young Star",   1.2f,  new Color(0.4f,  0.9f,  1.0f), 1.3f, 0.8f),
            CreateStage("Bright Star",  2.5f,  new Color(1.0f,  0.95f, 0.4f), 1.6f, 1.0f),
            CreateStage("Supernova",    4.5f,  new Color(1.0f,  0.55f, 0.1f), 2.0f, 1.3f),
            CreateStage("Galactic Core",7.0f,  new Color(0.75f, 0.3f,  1.0f), 2.5f, 1.6f),
        };

        EditorUtility.SetDirty(cfg);
        AssetDatabase.SaveAssets();
    }

    static EvolutionStageData CreateStage(string name, float threshold, Color color, float emission, float trailWidth)
    {
        string matPath = "Assets/EvolutionGame/Materials/Player_" + name.Replace(" ", "") + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat, matPath);
        }

        mat.color = color;
        mat.SetFloat("_Smoothness", 0.9f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color * emission);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        EditorUtility.SetDirty(mat);

        return new EvolutionStageData
        {
            stageName       = name,
            scaleThreshold  = threshold,
            playerMaterial  = mat,
            trailColor      = color,
            trailWidth      = trailWidth,
            emissionIntensity = emission
        };
    }

    static void EnsureEvolutionManager()
    {
        EvolutionManager existing = Object.FindObjectOfType<EvolutionManager>();
        EvolutionManager em = existing;

        if (existing == null)
        {
            GameObject go = new GameObject("EvolutionManager");
            em = go.AddComponent<EvolutionManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create EvolutionManager");
        }

        EvolutionConfig cfg = AssetDatabase.LoadAssetAtPath<EvolutionConfig>("Assets/EvolutionGame/Configs/EvolutionConfig.asset");
        using (var so = new SerializedObject(em))
        {
            so.FindProperty("config").objectReferenceValue = cfg;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(em);
    }

    static void EnsureStageTransitionUI()
    {
        Canvas canvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") { canvas = c; break; }

        if (canvas == null)
        {
            Debug.LogWarning("[Iteration 4] GameCanvas not found! Run Iteration 1 setup first.");
            return;
        }

        if (canvas.transform.Find("StageTransition") != null) return;

        GameObject root = new GameObject("StageTransition");
        root.transform.SetParent(canvas.transform, false);
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        EvolutionStageTransition transition = root.AddComponent<EvolutionStageTransition>();

        // Flash overlay — full screen white flash
        GameObject flashGo = new GameObject("FlashOverlay");
        flashGo.transform.SetParent(root.transform, false);
        RectTransform flashRect = flashGo.AddComponent<RectTransform>();
        flashRect.anchorMin = Vector2.zero;
        flashRect.anchorMax = Vector2.one;
        flashRect.offsetMin = Vector2.zero;
        flashRect.offsetMax = Vector2.zero;
        Image flashImg = flashGo.AddComponent<Image>();
        flashImg.color = Color.white;
        flashImg.raycastTarget = false;
        CanvasGroup flashCG = flashGo.AddComponent<CanvasGroup>();
        flashCG.alpha = 0f;
        flashCG.blocksRaycasts = false;
        flashCG.interactable = false;

        // Text group — centered
        GameObject textGroupGo = new GameObject("TextGroup");
        textGroupGo.transform.SetParent(root.transform, false);
        RectTransform textGroupRect = textGroupGo.AddComponent<RectTransform>();
        textGroupRect.anchorMin = new Vector2(0.1f, 0.5f);
        textGroupRect.anchorMax = new Vector2(0.9f, 0.5f);
        textGroupRect.anchoredPosition = Vector2.zero;
        textGroupRect.sizeDelta = new Vector2(0f, 120f);
        CanvasGroup textCG = textGroupGo.AddComponent<CanvasGroup>();
        textCG.alpha = 0f;
        textCG.blocksRaycasts = false;
        textCG.interactable = false;

        // Subtitle — EVOLUTION label above main text
        GameObject subtitleGo = new GameObject("Subtitle");
        subtitleGo.transform.SetParent(textGroupGo.transform, false);
        RectTransform subtitleRect = subtitleGo.AddComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0f, 1f);
        subtitleRect.anchorMax = new Vector2(1f, 1f);
        subtitleRect.anchoredPosition = new Vector2(0f, -18f);
        subtitleRect.sizeDelta = new Vector2(0f, 28f);
        TextMeshProUGUI subtitleTmp = subtitleGo.AddComponent<TextMeshProUGUI>();
        subtitleTmp.text = "EVOLUTION";
        subtitleTmp.fontSize = 16;
        subtitleTmp.fontStyle = FontStyles.Bold;
        subtitleTmp.alignment = TextAlignmentOptions.Center;
        subtitleTmp.color = new Color(1f, 1f, 1f, 0.6f);
        subtitleTmp.characterSpacing = 6f;

        // Stage name — large
        GameObject nameGo = new GameObject("StageName");
        nameGo.transform.SetParent(textGroupGo.transform, false);
        RectTransform nameRect = nameGo.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0f, 1f);
        nameRect.anchorMax = new Vector2(1f, 1f);
        nameRect.anchoredPosition = new Vector2(0f, -72f);
        nameRect.sizeDelta = new Vector2(0f, 60f);
        TextMeshProUGUI nameTmp = nameGo.AddComponent<TextMeshProUGUI>();
        nameTmp.text = "SPARK";
        nameTmp.fontSize = 46;
        nameTmp.fontStyle = FontStyles.Bold;
        nameTmp.alignment = TextAlignmentOptions.Center;
        nameTmp.color = Color.white;

        using (var so = new SerializedObject(transition))
        {
            so.FindProperty("flashOverlay").objectReferenceValue = flashCG;
            so.FindProperty("textGroup").objectReferenceValue = textCG;
            so.FindProperty("stageNameText").objectReferenceValue = nameTmp;
            so.FindProperty("stageSubtitleText").objectReferenceValue = subtitleTmp;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(transition);
        Undo.RegisterCreatedObjectUndo(root, "Create StageTransition UI");
    }
}
