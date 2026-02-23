using UnityEditor;
using UnityEngine;

public class SetupMainMenu_Iteration10
{
    [MenuItem("EvolutionGame/Final Checklist (Iteration 10)")]
    static void RunChecklist()
    {
        bool allGood = true;

        allGood &= Check(Object.FindObjectOfType<AudioManager>()     != null, "AudioManager on scene");
        allGood &= Check(Object.FindObjectOfType<GameManager>()      != null, "GameManager on scene");

        allGood &= CheckAsset("Assets/EvolutionGame/Configs/AudioConfig.asset",       "AudioConfig.asset");
        allGood &= CheckAsset("Assets/EvolutionGame/Configs/GameBalanceConfig.asset", "GameBalanceConfig.asset");
        allGood &= CheckAsset("Assets/EvolutionGame/Configs/EvolutionConfig.asset",   "EvolutionConfig.asset");
        allGood &= CheckAsset("Assets/EvolutionGame/Configs/ModelConfig.asset",       "ModelConfig.asset");

        Canvas menuCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "MainMenuCanvas") { menuCanvas = c; break; }
        allGood &= Check(menuCanvas != null, "MainMenuCanvas on scene");

        AudioManager am = Object.FindObjectOfType<AudioManager>();
        if (am != null)
            allGood &= Check(am.config != null, "AudioManager.config assigned");

        if (allGood)
            Debug.Log("[Iteration 10] ✓ Main Menu checklist PASSED. All systems present.");
        else
            Debug.LogWarning("[Iteration 10] Main Menu checklist has issues. Check messages above.");
    }

    [MenuItem("EvolutionGame/Final Checklist Game Scene (Iteration 10)")]
    static void RunGameChecklist()
    {
        bool allGood = true;

        allGood &= Check(Object.FindObjectOfType<SpawnManager>()      != null, "SpawnManager");
        allGood &= Check(Object.FindObjectOfType<EvolutionManager>()  != null, "EvolutionManager");
        allGood &= Check(Object.FindObjectOfType<DifficultyManager>() != null, "DifficultyManager");
        allGood &= Check(Object.FindObjectOfType<GameEventManager>()  != null, "GameEventManager");
        allGood &= Check(Object.FindObjectOfType<ComboSystem>()       != null, "ComboSystem");
        allGood &= Check(Object.FindObjectOfType<SessionTimer>()      != null, "SessionTimer");
        allGood &= Check(Object.FindObjectOfType<ObjectPool>()        != null, "ObjectPool");
        allGood &= Check(Object.FindObjectOfType<CameraShake>()       != null, "CameraShake");
        allGood &= Check(Object.FindObjectOfType<AbsorptionEffect>()  != null, "AbsorptionEffect");
        allGood &= Check(Object.FindObjectOfType<ParallaxStarfield>() != null, "ParallaxStarfield");

        SpawnManager sm = Object.FindObjectOfType<SpawnManager>();
        if (sm != null)
        {
            allGood &= Check(sm.playerTransform != null,     "SpawnManager.playerTransform assigned");
            allGood &= Check(sm.smallConfigs  != null && sm.smallConfigs.Length  > 0, "SpawnManager.smallConfigs assigned");
            allGood &= Check(sm.mediumConfigs != null && sm.mediumConfigs.Length > 0, "SpawnManager.mediumConfigs assigned");
            allGood &= Check(sm.largeConfigs  != null && sm.largeConfigs.Length  > 0, "SpawnManager.largeConfigs assigned");
        }

        EvolutionManager em = Object.FindObjectOfType<EvolutionManager>();
        if (em != null)
            allGood &= Check(em.config != null, "EvolutionManager.config assigned");

        Canvas gameCanvas = null;
        foreach (Canvas c in Object.FindObjectsOfType<Canvas>())
            if (c.name == "GameCanvas") { gameCanvas = c; break; }
        allGood &= Check(gameCanvas != null, "GameCanvas on scene");

        if (gameCanvas != null)
        {
            allGood &= Check(gameCanvas.transform.Find("HUD")              != null, "HUD in GameCanvas");
            allGood &= Check(gameCanvas.transform.Find("GameOverPanel")    != null, "GameOverPanel in GameCanvas");
            allGood &= Check(gameCanvas.transform.Find("StageTransition")  != null, "StageTransition in GameCanvas");
            allGood &= Check(gameCanvas.transform.Find("EventAnnouncement") != null, "EventAnnouncement in GameCanvas");
            allGood &= Check(gameCanvas.transform.Find("ScorePopupPool")   != null, "ScorePopupPool in GameCanvas");
        }

        if (allGood)
            Debug.Log("[Iteration 10] ✓ Game Scene checklist PASSED. All systems present.");
        else
            Debug.LogWarning("[Iteration 10] Game Scene checklist has issues. Check messages above.");
    }

    static bool Check(bool condition, string label)
    {
        if (condition)
            Debug.Log("  ✓ " + label);
        else
            Debug.LogError("  ✗ MISSING: " + label);
        return condition;
    }

    static bool CheckAsset(string path, string label)
    {
        bool exists = UnityEditor.AssetDatabase.LoadAssetAtPath<Object>(path) != null;
        return Check(exists, label + " at " + path);
    }
}
