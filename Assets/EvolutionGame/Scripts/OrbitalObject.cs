using UnityEngine;

public class OrbitalObject : MonoBehaviour
{
    private Transform center;
    private float orbitRadius;
    private float currentAngle;
    private float angularSpeed;
    private bool isOrbiting;

    public void SetOrbit(Transform orbitCenter, float radius, float startAngle, float speed)
    {
        center = orbitCenter;
        orbitRadius = radius;
        currentAngle = startAngle;
        angularSpeed = speed;
        isOrbiting = true;
    }

    public void StopOrbit()
    {
        isOrbiting = false;
    }

    void Update()
    {
        if (!isOrbiting || center == null) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;

        currentAngle += angularSpeed * Time.deltaTime;
        float rad = currentAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius;
        transform.position = center.position + offset;
    }
}
