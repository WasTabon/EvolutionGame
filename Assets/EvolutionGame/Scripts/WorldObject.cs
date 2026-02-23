using UnityEngine;
using DG.Tweening;

public class WorldObject : MonoBehaviour
{
    private WorldObjectConfig config;
    private float scale;
    private Vector3 moveDirection;
    private bool isAbsorbed;

    private Transform playerTransform;
    private float scatterRadius = 4f;
    private float scatterStrength = 2.5f;

    public void Initialize(WorldObjectConfig cfg)
    {
        config = cfg;
        scale = cfg.scale;
        isAbsorbed = false;
        transform.localScale = Vector3.one * scale;
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (cfg.mesh != null && mf != null) mf.mesh = cfg.mesh;
        if (cfg.material != null && mr != null) mr.sharedMaterial = cfg.material;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        PlayerController pc = Object.FindObjectOfType<PlayerController>();
        if (pc != null) playerTransform = pc.transform;
    }

    void Update()
    {
        if (isAbsorbed || config == null) return;

        if (GetComponent<OrbitalObject>()?.enabled == true) return;

        float speedMult = DifficultyManager.Instance != null
            ? DifficultyManager.Instance.Current.speedMultiplier
            : 1f;

        if (GravitySystem.Instance != null)
        {
            Vector3 gravity = GravitySystem.Instance.GetForceAt(transform.position, scale);
            moveDirection = (moveDirection * config.moveSpeed * speedMult + gravity * Time.deltaTime).normalized;
        }

        Vector3 move = moveDirection * config.moveSpeed * speedMult;

        if (config.type == WorldObjectType.Small && playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist < scatterRadius)
            {
                Vector3 away = (transform.position - playerTransform.position).normalized;
                float scatter = (1f - dist / scatterRadius) * scatterStrength;
                move += away * scatter;
            }
        }

        if (config.type == WorldObjectType.Large)
        {
            moveDirection = Vector3.Lerp(moveDirection,
                new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized,
                Time.deltaTime * 0.3f).normalized;
        }

        transform.position += move * Time.deltaTime;
    }

    public void GetAbsorbed(Vector3 targetPos)
    {
        isAbsorbed = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Vector3 awayDir = (transform.position - targetPos).normalized;
        Vector3 anticipationPos = transform.position + awayDir * (scale * 0.4f);

        transform.DOKill();
        transform.DOMove(anticipationPos, 0.07f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            transform.DOMove(targetPos, 0.22f).SetEase(Ease.InCubic);
            transform.DOScale(Vector3.zero, 0.22f).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if (ObjectPool.Instance != null && config != null)
                    ObjectPool.Instance.Return(gameObject, config);
                else
                    Destroy(gameObject);
            });
        });

        transform.DOScale(transform.localScale * 1.15f, 0.07f).SetEase(Ease.OutQuad);
    }

    public float GetScale() => scale;
    public float GetPoints() => config != null ? config.points : 0f;
    public float GetGrowthAmount() => config != null ? config.growthAmount : 0f;
    public WorldObjectConfig GetConfig() => config;
}
