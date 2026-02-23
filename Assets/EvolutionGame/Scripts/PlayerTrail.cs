using UnityEngine;
using DG.Tweening;

public class PlayerTrail : MonoBehaviour
{
    public TrailRenderer trailRenderer;
    public float maxSpeed = 5f;
    public float baseTrailWidth = 0.6f;

    private PlayerController player;

    void Start()
    {
        Debug.Assert(trailRenderer != null, "PlayerTrail: trailRenderer not assigned!");
        player = GetComponentInParent<PlayerController>();

        if (EvolutionManager.Instance != null)
        {
            EvolutionManager.Instance.OnStageChanged -= OnStageChanged;
            EvolutionManager.Instance.OnStageChanged += OnStageChanged;
            SyncColor(EvolutionManager.Instance.GetCurrentStageIndex());
        }
    }

    void OnDestroy()
    {
        if (EvolutionManager.Instance != null)
            EvolutionManager.Instance.OnStageChanged -= OnStageChanged;
    }

    void OnStageChanged(int index, EvolutionStageData stage)
    {
        SyncColor(index);
    }

    void SyncColor(int index)
    {
        if (trailRenderer == null) return;
        if (EvolutionManager.Instance?.config == null) return;

        EvolutionConfig cfg = EvolutionManager.Instance.config;
        if (index >= cfg.stages.Length) return;

        Color c = cfg.stages[index].trailColor;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(c, 0f),
                new GradientColorKey(c, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.85f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trailRenderer.colorGradient = gradient;
    }

    void Update()
    {
        if (player == null || trailRenderer == null) return;

        float speed  = player.GetVelocityMagnitude();
        float speedT = Mathf.Clamp01(speed / maxSpeed);
        float scale  = player.GetCurrentScale();

        float targetWidth = baseTrailWidth * scale * Mathf.Lerp(0.05f, 1f, speedT);
        trailRenderer.widthMultiplier = Mathf.Lerp(trailRenderer.widthMultiplier, targetWidth, Time.deltaTime * 8f);
    }
}
