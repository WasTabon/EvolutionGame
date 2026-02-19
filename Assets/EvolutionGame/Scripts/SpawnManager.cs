using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public WorldObjectConfig[] smallConfigs;
    public WorldObjectConfig[] mediumConfigs;
    public WorldObjectConfig[] largeConfigs;

    public Transform playerTransform;
    public float spawnRadius = 18f;
    public float despawnRadius = 28f;
    public float spawnInterval = 0.8f;
    public int maxObjects = 35;

    private List<WorldObject> activeObjects = new List<WorldObject>();
    private float timer;

    void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.CurrentState != GameState.Playing) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            if (activeObjects.Count < maxObjects)
                SpawnObject();
        }

        CleanupObjects();
    }

    void SpawnObject()
    {
        WorldObjectConfig cfg = GetRandomConfig();
        if (cfg == null) return;

        Vector2 randDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = playerTransform.position + new Vector3(randDir.x, 0f, randDir.y) * spawnRadius;

        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "WorldObject_" + cfg.type;
        go.transform.position = spawnPos;

        SphereCollider col = go.GetComponent<SphereCollider>();
        col.isTrigger = false;

        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        WorldObject wo = go.AddComponent<WorldObject>();
        wo.Initialize(cfg);

        activeObjects.Add(wo);
    }

    WorldObjectConfig GetRandomConfig()
    {
        float rand = Random.value;

        if (rand < 0.6f)
        {
            if (smallConfigs == null || smallConfigs.Length == 0)
            {
                Debug.LogWarning("SpawnManager: smallConfigs is empty!");
                return null;
            }
            return smallConfigs[Random.Range(0, smallConfigs.Length)];
        }
        if (rand < 0.88f)
        {
            if (mediumConfigs == null || mediumConfigs.Length == 0)
            {
                Debug.LogWarning("SpawnManager: mediumConfigs is empty!");
                return null;
            }
            return mediumConfigs[Random.Range(0, mediumConfigs.Length)];
        }

        if (largeConfigs == null || largeConfigs.Length == 0)
        {
            Debug.LogWarning("SpawnManager: largeConfigs is empty!");
            return null;
        }
        return largeConfigs[Random.Range(0, largeConfigs.Length)];
    }

    void CleanupObjects()
    {
        for (int i = activeObjects.Count - 1; i >= 0; i--)
        {
            if (activeObjects[i] == null)
            {
                activeObjects.RemoveAt(i);
                continue;
            }

            float dist = Vector3.Distance(activeObjects[i].transform.position, playerTransform.position);
            if (dist > despawnRadius)
            {
                Destroy(activeObjects[i].gameObject);
                activeObjects.RemoveAt(i);
            }
        }
    }
}
