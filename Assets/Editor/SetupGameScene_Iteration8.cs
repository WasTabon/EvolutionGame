using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SetupGameScene_Iteration8
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 8)")]
    static void Setup()
    {
        EnsureModelConfig();
        EnsureModelInitializer();
        TunePostProcessing();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 8] 3D Models & Visuals setup complete!");
    }

    static void EnsureModelConfig()
    {
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Configs"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "Configs");

        string path = "Assets/EvolutionGame/Configs/ModelConfig.asset";
        ModelConfig cfg = AssetDatabase.LoadAssetAtPath<ModelConfig>(path);
        if (cfg != null) return;

        cfg = ScriptableObject.CreateInstance<ModelConfig>();

        string[] stageNames = { "Spark", "Young Star", "Bright Star", "Supernova", "Galactic Core" };
        cfg.stageModels = new StageModel[stageNames.Length];
        for (int i = 0; i < stageNames.Length; i++)
            cfg.stageModels[i] = new StageModel { stageName = stageNames[i] };

        AssetDatabase.CreateAsset(cfg, path);
        AssetDatabase.SaveAssets();
        Debug.Log("[Iteration 8] ModelConfig.asset created at " + path + ". Assign Synty meshes and materials in the Inspector.");
    }

    static void EnsureModelInitializer()
    {
        if (Object.FindObjectOfType<ModelInitializer>() != null) return;

        GameObject go = new GameObject("ModelInitializer");
        ModelInitializer mi = go.AddComponent<ModelInitializer>();

        ModelConfig modelCfg = AssetDatabase.LoadAssetAtPath<ModelConfig>("Assets/EvolutionGame/Configs/ModelConfig.asset");
        WorldObjectConfig small  = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>("Assets/EvolutionGame/Configs/WorldObject_Small.asset");
        WorldObjectConfig medium = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>("Assets/EvolutionGame/Configs/WorldObject_Medium.asset");
        WorldObjectConfig large  = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>("Assets/EvolutionGame/Configs/WorldObject_Large.asset");
        EvolutionConfig evoConfig = AssetDatabase.LoadAssetAtPath<EvolutionConfig>("Assets/EvolutionGame/Configs/EvolutionConfig.asset");

        using (var so = new SerializedObject(mi))
        {
            so.FindProperty("modelConfig").objectReferenceValue = modelCfg;
            so.FindProperty("smallConfig").objectReferenceValue = small;
            so.FindProperty("mediumConfig").objectReferenceValue = medium;
            so.FindProperty("largeConfig").objectReferenceValue = large;
            so.FindProperty("evolutionConfig").objectReferenceValue = evoConfig;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(mi);
        Undo.RegisterCreatedObjectUndo(go, "Create ModelInitializer");
    }

    static void TunePostProcessing()
    {
        Volume vol = Object.FindObjectOfType<Volume>();
        if (vol == null || vol.sharedProfile == null)
        {
            Debug.LogWarning("[Iteration 8] GlobalVolume not found. Run Iteration 1 setup first.");
            return;
        }

        VolumeProfile profile = vol.sharedProfile;

        if (profile.TryGet<Bloom>(out Bloom bloom))
        {
            bloom.threshold.value = 0.6f;
            bloom.intensity.value = 2.8f;
            bloom.scatter.value = 0.75f;
            bloom.tint.value = new Color(0.75f, 0.65f, 1f);
        }

        if (profile.TryGet<Vignette>(out Vignette vignette))
        {
            vignette.intensity.value = 0.38f;
            vignette.smoothness.value = 0.55f;
        }

        if (profile.TryGet<ColorAdjustments>(out ColorAdjustments colorAdj))
        {
            colorAdj.contrast.value = 18f;
            colorAdj.saturation.value = 30f;
            colorAdj.colorFilter.value = new Color(0.88f, 0.85f, 1f);
        }

        EditorUtility.SetDirty(profile);
        AssetDatabase.SaveAssets();
        Debug.Log("[Iteration 8] Post-processing tuned for 3D models.");
    }
}
