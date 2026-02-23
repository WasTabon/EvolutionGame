using UnityEngine;
using System.Collections.Generic;

public class GravitySystem : MonoBehaviour
{
    public static GravitySystem Instance;

    public float gravitationalConstant = 4f;

    private List<GravitationalBody> bodies = new List<GravitationalBody>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(GravitationalBody body)
    {
        if (!bodies.Contains(body)) bodies.Add(body);
    }

    public void Unregister(GravitationalBody body)
    {
        bodies.Remove(body);
    }

    public Vector3 GetForceAt(Vector3 position, float receiverMass = 1f)
    {
        Vector3 total = Vector3.zero;
        foreach (GravitationalBody body in bodies)
        {
            if (body == null || !body.gameObject.activeSelf) continue;

            Vector3 dir = body.transform.position - position;
            float dist = dir.magnitude;
            if (dist < 0.1f || dist > body.radius) continue;

            float falloff = 1f - Mathf.Clamp01(dist / body.radius);
            float forceMag = gravitationalConstant * body.mass * receiverMass * falloff / (dist + 0.5f);

            Vector3 force = dir.normalized * forceMag;
            if (body.gravityType == GravityType.Repel) force = -force;
            total += force;
        }
        return total;
    }
}
