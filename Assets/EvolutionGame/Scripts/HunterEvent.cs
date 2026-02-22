using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HunterEvent : MonoBehaviour
{
    public float duration = 15f;
    public float hunterSpeed = 2.5f;
    public float hunterScale = 2.2f;
    public float fleeThreshold = 0.85f;

    private GameObject hunterGo;
    private bool isActive;

    public void Begin()
    {
        EventAnnouncementUI.Instance?.Show("Hunter", "Predator detected! Flee!", new Color(1f, 0.2f, 0.2f));
        StartCoroutine(RunEvent());
    }

    IEnumerator RunEvent()
    {
        isActive = true;
        SpawnHunter();

        float elapsed = 0f;
        while (elapsed < duration && hunterGo != null)
        {
            elapsed += Time.deltaTime;
            UpdateHunter();
            yield return null;
        }

        DespawnHunter();
        isActive = false;
        GameEventManager.Instance?.OnEventFinished();
    }

    void SpawnHunter()
    {
        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player == null) return;

        Vector2 dir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.transform.position + new Vector3(dir.x, 0f, dir.y) * 20f;

        hunterGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        hunterGo.name = "Hunter";
        hunterGo.transform.position = spawnPos;
        hunterGo.transform.localScale = Vector3.one * hunterScale;

        SphereCollider col = hunterGo.GetComponent<SphereCollider>();
        col.isTrigger = true;

        Rigidbody rb = hunterGo.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        MeshRenderer mr = hunterGo.GetComponent<MeshRenderer>();
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.color = new Color(1f, 0.15f, 0.1f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 0.05f, 0f) * 1.8f);
        mr.material = mat;

        hunterGo.AddComponent<HunterContactKiller>();

        hunterGo.transform.localScale = Vector3.zero;
        hunterGo.transform.DOScale(Vector3.one * hunterScale, 0.5f).SetEase(Ease.OutBack);
    }

    void UpdateHunter()
    {
        if (hunterGo == null) return;

        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player == null) return;

        float playerScale = player.GetCurrentScale();
        Vector3 dir = (player.transform.position - hunterGo.transform.position).normalized;

        if (playerScale >= hunterScale * fleeThreshold)
            dir = -dir;

        hunterGo.transform.position += dir * hunterSpeed * Time.deltaTime;
    }

    void DespawnHunter()
    {
        if (hunterGo == null) return;
        hunterGo.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack)
            .OnComplete(() => { if (hunterGo != null) Destroy(hunterGo); });
    }
}

public class HunterContactKiller : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        PlayerController pc = other.GetComponent<PlayerController>();
        if (pc == null) return;

        float playerScale = pc.GetCurrentScale();
        float hunterScale = transform.localScale.x;

        if (playerScale < hunterScale * 0.9f)
            pc.ForceKill();
    }
}
