using UnityEngine;
using System.Collections;
using DG.Tweening;

public class GravitationalWaveEvent : MonoBehaviour
{
    public static GravitationalWaveEvent Instance;

    public float duration = 6f;
    public float maxForce = 4f;

    private float currentForce;
    private bool isActive;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public float GetForce() => isActive ? currentForce : 0f;

    public void Begin()
    {
        EventAnnouncementUI.Instance?.Show("Gravitational Wave", "Gravitational anomaly detected!", new Color(1f, 0.6f, 0.1f));
        StartCoroutine(RunEvent());
    }

    IEnumerator RunEvent()
    {
        isActive = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            currentForce = Mathf.Sin(t * Mathf.PI) * maxForce;
            yield return null;
        }

        currentForce = 0f;
        isActive = false;
        GameEventManager.Instance?.OnEventFinished();
    }
}
