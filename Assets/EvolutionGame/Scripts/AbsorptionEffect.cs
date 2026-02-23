using UnityEngine;
using DG.Tweening;

public class AbsorptionEffect : MonoBehaviour
{
    public static AbsorptionEffect Instance;

    private ParticleSystem particles;

    [Header("Micro Spheres")]
    public int microSphereCount = 8;
    public float microSphereBaseSize = 0.12f;
    public float microSphereLifetime = 2.2f;
    public float microSphereSpeed = 4.5f;

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
        main.startSizeMultiplier  = scaleFactor;
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

        Color sphereColor = color ?? new Color(0.5f, 0.8f, 1f);
        SpawnMicroSpheres(worldPos, scaleFactor, sphereColor);
    }

    void SpawnMicroSpheres(Vector3 origin, float scaleFactor, Color color)
    {
        int count = Mathf.RoundToInt(microSphereCount * Mathf.Clamp(scaleFactor, 0.5f, 3f));

        for (int i = 0; i < count; i++)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.name = "MicroSphere";

            Collider col = sphere.GetComponent<Collider>();
            if (col != null) Destroy(col);

            float size = microSphereBaseSize * Random.Range(0.6f, 1.4f) * Mathf.Clamp(scaleFactor, 0.4f, 2f);
            sphere.transform.position = origin;
            sphere.transform.localScale = Vector3.one * size;

            Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = color;
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", color * Random.Range(1.5f, 3f));
            mat.SetFloat("_Smoothness", 0.9f);
            mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;

            MeshRenderer mr = sphere.GetComponent<MeshRenderer>();
            mr.material = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            Vector3 dir = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-0.3f, 0.3f),
                Random.Range(-1f, 1f)
            ).normalized;

            float speed = microSphereSpeed * Random.Range(0.5f, 1.5f) * Mathf.Clamp(scaleFactor, 0.5f, 2.5f);

            sphere.transform.DOMove(origin + dir * speed, microSphereLifetime)
                .SetEase(Ease.OutQuart);

            sphere.transform.DOScale(Vector3.zero, microSphereLifetime * 0.7f)
                .SetEase(Ease.InQuart)
                .SetDelay(microSphereLifetime * 0.3f);

            mat.DOFade(0f, microSphereLifetime * 0.5f)
                .SetDelay(microSphereLifetime * 0.5f)
                .OnComplete(() =>
                {
                    if (sphere != null) Destroy(sphere);
                });
        }
    }
}
