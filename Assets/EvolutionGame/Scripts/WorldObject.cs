using UnityEngine;
using DG.Tweening;

public class WorldObject : MonoBehaviour
{
    private WorldObjectConfig config;
    private float scale;
    private Vector3 moveDirection;
    private bool isAbsorbed;

    public void Initialize(WorldObjectConfig cfg)
    {
        config = cfg;
        scale = cfg.scale;
        transform.localScale = Vector3.one * scale;

        moveDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        MeshFilter mf = GetComponent<MeshFilter>();
        MeshRenderer mr = GetComponent<MeshRenderer>();

        if (cfg.mesh != null && mf != null)
            mf.mesh = cfg.mesh;
        if (cfg.material != null && mr != null)
            mr.material = cfg.material;
    }

    void Update()
    {
        if (isAbsorbed) return;
        transform.position += moveDirection * config.moveSpeed * Time.deltaTime;
    }

    public void GetAbsorbed(Vector3 targetPos)
    {
        isAbsorbed = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        transform.DOMove(targetPos, 0.25f).SetEase(Ease.InCubic);
        transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InCubic)
            .OnComplete(() => Destroy(gameObject));
    }

    public float GetScale() => scale;
    public float GetPoints() => config.points;
    public float GetGrowthAmount() => config.growthAmount;
}
