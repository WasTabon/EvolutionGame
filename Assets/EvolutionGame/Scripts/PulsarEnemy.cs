using UnityEngine;
using DG.Tweening;
using System.Collections;

public class PulsarEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private Tweener flashTween;
    public float pulseInterval = 2.2f;
    public float pulseForce = 8f;
    public float pulseRadius = 10f;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(0.9f, 1.5f);
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(0.3f, 0.7f, 1f), 2f);
        AnimateIntro();
        SetupGravity();
        StartCoroutine(PulseRoutine());
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 3f;
        gravBody.radius = pulseRadius;
        gravBody.gravityType = GravityType.Repel;
    }

    IEnumerator PulseRoutine()
    {
        while (isAlive)
        {
            yield return new WaitForSeconds(pulseInterval);
            if (!isAlive) yield break;
            FirePulse();
        }
    }

    void FirePulse()
    {
        flashTween?.Kill();

        if (mat != null)
        {
            mat.SetColor("_EmissionColor", new Color(0.3f, 0.7f, 1f) * 6f);
            DOTween.To(
                () => 6f, v => mat?.SetColor("_EmissionColor", new Color(0.3f, 0.7f, 1f) * v),
                2f, 0.5f
            ).SetEase(Ease.OutQuart);
        }

        transform.DOPunchScale(Vector3.one * 0.4f, 0.3f, 4, 0.5f);

        PlayerController pc = Object.FindObjectOfType<PlayerController>();
        if (pc == null) return;
        float dist = Vector3.Distance(transform.position, pc.transform.position);
        if (dist < pulseRadius)
        {
            Vector3 dir = (pc.transform.position - transform.position).normalized;
            float strength = (1f - dist / pulseRadius) * pulseForce;
        }
    }

    void OnDestroy()
    {
        flashTween?.Kill();
    }
}
