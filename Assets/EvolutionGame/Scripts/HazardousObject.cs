using UnityEngine;
using DG.Tweening;

public class HazardousObject : MonoBehaviour
{
    public float moveSpeed = 1.2f;
    public float warningRadius = 3.5f;

    private Vector3 moveDirection;
    private MeshRenderer meshRenderer;
    private Transform playerTransform;
    private Color baseEmission;
    private bool isWarning;
    private Tweener pulseTweener;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        moveDirection = new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;

        PlayerController pc = Object.FindObjectOfType<PlayerController>();
        if (pc != null) playerTransform = pc.transform;

        if (meshRenderer != null && meshRenderer.material.HasProperty("_EmissionColor"))
            baseEmission = meshRenderer.material.GetColor("_EmissionColor");

        StartPulse();
    }

    void StartPulse()
    {
        if (meshRenderer == null) return;
        pulseTweener?.Kill();
        pulseTweener = DOTween.To(
            () => 0f, t =>
            {
                if (meshRenderer == null) return;
                float intensity = 0.5f + Mathf.Sin(t * Mathf.PI * 2f) * 0.5f;
                meshRenderer.material.SetColor("_EmissionColor", new Color(1f, 0.1f, 0.05f) * intensity * 1.5f);
            },
            1f, 1f
        ).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (playerTransform != null)
        {
            float dist = Vector3.Distance(transform.position, playerTransform.position);
            if (dist < warningRadius && !isWarning)
            {
                isWarning = true;
                pulseTweener?.Kill();
                pulseTweener = DOTween.To(
                    () => 0f, t =>
                    {
                        if (meshRenderer == null) return;
                        float intensity = 0.5f + Mathf.Sin(t * Mathf.PI * 2f) * 0.5f;
                        meshRenderer.material.SetColor("_EmissionColor", new Color(1f, 0.1f, 0.05f) * intensity * 3f);
                    },
                    1f, 0.35f
                ).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);
            }
            else if (dist >= warningRadius && isWarning)
            {
                isWarning = false;
                StartPulse();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>() != null)
            other.GetComponent<PlayerController>().ForceKill();
    }

    void OnDestroy()
    {
        pulseTweener?.Kill();
    }
}
