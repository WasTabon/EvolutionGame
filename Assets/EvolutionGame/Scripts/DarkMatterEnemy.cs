using UnityEngine;
using DG.Tweening;

public class DarkMatterEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private Tweener fadeTween;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(1.2f, 2.2f);
    }

    protected override void Start()
    {
        if (mat != null)
        {
            mat.color = new Color(0.55f, 0.55f, 0.65f, 0.12f);
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", new Color(0.4f, 0.4f, 0.55f) * 0.3f);
            mat.SetFloat("_Smoothness", 0.6f);

            if (mat.HasProperty("_Surface"))
            {
                mat.SetFloat("_Surface", 1f);
                mat.renderQueue = 3000;
            }
        }

        AnimateIntro();
        SetupGravity();
        StartFade();
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 3f;
        gravBody.radius = scale * 8f;
        gravBody.gravityType = GravityType.Attract;
    }

    void StartFade()
    {
        if (mat == null) return;
        fadeTween = DOTween.To(
            () => 0f, t =>
            {
                if (mat == null) return;
                float alpha = 0.08f + Mathf.Sin(t * Mathf.PI * 2f) * 0.06f;
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }, 1f, 4f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
    }

    void OnDestroy() => fadeTween?.Kill();
}
