using UnityEngine;
using DG.Tweening;

public class NeutronStarEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private Tweener glowTween;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(0.4f, 0.7f);
        isLethalOnContact = true;
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(0.85f, 0.95f, 1f), 4f);
        AnimateIntro();
        SetupGravity();
        StartGlow();
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 20f;
        gravBody.radius = scale * 12f;
        gravBody.gravityType = GravityType.Attract;
    }

    void StartGlow()
    {
        if (mat == null) return;
        glowTween = DOTween.To(
            () => 0f, t =>
            {
                if (mat == null) return;
                float i = 3f + Mathf.Sin(t * Mathf.PI * 2f) * 1.5f;
                mat.SetColor("_EmissionColor", Color.white * i);
            }, 1f, 0.6f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
    }

    void OnDestroy() => glowTween?.Kill();
}
