using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class SetupGameScene_Iteration1
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 1)")]
    static void Setup()
    {
        EnsureConfigs();
        EnsureGameManager();
        EnsureScoreManager();
        EnsureCamera();
        EnsurePlayer();
        EnsureSpawnManager();
        EnsureCanvas();
        EnsureGlobalVolume();
        SetupLightingAndFog();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 1] Game Scene setup complete!");
    }

    static void EnsureConfigs()
    {
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame"))
            AssetDatabase.CreateFolder("Assets", "EvolutionGame");
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Configs"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "Configs");

        CreateConfigIfMissing<PlayerConfig>("Assets/EvolutionGame/Configs/PlayerConfig.asset", cfg =>
        {
            cfg.baseSpeed = 5f;
            cfg.minSpeed = 1.5f;
            cfg.speedScalePenalty = 0.4f;
            cfg.inertiaSmoothing = 8f;
            cfg.baseScale = 0.5f;
        });

        CreateWorldObjectConfig("Small",  WorldObjectType.Small,  0.3f,  10f,  0.04f, 1.8f);
        CreateWorldObjectConfig("Medium", WorldObjectType.Medium, 0.75f, 35f,  0.12f, 1.1f);
        CreateWorldObjectConfig("Large",  WorldObjectType.Large,  1.6f,  120f, 0.35f, 0.5f);
    }

    static void CreateWorldObjectConfig(string name, WorldObjectType type, float scale, float points, float growth, float speed)
    {
        string path = "Assets/EvolutionGame/Configs/WorldObject_" + name + ".asset";
        if (AssetDatabase.LoadAssetAtPath<WorldObjectConfig>(path) != null) return;

        WorldObjectConfig cfg = ScriptableObject.CreateInstance<WorldObjectConfig>();
        cfg.type         = type;
        cfg.scale        = scale;
        cfg.points       = points;
        cfg.growthAmount = growth;
        cfg.moveSpeed    = speed;

        AssetDatabase.CreateAsset(cfg, path);
        AssetDatabase.SaveAssets();
    }

    static void CreateConfigIfMissing<T>(string path, System.Action<T> setup) where T : ScriptableObject
    {
        if (AssetDatabase.LoadAssetAtPath<T>(path) != null) return;
        T asset = ScriptableObject.CreateInstance<T>();
        setup(asset);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
    }

    static void EnsureGameManager()
    {
        if (Object.FindObjectOfType<GameManager>() != null) return;
        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create GameManager");
    }

    static void EnsureScoreManager()
    {
        if (Object.FindObjectOfType<ScoreManager>() != null) return;
        GameObject go = new GameObject("ScoreManager");
        go.AddComponent<ScoreManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create ScoreManager");
    }

    static void EnsureCamera()
    {
        Camera cam = Object.FindObjectOfType<Camera>();
        GameObject camGo = cam != null ? cam.gameObject : new GameObject("Main Camera");
        camGo.tag = "MainCamera";

        Camera camComponent = camGo.GetComponent<Camera>() ?? camGo.AddComponent<Camera>();
        camComponent.clearFlags = CameraClearFlags.SolidColor;
        camComponent.backgroundColor = new Color(0.02f, 0.01f, 0.06f);
        camComponent.fieldOfView = 60f;
        camComponent.farClipPlane = 200f;

        camGo.transform.position = new Vector3(0f, 22f, 0f);
        camGo.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        CameraFollow follow = camGo.GetComponent<CameraFollow>() ?? camGo.AddComponent<CameraFollow>();
        follow.offset = new Vector3(0f, 22f, 0f);
        follow.smoothSpeed = 6f;

        if (cam == null)
            Undo.RegisterCreatedObjectUndo(camGo, "Create Camera");
    }

    static void EnsurePlayer()
    {
        PlayerController existing = Object.FindObjectOfType<PlayerController>();
        if (existing != null)
        {
            WirePlayerReferences(existing.gameObject);
            return;
        }

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Player";
        player.transform.position = Vector3.zero;

        SphereCollider col = player.GetComponent<SphereCollider>();
        col.isTrigger = true;

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        PlayerConfig cfg = AssetDatabase.LoadAssetAtPath<PlayerConfig>("Assets/EvolutionGame/Configs/PlayerConfig.asset");
        PlayerController pc = player.AddComponent<PlayerController>();

        using (var so = new SerializedObject(pc))
        {
            so.FindProperty("config").objectReferenceValue = cfg;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        MeshRenderer mr = player.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(0.85f, 0.9f, 1f);
        mat.SetFloat("_Smoothness", 0.9f);
        mr.sharedMaterial = mat;

        WirePlayerReferences(player);
        Undo.RegisterCreatedObjectUndo(player, "Create Player");
    }

    static void WirePlayerReferences(GameObject player)
    {
        CameraFollow follow = Object.FindObjectOfType<CameraFollow>();
        if (follow != null && follow.target == null)
        {
            using (var so = new SerializedObject(follow))
            {
                so.FindProperty("target").objectReferenceValue = player.transform;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        SpawnManager sm = Object.FindObjectOfType<SpawnManager>();
        if (sm != null && sm.playerTransform == null)
        {
            using (var so = new SerializedObject(sm))
            {
                so.FindProperty("playerTransform").objectReferenceValue = player.transform;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }
    }

    static void EnsureSpawnManager()
    {
        SpawnManager existing = Object.FindObjectOfType<SpawnManager>();
        SpawnManager sm = existing;

        if (existing == null)
        {
            GameObject go = new GameObject("SpawnManager");
            sm = go.AddComponent<SpawnManager>();
            Undo.RegisterCreatedObjectUndo(go, "Create SpawnManager");
        }

        WorldObjectConfig small  = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>("Assets/EvolutionGame/Configs/WorldObject_Small.asset");
        WorldObjectConfig medium = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>("Assets/EvolutionGame/Configs/WorldObject_Medium.asset");
        WorldObjectConfig large  = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>("Assets/EvolutionGame/Configs/WorldObject_Large.asset");

        using (var so = new SerializedObject(sm))
        {
            SetConfigArray(so, "smallConfigs",  small);
            SetConfigArray(so, "mediumConfigs", medium);
            SetConfigArray(so, "largeConfigs",  large);

            PlayerController player = Object.FindObjectOfType<PlayerController>();
            if (player != null)
                so.FindProperty("playerTransform").objectReferenceValue = player.transform;

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(sm);
    }

    static void SetConfigArray(SerializedObject so, string propName, WorldObjectConfig cfg)
    {
        if (cfg == null) return;
        SerializedProperty arr = so.FindProperty(propName);
        arr.arraySize = 1;
        arr.GetArrayElementAtIndex(0).objectReferenceValue = cfg;
    }

    static void EnsureCanvas()
    {
        Canvas existingCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
        {
            if (c.name == "GameCanvas") { existingCanvas = c; break; }
        }

        GameObject canvasGo;
        if (existingCanvas != null)
        {
            canvasGo = existingCanvas.gameObject;
        }
        else
        {
            canvasGo = new GameObject("GameCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(390f, 844f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;

            canvasGo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasGo, "Create GameCanvas");
        }

        EnsureHUD(canvasGo);
        EnsureGameOverPanel(canvasGo);
        EnsureEventSystem();
    }

    static void EnsureHUD(GameObject canvas)
    {
        if (canvas.transform.Find("HUD") != null) return;

        GameObject hud = new GameObject("HUD");
        hud.transform.SetParent(canvas.transform, false);
        RectTransform hudRect = hud.AddComponent<RectTransform>();
        hudRect.anchorMin = Vector2.zero;
        hudRect.anchorMax = Vector2.one;
        hudRect.offsetMin = Vector2.zero;
        hudRect.offsetMax = Vector2.zero;

        CanvasGroup cg = hud.AddComponent<CanvasGroup>();
        GameHUD gameHud = hud.AddComponent<GameHUD>();

        GameObject scoreGo = CreateText(hud.transform, "ScoreText", "0", 60, FontStyle.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -70f), new Vector2(300f, 80f));
        scoreGo.GetComponent<Text>().color = Color.white;

        GameObject stageGo = CreateText(hud.transform, "StageText", "Spark", 20, FontStyle.Normal,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -148f), new Vector2(260f, 30f));
        stageGo.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.6f);

        using (var so = new SerializedObject(gameHud))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("scoreText").objectReferenceValue = scoreGo.GetComponent<Text>();
            so.FindProperty("stageText").objectReferenceValue = stageGo.GetComponent<Text>();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(gameHud);
    }

    static void EnsureGameOverPanel(GameObject canvas)
    {
        if (canvas.transform.Find("GameOverPanel") != null) return;

        GameObject panel = new GameObject("GameOverPanel");
        panel.transform.SetParent(canvas.transform, false);
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        CanvasGroup cg = panel.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        GameOverUI gameOverUI = panel.AddComponent<GameOverUI>();

        GameObject bg = new GameObject("Bg");
        bg.transform.SetParent(panel.transform, false);
        RectTransform bgRect = bg.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        Image bgImg = bg.AddComponent<Image>();
        bgImg.color = new Color(0f, 0f, 0f, 0.65f);

        GameObject card = new GameObject("Card");
        card.transform.SetParent(panel.transform, false);
        RectTransform cardRect = card.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.08f, 0.25f);
        cardRect.anchorMax = new Vector2(0.92f, 0.78f);
        cardRect.offsetMin = Vector2.zero;
        cardRect.offsetMax = Vector2.zero;
        Image cardImg = card.AddComponent<Image>();
        cardImg.color = new Color(0f, 0f, 0f, 0.5f);

        GameObject titleGo = CreateText(card.transform, "Title", "GAME OVER", 44, FontStyle.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -55f), new Vector2(300f, 60f));
        titleGo.GetComponent<Text>().color = Color.white;

        GameObject scoreLabelGo = CreateText(card.transform, "ScoreLabel", "SCORE", 18, FontStyle.Normal,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -130f), new Vector2(280f, 28f));
        scoreLabelGo.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.5f);

        GameObject scoreGo = CreateText(card.transform, "ScoreValue", "0", 52, FontStyle.Bold,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -175f), new Vector2(280f, 65f));
        scoreGo.GetComponent<Text>().color = Color.white;

        GameObject bestLabelGo = CreateText(card.transform, "BestLabel", "BEST", 18, FontStyle.Normal,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -250f), new Vector2(280f, 28f));
        bestLabelGo.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.5f);

        GameObject bestGo = CreateText(card.transform, "BestValue", "0", 36, FontStyle.Normal,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -285f), new Vector2(280f, 44f));
        bestGo.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.85f);

        GameObject restartBtn = CreateButton(panel.transform, "RestartButton", "PLAY AGAIN",
            new Vector2(0.08f, 0.12f), new Vector2(0.92f, 0.22f));

        GameObject menuBtn = CreateButton(panel.transform, "MenuButton", "MENU",
            new Vector2(0.25f, 0.05f), new Vector2(0.75f, 0.11f));
        menuBtn.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.08f);

        using (var so = new SerializedObject(gameOverUI))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.FindProperty("panel").objectReferenceValue = cardRect;
            so.FindProperty("scoreText").objectReferenceValue = scoreGo.GetComponent<Text>();
            so.FindProperty("bestScoreText").objectReferenceValue = bestGo.GetComponent<Text>();
            so.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
            so.FindProperty("menuButton").objectReferenceValue = menuBtn.GetComponent<Button>();
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(gameOverUI);
    }

    static void EnsureEventSystem()
    {
        if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<UnityEngine.EventSystems.EventSystem>();
        es.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
    }

    static void EnsureGlobalVolume()
    {
        if (Object.FindObjectOfType<UnityEngine.Rendering.Volume>() != null) return;

        GameObject volGo = new GameObject("GlobalVolume");
        var vol = volGo.AddComponent<UnityEngine.Rendering.Volume>();
        vol.isGlobal = true;
        vol.priority = 10;

        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/VolumeProfiles"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "VolumeProfiles");

        string profilePath = "Assets/EvolutionGame/VolumeProfiles/GameSceneProfile.asset";
        var profile = AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.VolumeProfile>(profilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<UnityEngine.Rendering.VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);
        }

        if (!profile.Has<Bloom>())
        {
            Bloom bloom = profile.Add<Bloom>(true);
            bloom.threshold.value = 0.8f;
            bloom.intensity.value = 1.8f;
            bloom.scatter.value = 0.7f;
            bloom.tint.value = new Color(0.7f, 0.6f, 1f);
        }

        if (!profile.Has<Vignette>())
        {
            Vignette vignette = profile.Add<Vignette>(true);
            vignette.intensity.value = 0.4f;
            vignette.smoothness.value = 0.5f;
            vignette.color.value = new Color(0.02f, 0f, 0.08f);
        }

        if (!profile.Has<ColorAdjustments>())
        {
            ColorAdjustments colorAdj = profile.Add<ColorAdjustments>(true);
            colorAdj.contrast.value = 15f;
            colorAdj.saturation.value = 20f;
            colorAdj.colorFilter.value = new Color(0.85f, 0.85f, 1f);
        }

        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
        vol.sharedProfile = profile;
        Undo.RegisterCreatedObjectUndo(volGo, "Create GlobalVolume");
    }

    static void SetupLightingAndFog()
    {
        Light existingLight = Object.FindObjectOfType<Light>();
        if (existingLight == null)
        {
            GameObject lightGo = new GameObject("Directional Light");
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = new Color(0.7f, 0.75f, 1f);
            light.intensity = 0.6f;
            lightGo.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(lightGo, "Create Directional Light");
        }
        else
        {
            existingLight.color = new Color(0.7f, 0.75f, 1f);
            existingLight.intensity = 0.6f;
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.04f, 0.02f, 0.1f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.03f, 0.01f, 0.1f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 15f;
        RenderSettings.fogEndDistance = 45f;
        RenderSettings.skybox = null;

        Camera mainCam = Object.FindObjectOfType<Camera>();
        if (mainCam != null)
        {
            mainCam.clearFlags = CameraClearFlags.SolidColor;
            mainCam.backgroundColor = new Color(0.02f, 0.01f, 0.06f);
        }
    }

    static GameObject CreateText(Transform parent, string name, string content, int fontSize, FontStyle style,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = sizeDelta;

        Text text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return go;
    }

    static GameObject CreateButton(Transform parent, string name, string label,
        Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        Image img = go.AddComponent<Image>();
        img.color = new Color(1f, 1f, 1f, 0.15f);

        Button btn = go.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(1f, 1f, 1f, 0.15f);
        cb.highlightedColor = new Color(1f, 1f, 1f, 0.25f);
        cb.pressedColor = new Color(1f, 1f, 1f, 0.08f);
        cb.fadeDuration = 0.08f;
        btn.colors = cb;

        GameObject textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        RectTransform textRect = textGo.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        Text text = textGo.AddComponent<Text>();
        text.text = label;
        text.fontSize = 26;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        return go;
    }
}
