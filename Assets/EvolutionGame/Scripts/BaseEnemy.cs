using UnityEngine;
using DG.Tweening;

public abstract class BaseEnemy : MonoBehaviour
{
    protected MeshRenderer meshRenderer;
    protected Material mat;
    protected bool isAlive = true;

    public float scale = 1.5f;
    public bool isLethalOnContact = false;

    protected virtual void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null) mat = meshRenderer.material;
    }

    protected virtual void Start() { }

    public virtual bool CanBeAbsorbed(float playerScale)
    {
        return playerScale > scale * 1.1f;
    }

    public virtual void OnAbsorbedByPlayer()
    {
        Destroy(gameObject);
    }

    protected void SetEmissiveColor(Color baseColor, float intensity = 1.5f)
    {
        if (mat == null) return;
        mat.color = baseColor;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", baseColor * intensity);
        mat.SetFloat("_Smoothness", 0.9f);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
    }

    protected void AnimateIntro()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one * scale, 0.6f).SetEase(Ease.OutBack);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isAlive) return;
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;
        HandlePlayerContact(pc);
    }

    protected virtual void HandlePlayerContact(PlayerController player)
    {
        float playerScale = player.GetCurrentScale();

        if (isLethalOnContact)
        {
            player.ForceKill();
            return;
        }

        if (CanBeAbsorbed(playerScale))
        {
            ScoreManager.Instance?.AddScore(scale * 15f, transform.position);
            AbsorptionEffect.Instance?.Play(transform.position, scale, mat?.color);
            isAlive = false;
            OnAbsorbedByPlayer();
        }
        else if (playerScale < scale * 0.9f)
        {
            player.ForceKill();
        }
    }
}
