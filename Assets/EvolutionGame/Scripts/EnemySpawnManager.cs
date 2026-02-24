using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemySpawnManager : MonoBehaviour
{
    public static EnemySpawnManager Instance;

    [System.Serializable]
    public class EnemyEntry
    {
        public string typeName;
        public int minStage = 0;
        public float spawnWeight = 1f;
    }

    public EnemyEntry[] enemyTable = new EnemyEntry[]
    {
        new EnemyEntry { typeName = "SunEnemy",         minStage = 0, spawnWeight = 2f  },
        new EnemyEntry { typeName = "CometEnemy",       minStage = 0, spawnWeight = 3f  },
        new EnemyEntry { typeName = "PulsarEnemy",      minStage = 1, spawnWeight = 2f  },
        new EnemyEntry { typeName = "RedGiantEnemy",    minStage = 1, spawnWeight = 1.5f},
        new EnemyEntry { typeName = "MagnetarEnemy",    minStage = 2, spawnWeight = 1.5f},
        new EnemyEntry { typeName = "DarkMatterEnemy",  minStage = 2, spawnWeight = 1f  },
        new EnemyEntry { typeName = "NeutronStarEnemy", minStage = 2, spawnWeight = 1f  },
        new EnemyEntry { typeName = "BinaryPairEnemy",  minStage = 3, spawnWeight = 1f  },
        new EnemyEntry { typeName = "SupernovaEnemy",   minStage = 3, spawnWeight = 0.7f},
        new EnemyEntry { typeName = "BlackHoleEnemy",   minStage = 4, spawnWeight = 0.5f},
    };

    public float spawnInterval = 18f;
    public int maxEnemies = 4;
    public float spawnRadius = 22f;

    private float timer;
    private int currentStage;
    private List<GameObject> activeEnemies = new List<GameObject>();

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

    void Start()
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

    void OnStageChanged(int idx, EvolutionStageData stage) => currentStage = idx;

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;

        activeEnemies.RemoveAll(e => e == null);

        timer += Time.deltaTime;
        if (timer >= spawnInterval && activeEnemies.Count < maxEnemies)
        {
            timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        Type enemyType = GetRandomEnemyType();
        if (enemyType == null) return;

        PlayerController pc = UnityEngine.Object.FindObjectOfType<PlayerController>();
        if (pc == null) return;

        Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
        Vector3 pos = pc.transform.position + new Vector3(dir.x, 0f, dir.y) * spawnRadius;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = enemyType.Name;
        go.transform.position = pos;

        Collider col = go.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        go.AddComponent(enemyType);

        activeEnemies.Add(go);
    }

    static Type FindType(string typeName)
    {
        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type t = assembly.GetType(typeName);
            if (t != null) return t;
        }
        return null;
    }

    Type GetRandomEnemyType()
    {
        int stage = EvolutionManager.Instance != null
            ? EvolutionManager.Instance.GetCurrentStageIndex()
            : currentStage;

        List<(Type type, float weight)> candidates = new List<(Type, float)>();
        float totalWeight = 0f;

        foreach (EnemyEntry entry in enemyTable)
        {
            if (entry.minStage > stage) continue;
            Type t = FindType(entry.typeName);
            if (t == null) continue;
            candidates.Add((t, entry.spawnWeight));
            totalWeight += entry.spawnWeight;
        }

        if (candidates.Count == 0) return null;

        float rand = UnityEngine.Random.Range(0f, totalWeight);
        float acc = 0f;
        foreach (var (type, weight) in candidates)
        {
            acc += weight;
            if (rand <= acc) return type;
        }
        return candidates[candidates.Count - 1].type;
    }
}