using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Shake(float duration = 0.3f, float strength = 0.4f, int vibrato = 10)
    {
        Camera.main.transform.DOKill();
        Camera.main.transform.DOShakePosition(duration, new Vector3(strength, strength, 0f), vibrato, 90f, false, true);
    }
}
