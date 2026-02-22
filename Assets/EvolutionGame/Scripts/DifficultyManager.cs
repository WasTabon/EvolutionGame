using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    [System.Serializable]
    public class DifficultyLevel
    {
        public float spawnInterval = 0.8f;
        public int maxObjects = 35;
        public float smallRatio = 0.60f;
        public float mediumRatio = 0.28f;
        public float speedMultiplier = 1f;
    }

    public DifficultyLevel[] levels = new DifficultyLevel[]
    {
        new DifficultyLevel { spawnInterval = 0.8f, maxObjects = 30, smallRatio = 0.65f, mediumRatio = 0.28f, speedMultiplier = 1.0f },
        new DifficultyLevel { spawnInterval = 0.7f, maxObjects = 35, smallRatio = 0.55f, mediumRatio = 0.32f, speedMultiplier = 1.15f },
        new DifficultyLevel { spawnInterval = 0.6f, maxObjects = 40, smallRatio = 0.45f, mediumRatio = 0.35f, speedMultiplier = 1.3f },
        new DifficultyLevel { spawnInterval = 0.5f, maxObjects = 45, smallRatio = 0.35f, mediumRatio = 0.38f, speedMultiplier = 1.5f },
        new DifficultyLevel { spawnInterval = 0.4f, maxObjects = 50, smallRatio = 0.28f, mediumRatio = 0.40f, speedMultiplier = 1.7f },
    };

    private int currentLevel;

    public DifficultyLevel Current => levels[Mathf.Clamp(currentLevel, 0, levels.Length - 1)];

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
        if (EvolutionManager.Instance != null)
        {
            EvolutionManager.Instance.OnStageChanged -= OnStageChanged;
            EvolutionManager.Instance.OnStageChanged += OnStageChanged;
        }
    }

    void OnDisable()
    {
        if (EvolutionManager.Instance != null)
            EvolutionManager.Instance.OnStageChanged -= OnStageChanged;
    }

    void OnStageChanged(int stageIndex, EvolutionStageData stage)
    {
        currentLevel = Mathf.Clamp(stageIndex, 0, levels.Length - 1);
        if (SpawnManager.Instance != null)
            SpawnManager.Instance.ApplyDifficulty(Current);
    }
}
