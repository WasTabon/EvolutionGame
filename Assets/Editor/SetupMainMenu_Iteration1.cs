using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class SetupMainMenu_Iteration1
{
    [MenuItem("EvolutionGame/Setup Main Menu Scene (Iteration 1)")]
    static void Setup()
    {
        EnsureGameManager();
        EnsureCamera();
        EnsureCanvas();
        EnsureGlobalVolume();
        SetupLightingAndFog();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 1] Main Menu Scene setup complete!");
    }

    static void EnsureGameManager()
    {
        if (Object.FindObjectOfType<GameManager>() != null) return;
        GameObject go = new GameObject("GameManager");
        go.AddComponent<GameManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create GameManager");
    }

    static void EnsureCamera()
    {
        Camera cam = Object.FindObjectOfType<Camera>();
        GameObject camGo = cam != null ? cam.gameObject : new GameObject("Main Camera");
        camGo.tag = "MainCamera";

        Camera camComponent = camGo.GetComponent<Camera>() ?? camGo.AddComponent<Camera>();
        camComponent.clearFlags = CameraClearFlags.SolidColor;
        camComponent.backgroundColor = new Color(0.02f, 0.01f, 0.06f);
        camComponent.orthographic = false;
        camComponent.fieldOfView = 60f;
        camGo.transform.position = new Vector3(0f, 0f, -10f);
        camGo.transform.rotation = Quaternion.identity;

        if (cam == null)
            Undo.RegisterCreatedObjectUndo(camGo, "Create Camera");
    }

    static void EnsureCanvas()
    {
        Canvas existingCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
        {
            if (c.name == "MainMenuCanvas") { existingCanvas = c; break; }
        }

        GameObject canvasGo;
        if (existingCanvas != null)
        {
            canvasGo = existingCanvas.gameObject;
        }
        else
        {
            canvasGo = new GameObject("MainMenuCanvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(390f, 844f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 1f;

            canvasGo.AddComponent<GraphicRaycaster>();
            Undo.RegisterCreatedObjectUndo(canvasGo, "Create MainMenuCanvas");
        }

        if (canvasGo.transform.Find("MenuRoot") != null) return;

        // MenuRoot — holds MainMenuUI + CanvasGroup on the SAME object
        GameObject menuRoot = new GameObject("MenuRoot");
        menuRoot.transform.SetParent(canvasGo.transform, false);
        RectTransform rootRect = menuRoot.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CanvasGroup cg = menuRoot.AddComponent<CanvasGroup>();
        MainMenuUI menuUI = menuRoot.AddComponent<MainMenuUI>();

        using (var so = new SerializedObject(menuUI))
        {
            so.FindProperty("canvasGroup").objectReferenceValue = cg;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Title
        GameObject titleGo = CreateText(menuRoot.transform, "Title", "EVOLUTION", 64, FontStyle.Bold,
            new Vector2(0.5f, 0.65f), new Vector2(0.5f, 0.65f), Vector2.zero, new Vector2(360f, 90f));
        titleGo.GetComponent<Text>().color = Color.white;
        RectTransform titleRect = titleGo.GetComponent<RectTransform>();

        using (var so = new SerializedObject(menuUI))
        {
            so.FindProperty("titleTransform").objectReferenceValue = titleRect;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // Subtitle
        GameObject subGo = CreateText(menuRoot.transform, "Subtitle", "GAME", 26, FontStyle.Normal,
            new Vector2(0.5f, 0.55f), new Vector2(0.5f, 0.55f), Vector2.zero, new Vector2(360f, 40f));
        subGo.GetComponent<Text>().color = new Color(1f, 1f, 1f, 0.5f);

        // START button — large, centered in lower half (hypercasual)
        GameObject btnGo = new GameObject("StartButton");
        btnGo.transform.SetParent(menuRoot.transform, false);
        RectTransform btnRect = btnGo.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.1f, 0.18f);
        btnRect.anchorMax = new Vector2(0.9f, 0.28f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;

        Image btnImg = btnGo.AddComponent<Image>();
        btnImg.color = new Color(1f, 1f, 1f, 0.15f);

        Button btn = btnGo.AddComponent<Button>();
        ColorBlock cb = btn.colors;
        cb.normalColor = new Color(1f, 1f, 1f, 0.15f);
        cb.highlightedColor = new Color(1f, 1f, 1f, 0.25f);
        cb.pressedColor = new Color(1f, 1f, 1f, 0.08f);
        cb.fadeDuration = 0.1f;
        btn.colors = cb;

        GameObject btnLabel = CreateText(btnGo.transform, "Label", "TAP TO PLAY", 32, FontStyle.Bold,
            Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        btnLabel.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        btnLabel.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        btnLabel.GetComponent<Text>().color = Color.white;

        using (var so = new SerializedObject(menuUI))
        {
            so.FindProperty("startButton").objectReferenceValue = btn;
            so.FindProperty("startButtonTransform").objectReferenceValue = btnRect;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EnsureEventSystem();
        EditorUtility.SetDirty(menuUI);
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

        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame"))
            AssetDatabase.CreateFolder("Assets", "EvolutionGame");
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/VolumeProfiles"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "VolumeProfiles");

        string profilePath = "Assets/EvolutionGame/VolumeProfiles/MenuSceneProfile.asset";
        var profile = AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.VolumeProfile>(profilePath);
        if (profile == null)
        {
            profile = ScriptableObject.CreateInstance<UnityEngine.Rendering.VolumeProfile>();
            AssetDatabase.CreateAsset(profile, profilePath);
        }

        if (!profile.Has<Bloom>())
        {
            Bloom bloom = profile.Add<Bloom>(true);
            bloom.threshold.value = 0.7f;
            bloom.intensity.value = 2.2f;
            bloom.scatter.value = 0.75f;
            bloom.tint.value = new Color(0.6f, 0.5f, 1f);
        }

        if (!profile.Has<Vignette>())
        {
            Vignette vignette = profile.Add<Vignette>(true);
            vignette.intensity.value = 0.45f;
            vignette.smoothness.value = 0.45f;
            vignette.color.value = new Color(0.01f, 0f, 0.07f);
        }

        if (!profile.Has<ColorAdjustments>())
        {
            ColorAdjustments colorAdj = profile.Add<ColorAdjustments>(true);
            colorAdj.contrast.value = 10f;
            colorAdj.saturation.value = 25f;
            colorAdj.colorFilter.value = new Color(0.85f, 0.82f, 1f);
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
            light.color = new Color(0.6f, 0.5f, 1f);
            light.intensity = 0.4f;
            lightGo.transform.rotation = Quaternion.Euler(45f, -30f, 0f);
            Undo.RegisterCreatedObjectUndo(lightGo, "Create Directional Light");
        }

        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.03f, 0.01f, 0.08f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.02f, 0.01f, 0.08f);
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogStartDistance = 10f;
        RenderSettings.fogEndDistance = 30f;
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
}
