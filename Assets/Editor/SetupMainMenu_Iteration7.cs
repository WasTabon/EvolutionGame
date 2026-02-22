using UnityEditor;
using UnityEngine;

public class SetupMainMenu_Iteration7
{
    [MenuItem("EvolutionGame/Setup Main Menu Scene (Iteration 7)")]
    static void Setup()
    {
        EnsureAudioManager();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 7] Audio setup complete on Main Menu scene!");
    }

    static void EnsureAudioManager()
    {
        if (Object.FindObjectOfType<AudioManager>() != null) return;

        string configPath = "Assets/EvolutionGame/Configs/AudioConfig.asset";
        AudioConfig config = AssetDatabase.LoadAssetAtPath<AudioConfig>(configPath);

        if (config == null)
        {
            config = ScriptableObject.CreateInstance<AudioConfig>();
            if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Configs"))
                AssetDatabase.CreateFolder("Assets/EvolutionGame", "Configs");
            AssetDatabase.CreateAsset(config, configPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Iteration 7] AudioConfig.asset created at " + configPath + ". Assign AudioClips in the Inspector.");
        }

        GameObject go = new GameObject("AudioManager");
        AudioManager am = go.AddComponent<AudioManager>();

        using (var so = new SerializedObject(am))
        {
            so.FindProperty("config").objectReferenceValue = config;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(am);
        Undo.RegisterCreatedObjectUndo(go, "Create AudioManager");
    }
}
