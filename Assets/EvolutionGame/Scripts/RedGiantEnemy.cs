using UnityEngine;
using DG.Tweening;

public class RedGiantEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private Tweener pulseTween;
    private float currentScale;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(2.5f, 4f);
        currentScale = scale;
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(1f, 0.25f, 0.05f), 1.6f);
        AnimateIntro();
        SetupGravity();
        StartPulse();
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 4f;
        gravBody.radius = scale * 5f;
        gravBody.gravityType = GravityType.Attract;
    }

    void StartPulse()
    {
        if (mat == null) return;
        pulseTween = DOTween.To(
            () => 0f, t =>
            {
                if (mat == null) return;
                float i = 1.2f + Mathf.Sin(t * Mathf.PI * 2f) * 0.5f;
                mat.SetColor("_EmissionColor", new Color(1f, 0.25f, 0.05f) * i);
            }, 1f, 3f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
    }

    void OnTriggerEnter(Collider other)
    {
        WorldObject wo = other.GetComponent<WorldObject>();
        if (wo == null || wo.GetScale() >= currentScale * 0.5f) return;
        currentScale += wo.GetGrowthAmount() * 0.3f;
        transform.DOScale(Vector3.one * currentScale, 0.3f).SetEase(Ease.OutBack);
        wo.GetAbsorbed(transform.position);
        if (gravBody != null) gravBody.mass = currentScale * 4f;
    }

    public override bool CanBeAbsorbed(float playerScale) => playerScale > currentScale * 1.1f;

    void OnDestroy() => pulseTween?.Kill();
}
