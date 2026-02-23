using UnityEditor;
using UnityEngine;

public class SetupGameScene_Iteration11
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 11)")]
    static void Setup()
    {
        EnsureGravitySystem();
        FixPlayerTrailBaseWidth();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 11] Gravity System + Trail Polish setup complete!");
    }

    static void EnsureGravitySystem()
    {
        if (Object.FindObjectOfType<GravitySystem>() != null) return;
        GameObject go = new GameObject("GravitySystem");
        go.AddComponent<GravitySystem>();
        Undo.RegisterCreatedObjectUndo(go, "Create GravitySystem");
        Debug.Log("[Iteration 11] GravitySystem added.");
    }

    static void FixPlayerTrailBaseWidth()
    {
        PlayerTrail trail = Object.FindObjectOfType<PlayerTrail>();
        if (trail == null)
        {
            Debug.LogWarning("[Iteration 11] PlayerTrail not found. Run Iteration 3 setup first.");
            return;
        }

        using (var so = new SerializedObject(trail))
        {
            so.FindProperty("baseTrailWidth").floatValue = 0.6f;
            so.FindProperty("maxSpeed").floatValue = 5f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(trail);
        Debug.Log("[Iteration 11] PlayerTrail baseTrailWidth set.");
    }
}
