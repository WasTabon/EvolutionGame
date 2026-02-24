using UnityEngine;
using DG.Tweening;

public class BlackHoleEnemy : BaseEnemy
{
    private GravitationalBody gravBody;
    private Tweener spinTween;
    private Tweener ringTween;
    private GameObject accretionRing;

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(1.5f, 2.5f);
        isLethalOnContact = true;
    }

    protected override void Start()
    {
        SetEmissiveColor(new Color(0.25f, 0.05f, 0.4f), 1.8f);
        AnimateIntro();
        SetupGravity();
        CreateAccretionRing();
        StartEffects();
    }

    void SetupGravity()
    {
        gravBody = gameObject.AddComponent<GravitationalBody>();
        gravBody.mass = scale * 12f;
        gravBody.radius = scale * 14f;
        gravBody.gravityType = GravityType.Attract;
    }

    void CreateAccretionRing()
    {
        accretionRing = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        accretionRing.name = "AccretionRing";
        accretionRing.transform.SetParent(transform);
        accretionRing.transform.localPosition = Vector3.zero;
        accretionRing.transform.localScale = new Vector3(scale * 2.2f, 0.05f, scale * 2.2f);

        Collider c = accretionRing.GetComponent<Collider>();
        if (c != null) Destroy(c);

        Material ringMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        ringMat.color = new Color(0.6f, 0.1f, 1f, 0.6f);
        ringMat.EnableKeyword("_EMISSION");
        ringMat.SetColor("_EmissionColor", new Color(0.5f, 0.05f, 0.8f) * 3f);
        ringMat.SetFloat("_Smoothness", 1f);
        accretionRing.GetComponent<MeshRenderer>().material = ringMat;
    }

    void StartEffects()
    {
        spinTween = accretionRing?.transform.DORotate(new Vector3(0f, 360f, 0f), 2f, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        if (mat != null)
        {
            ringTween = DOTween.To(
                () => 0f, t =>
                {
                    if (mat == null) return;
                    float i = 1.4f + Mathf.Sin(t * Mathf.PI * 2f) * 0.5f;
                    mat.SetColor("_EmissionColor", new Color(0.25f, 0.05f, 0.4f) * i);
                }, 1f, 2.5f
            ).SetLoops(-1, LoopType.Restart).SetEase(Ease.InOutSine);
        }
    }

    public override bool CanBeAbsorbed(float playerScale) => false;

    void OnDestroy()
    {
        spinTween?.Kill();
        ringTween?.Kill();
    }
}
