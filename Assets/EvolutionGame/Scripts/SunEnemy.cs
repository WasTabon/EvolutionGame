using UnityEngine;
using DG.Tweening;

public class SunEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private OrbitalSpawner orbitalSpawner;
    private Tweener pulseTween;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(1.8f, 2.8f);
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(1f, 0.75f, 0.1f), 2.2f);
        AnimateIntro();
        SetupGravity();
        StartPulse();

        orbitalSpawner = gameObject.AddComponent<OrbitalSpawner>();
        orbitalSpawner.smallCount  = Random.Range(2, 5);
        orbitalSpawner.mediumCount = Random.Range(1, 3);
        orbitalSpawner.minOrbitRadius = scale * 2f;
        orbitalSpawner.maxOrbitRadius = scale * 4.5f;

        SpawnManager sm = Object.FindObjectOfType<SpawnManager>();
        if (sm != null)
            orbitalSpawner.SpawnOrbiters(sm.smallConfigs, sm.mediumConfigs);
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 6f;
        gravBody.radius = scale * 9f;
        gravBody.gravityType = GravityType.Attract;
    }

    void StartPulse()
    {
        if (mat == null) return;
        pulseTween = DOTween.To(
            () => 0f, t =>
            {
                if (mat == null) return;
                float intensity = 1.8f + Mathf.Sin(t * Mathf.PI * 2f) * 0.6f;
                mat.SetColor("_EmissionColor", new Color(1f, 0.75f, 0.1f) * intensity);
            }, 1f, 1.8f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
    }

    public override void OnAbsorbedByPlayer()
    {
        pulseTween?.Kill();
        if (gravBody != null) gravBody.enabled = false;
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        pulseTween?.Kill();
    }
}
