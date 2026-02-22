using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;

    private Dictionary<WorldObjectConfig, Queue<GameObject>> pools = new Dictionary<WorldObjectConfig, Queue<GameObject>>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public GameObject Get(WorldObjectConfig config)
    {
        if (!pools.ContainsKey(config))
            pools[config] = new Queue<GameObject>();

        Queue<GameObject> pool = pools[config];

        GameObject go = null;
        while (pool.Count > 0)
        {
            go = pool.Dequeue();
            if (go != null) break;
            go = null;
        }

        if (go == null)
            go = CreateNew(config);

        go.SetActive(true);
        return go;
    }

    public void Return(GameObject go, WorldObjectConfig config)
    {
        if (go == null) return;

        go.SetActive(false);
        go.transform.position = Vector3.zero;

        if (!pools.ContainsKey(config))
            pools[config] = new Queue<GameObject>();

        pools[config].Enqueue(go);
    }

    GameObject CreateNew(WorldObjectConfig config)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = "WorldObject_" + config.type;

        SphereCollider col = go.GetComponent<SphereCollider>();
        col.isTrigger = false;

        Rigidbody rb = go.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        go.AddComponent<WorldObject>();
        go.SetActive(false);
        return go;
    }
}
