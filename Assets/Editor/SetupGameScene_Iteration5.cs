using UnityEditor;
using UnityEngine;

public class SetupGameScene_Iteration5
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 5)")]
    static void Setup()
    {
        EnsureObjectPool();
        EnsureDifficultyManager();
        EnsureParallaxStarfield();
        DisableUIStarfield();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 5] World dynamics setup complete!");
    }

    static void EnsureObjectPool()
    {
        if (Object.FindObjectOfType<ObjectPool>() != null) return;
        GameObject go = new GameObject("ObjectPool");
        go.AddComponent<ObjectPool>();
        Undo.RegisterCreatedObjectUndo(go, "Create ObjectPool");
    }

    static void EnsureDifficultyManager()
    {
        if (Object.FindObjectOfType<DifficultyManager>() != null) return;
        GameObject go = new GameObject("DifficultyManager");
        go.AddComponent<DifficultyManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create DifficultyManager");
    }

    static void EnsureParallaxStarfield()
    {
        if (Object.FindObjectOfType<ParallaxStarfield>() != null) return;

        GameObject go = new GameObject("ParallaxStarfield");
        go.transform.position = Vector3.zero;
        go.AddComponent<ParallaxStarfield>();
        Undo.RegisterCreatedObjectUndo(go, "Create ParallaxStarfield");
    }

    static void DisableUIStarfield()
    {
        Canvas canvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") { canvas = c; break; }

        if (canvas == null) return;

        Transform sf = canvas.transform.Find("Starfield");
        if (sf != null && sf.gameObject.activeSelf)
        {
            sf.gameObject.SetActive(false);
            EditorUtility.SetDirty(sf.gameObject);
            Debug.Log("[Iteration 5] UI Starfield disabled — replaced by ParallaxStarfield in world space.");
        }
    }
}
