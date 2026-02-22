using UnityEngine;

public class AbsorptionEffect : MonoBehaviour
{
    public static AbsorptionEffect Instance;

    private ParticleSystem particles;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        particles = GetComponent<ParticleSystem>();
        Debug.Assert(particles != null, "AbsorptionEffect: ParticleSystem not found!");
    }

    public void Play(Vector3 worldPos, float scaleFactor = 1f, Color? color = null)
    {
        transform.position = worldPos;

        var main = particles.main;
        main.startSizeMultiplier = scaleFactor;
        main.startSpeedMultiplier = scaleFactor * 1.5f;

        if (color.HasValue)
        {
            main.startColor = new ParticleSystem.MinMaxGradient(
                color.Value,
                Color.Lerp(color.Value, Color.white, 0.4f)
            );
        }

        particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        particles.Play();
    }
}
