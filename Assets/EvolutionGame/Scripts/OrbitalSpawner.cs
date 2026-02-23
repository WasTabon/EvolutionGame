using UnityEngine;
using System.Collections.Generic;

public class OrbitalSpawner : MonoBehaviour
{
    public int smallCount = 4;
    public int mediumCount = 2;
    public float minOrbitRadius = 3f;
    public float maxOrbitRadius = 9f;

    private List<OrbitalObject> orbiters = new List<OrbitalObject>();

    public void SpawnOrbiters(WorldObjectConfig[] smallConfigs, WorldObjectConfig[] mediumConfigs)
    {
        SpawnGroup(smallConfigs, smallCount);
        SpawnGroup(mediumConfigs, mediumCount);
    }

    void SpawnGroup(WorldObjectConfig[] configs, int count)
    {
        if (configs == null || configs.Length == 0) return;

        for (int i = 0; i < count; i++)
        {
            WorldObjectConfig cfg = configs[Random.Range(0, configs.Length)];

            GameObject go;
            if (ObjectPool.Instance != null)
                go = ObjectPool.Instance.Get(cfg);
            else
            {
                go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.AddComponent<WorldObject>();
            }

            float orbitRadius = Random.Range(minOrbitRadius, maxOrbitRadius);
            float angle = Random.Range(0f, 360f);
            Vector3 offset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0f, Mathf.Sin(angle * Mathf.Deg2Rad)) * orbitRadius;
            go.transform.position = transform.position + offset;

            WorldObject wo = go.GetComponent<WorldObject>();
            wo.Initialize(cfg);

            float angularSpeed = Random.Range(20f, 60f) * (Random.value > 0.5f ? 1f : -1f);
            OrbitalObject orbital = go.GetComponent<OrbitalObject>();
            if (orbital == null) orbital = go.AddComponent<OrbitalObject>();
            orbital.SetOrbit(transform, orbitRadius, angle, angularSpeed);

            orbiters.Add(orbital);
        }
    }

    void OnDisable()
    {
        foreach (OrbitalObject o in orbiters)
        {
            if (o == null) continue;
            WorldObject wo = o.GetComponent<WorldObject>();
            WorldObjectConfig cfg = wo?.GetConfig();
            if (ObjectPool.Instance != null && cfg != null)
                ObjectPool.Instance.Return(o.gameObject, cfg);
            else if (o.gameObject != null)
                Destroy(o.gameObject);
        }
        orbiters.Clear();
    }
}
