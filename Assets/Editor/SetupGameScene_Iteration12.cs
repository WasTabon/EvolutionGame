using UnityEditor;
using UnityEngine;

public class SetupGameScene_Iteration12
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 12)")]
    static void Setup()
    {
        EnsureEnemySpawnManager();
        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 12] Enemy Spawn Manager added. 10 enemy types ready.");
    }

    static void EnsureEnemySpawnManager()
    {
        if (Object.FindObjectOfType<EnemySpawnManager>() != null)
        {
            Debug.Log("[Iteration 12] EnemySpawnManager already exists.");
            return;
        }

        GameObject go = new GameObject("EnemySpawnManager");
        go.AddComponent<EnemySpawnManager>();
        Undo.RegisterCreatedObjectUndo(go, "Create EnemySpawnManager");
        Debug.Log("[Iteration 12] EnemySpawnManager added.");
    }
}
