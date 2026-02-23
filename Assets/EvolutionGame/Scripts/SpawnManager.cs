using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    public WorldObjectConfig[] smallConfigs;
    public WorldObjectConfig[] mediumConfigs;
    public WorldObjectConfig[] largeConfigs;

    public Transform playerTransform;
    public float spawnRadius = 18f;
    public float despawnRadius = 28f;
    public float spawnInterval = 0.8f;
    public int maxObjects = 35;

    private float smallRatio = 0.60f;
    private float mediumRatio = 0.28f;

    private List<WorldObject> activeObjects = new List<WorldObject>();
    private float timer;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public GameBalanceConfig balanceConfig;

    void Start()
    {
        if (balanceConfig != null)
        {
            spawnRadius   = balanceConfig.spawnRadius;
            despawnRadius = balanceConfig.despawnRadius;
            spawnInterval = balanceConfig.baseSpawnInterval;
            maxObjects    = balanceConfig.baseMaxObjects;
        }
    }

    private float spawnMultiplier = 1f;

    public void SetSpawnMultiplier(float mult)
    {
        spawnMultiplier = mult;
    }

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        timer += Time.deltaTime;
        float effectiveInterval = spawnInterval / spawnMultiplier;
        if (timer >= effectiveInterval)
        {
            timer = 0f;
            int spawnCount = Mathf.RoundToInt(spawnMultiplier);
            for (int i = 0; i < spawnCount; i++)
            {
                if (activeObjects.Count < maxObjects * spawnMultiplier)
                    SpawnObject();
            }
        }

        CleanupObjects();
    }

    public void ApplyDifficulty(DifficultyManager.DifficultyLevel level)
    {
        spawnInterval = level.spawnInterval;
        maxObjects    = level.maxObjects;
        smallRatio    = level.smallRatio;
        mediumRatio   = level.mediumRatio;
    }

    void SpawnObject()
    {
        WorldObjectConfig cfg = GetRandomConfig();
        if (cfg == null) return;

        Vector2 randDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerTransform.position + new Vector3(randDir.x, 0f, randDir.y) * spawnRadius;

        GameObject go;
        if (ObjectPool.Instance != null)
            go = ObjectPool.Instance.Get(cfg);
        else
        {
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            SphereCollider col = go.GetComponent<SphereCollider>();
            col.isTrigger = false;
            Rigidbody rb = go.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
            go.AddComponent<WorldObject>();
        }

        go.transform.position = spawnPos;

        WorldObject wo = go.GetComponent<WorldObject>();
        wo.Initialize(cfg);

        Collider goCol = go.GetComponent<Collider>();
        if (goCol != null) goCol.isTrigger = false;

        activeObjects.Add(wo);
    }

    WorldObjectConfig GetRandomConfig()
    {
        float rand = Random.value;
        float largeRatio = 1f - smallRatio - mediumRatio;

        if (rand < smallRatio)
        {
            if (smallConfigs == null || smallConfigs.Length == 0) { Debug.LogWarning("SpawnManager: smallConfigs is empty!"); return null; }
            return smallConfigs[Random.Range(0, smallConfigs.Length)];
        }
        if (rand < smallRatio + mediumRatio)
        {
            if (mediumConfigs == null || mediumConfigs.Length == 0) { Debug.LogWarning("SpawnManager: mediumConfigs is empty!"); return null; }
            return mediumConfigs[Random.Range(0, mediumConfigs.Length)];
        }

        if (largeConfigs == null || largeConfigs.Length == 0) { Debug.LogWarning("SpawnManager: largeConfigs is empty!"); return null; }
        return largeConfigs[Random.Range(0, largeConfigs.Length)];
    }

    void CleanupObjects()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null) { activeObjects.RemoveAt(i); continue; }
            if (!activeObjects[i].gameObject.activeSelf) { activeObjects.RemoveAt(i); continue; }

            float dist = Vector3.Distance(activeObjects[i].transform.position, playerTransform.position);
            if (dist > despawnRadius)
            {
                WorldObjectConfig cfg = activeObjects[i].GetConfig();
                if (ObjectPool.Instance != null && cfg != null)
                    ObjectPool.Instance.Return(activeObjects[i].gameObject, cfg);
                else
                    Destroy(activeObjects[i].gameObject);
                activeObjects.RemoveAt(i);
            }
        }
    }
}
