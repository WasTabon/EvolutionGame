using UnityEngine;
using DG.Tweening;
using System.Collections;

public class SupernovaEnemy : BaseEnemy
{
    public float buildupTime = 10f;
    public float blastRadius = 12f;
    private Tweener buildupTween;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(1.4f, 2.2f);
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(1f, 0.95f, 0.7f), 1.5f);
        AnimateIntro();
        StartCoroutine(BuildupRoutine());
    }

    IEnumerator BuildupRoutine()
    {
        float elapsed = 0f;
        while (elapsed < buildupTime && isAlive)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / buildupTime;

            if (mat != null)
            {
                float intensity = Mathf.Lerp(1.5f, 8f, t);
                Color c = Color.Lerp(new Color(1f, 0.95f, 0.7f), new Color(1f, 0.3f, 0.05f), t);
                mat.color = c;
                mat.SetColor("_EmissionColor", c * intensity);
            }

            float scaleBoost = scale * (1f + t * 0.4f);
            transform.localScale = Vector3.one * scaleBoost;

            yield return null;
        }

        if (isAlive) Explode();
    }

    void Explode()
    {
        isAlive = false;

        if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.6f, 0.8f, 16);
        AbsorptionEffect.Instance?.Play(transform.position, scale * 2f, new Color(1f, 0.4f, 0.1f));

        GameObject shockwave = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shockwave.name = "Shockwave";
        shockwave.transform.position = transform.position;
        shockwave.transform.localScale = Vector3.one * 0.5f;
        Collider sc = shockwave.GetComponent<Collider>();
        if (sc != null) sc.isTrigger = true;

        MeshRenderer smr = shockwave.GetComponent<MeshRenderer>();
        Material smat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        smat.color = new Color(1f, 0.5f, 0.1f, 0.5f);
        smat.EnableKeyword("_EMISSION");
        smat.SetColor("_EmissionColor", new Color(1f, 0.4f, 0.05f) * 4f);
        if (smat.HasProperty("_Surface")) { smat.SetFloat("_Surface", 1f); smat.renderQueue = 3000; }
        smr.material = smat;

        PlayerController pc = Object.FindObjectOfType<PlayerController>();
        if (pc != null)
        {
            float dist = Vector3.Distance(transform.position, pc.transform.position);
            if (dist < blastRadius) pc.ForceKill();
        }

        shockwave.transform.DOScale(Vector3.one * blastRadius * 2f, 0.6f).SetEase(Ease.OutQuart);
        smat.DOFade(0f, 0.6f).OnComplete(() => Destroy(shockwave));

        Destroy(gameObject);
    }

    public override void OnAbsorbedByPlayer()
    {
        StopAllCoroutines();
        buildupTween?.Kill();
        isAlive = false;
        base.OnAbsorbedByPlayer();
    }

    void OnDestroy() => buildupTween?.Kill();
}
