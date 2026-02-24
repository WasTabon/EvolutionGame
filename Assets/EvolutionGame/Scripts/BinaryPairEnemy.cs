using UnityEngine;
using DG.Tweening;

public class BinaryPairEnemy : BaseEnemy
{
    private GameObject bodyB;
    private MeshRenderer bodyBRenderer;
    private Material bodyBMat;
    private float angle = 0f;
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 90f;

    private static readonly Color colorA = new Color(0.3f, 0.5f, 1f);
    private static readonly Color colorB = new Color(1f, 0.55f, 0.1f);

    protected override void Awake()
    {
        base.Awake();
        scale = Random.Range(0.8f, 1.3f);
    }

    protected override void Start()
    {
        SetEmissiveColor(colorA, 2f);
        AnimateIntro();
        CreateBodyB();
        AddGravity(gameObject, scale * 5f);
        AddGravity(bodyB, scale * 5f);
    }

    void CreateBodyB()
    {
        bodyB = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bodyB.name = "BinaryB";
        bodyB.transform.localScale = Vector3.one * scale;

        Collider col = bodyB.GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        bodyBRenderer = bodyB.GetComponent<MeshRenderer>();
        bodyBMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        bodyBMat.color = colorB;
        bodyBMat.EnableKeyword("_EMISSION");
        bodyBMat.SetColor("_EmissionColor", colorB * 2f);
        bodyBMat.SetFloat("_Smoothness", 0.9f);
        bodyBRenderer.material = bodyBMat;

        bodyB.transform.position = transform.position + Vector3.right * orbitRadius;
        bodyB.AddComponent<BinaryContactKiller>();
    }

    void AddGravity(GameObject go, float mass)
    {
        GravitationalBody gb = go.AddComponent<GravitationalBody>();
        gb.mass = mass;
        gb.radius = scale * 8f;
        gb.gravityType = GravityType.Attract;
    }

    void Update()
    {
        if (!isAlive || bodyB == null) return;
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;

        angle += orbitSpeed * Time.deltaTime;
        float rad = angle * Mathf.Deg2Rad;
        Vector3 center = transform.position;
        bodyB.transform.position = center + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * orbitRadius;
    }

    public override void OnAbsorbedByPlayer()
    {
        isAlive = false;
        if (bodyB != null) Destroy(bodyB);
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (bodyB != null) Destroy(bodyB);
    }
}

public class BinaryContactKiller : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;
        BinaryPairEnemy parent = GetComponentInParent<BinaryPairEnemy>()
            ?? Object.FindObjectOfType<BinaryPairEnemy>();
        if (parent == null) return;
        parent.HandleContact(pc);
    }
}

public static class BinaryPairEnemyExtensions
{
    public static void HandleContact(this BinaryPairEnemy enemy, PlayerController pc)
    {
        float ps = pc.GetCurrentScale();
        if (ps > enemy.scale * 1.1f)
        {
            ScoreManager.Instance?.AddScore(enemy.scale * 20f, enemy.transform.position);
            enemy.OnAbsorbedByPlayer();
        }
        else if (ps < enemy.scale * 0.9f)
        {
            pc.ForceKill();
        }
    }
}
