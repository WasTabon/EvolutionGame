using UnityEngine;
using DG.Tweening;
using System.Collections;

public class MagnetarEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private Tweener colorTween;
    public float switchInterval = 3f;
    private bool attracting = true;

    private static readonly Color attractColor = new Color(0.9f, 0.3f, 1f);
    private static readonly Color repelColor   = new Color(0.3f, 1f, 0.5f);

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(1f, 1.8f);
    }

    protected override void Start()
    {
        SetEmissiveColor(attractColor, 2f);
        AnimateIntro();
        SetupGravity();
        StartCoroutine(SwitchRoutine());
        PulseColor(attractColor);
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 8f;
        gravBody.radius = scale * 10f;
        gravBody.gravityType = GravityType.Attract;
    }

    IEnumerator SwitchRoutine()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(switchInterval);
            if (!isAlive) yield break;
            attracting = !attracting;
            gravBody.gravityType = attracting ? GravityType.Attract : GravityType.Repel;
            Color c = attracting ? attractColor : repelColor;
            SetEmissiveColor(c, 2f);
            PulseColor(c);
            transform.DOPunchScale(Vector3.one * 0.35f, 0.3f, 5, 0.5f);
        }
    }

    void PulseColor(Color c)
    {
        colorTween?.Kill();
        colorTween = DOTween.To(
            () => 0f, t =>
            {
                if (mat == null) return;
                float i = 1.8f + Mathf.Sin(t * Mathf.PI * 2f) * 0.8f;
                mat.SetColor("_EmissionColor", c * i);
            }, 1f, switchInterval * 0.5f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
    }

    void OnDestroy() => colorTween?.Kill();
}
