using UnityEngine;

public class PlayerTrail : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public float maxSpeed = 5f;

    private PlayerController player;

    void Start()
    {
        Debug.Assert(trailRenderer != null, "PlayerTrail: trailRenderer not assigned!");
        player = GetComponentInParent<PlayerController>();
    }

    void Update()
    {
        if (player == null || trailRenderer == null) return;

        float speed = player.GetVelocityMagnitude();
        float t = Mathf.Clamp01(speed / maxSpeed);
        trailRenderer.widthMultiplier = Mathf.Lerp(0.05f, 1f, t);
    }
}
