using UnityEngine;
using DG.Tweening;

public class CometEnemy : BaseEnemy
{
    public float speed = 7f;
    public float bounds = 25f;

    private Vector3 direction;
    private TrailRenderer trail;
    private Tweener glowTween;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(0.5f, 1.1f);
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(0.3f, 1f, 0.95f), 2.5f);
        AnimateIntro();

        direction = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 0.7f;
        trail.widthMultiplier = scale * 0.4f;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.3f, 1f, 0.95f), 0f),
                new GradientColorKey(Color.white, 0.3f),
                new GradientColorKey(new Color(0.3f, 1f, 0.95f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = g;

        Material trailMat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        trailMat.color = new Color(0.3f, 1f, 0.95f);
        trail.material = trailMat;

        StartGlow();
    }

    void Update()
    {
        if (!isAlive) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;

        transform.position += direction * speed * Time.deltaTime;

        Vector3 pos = transform.position;
        if (Mathf.Abs(pos.x) > bounds) { direction.x = -direction.x; pos.x = Mathf.Sign(pos.x) * bounds; }
        if (Mathf.Abs(pos.z) > bounds) { direction.z = -direction.z; pos.z = Mathf.Sign(pos.z) * bounds; }
        transform.position = pos;
    }

    void StartGlow()
    {
        if (mat == null) return;
        glowTween = DOTween.To(
            () => 0f, t =>
            {
                if (mat == null) return;
                float i = 2f + Mathf.Sin(t * Mathf.PI * 2f) * 0.8f;
                mat.SetColor("_EmissionColor", new Color(0.3f, 1f, 0.95f) * i);
            }, 1f, 0.4f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
    }

    void OnDestroy() => glowTween?.Kill();
}
