using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class SetupGameScene_Iteration3
{
    [MenuItem("EvolutionGame/Setup Game Scene (Iteration 3)")]
    static void Setup()
    {
        EnsureCameraShake();
        EnsurePlayerTrail();
        EnsureAbsorptionEffect();
        UpdateWorldObjectMaterials();

        EditorApplication.ExecuteMenuItem("File/Save");
        Debug.Log("[Iteration 3] Game Scene additions complete!");
    }

    static void EnsureCameraShake()
    {
        if (Object.FindObjectOfType<CameraShake>() != null) return;

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("[Iteration 3] Main Camera not found!");
            return;
        }

        CameraShake shake = cam.gameObject.GetComponent<CameraShake>() ?? cam.gameObject.AddComponent<CameraShake>();
        EditorUtility.SetDirty(cam.gameObject);
        Debug.Log("[Iteration 3] CameraShake added to Main Camera.");
    }

    static void EnsurePlayerTrail()
    {
        PlayerController player = Object.FindObjectOfType<PlayerController>();
        if (player == null)
        {
            Debug.LogWarning("[Iteration 3] Player not found! Run Iteration 1 setup first.");
            return;
        }

        Transform existingTrailObj = player.transform.Find("Trail");
        if (existingTrailObj != null) return;

        GameObject trailGo = new GameObject("Trail");
        trailGo.transform.SetParent(player.transform, false);
        trailGo.transform.localPosition = Vector3.zero;

        TrailRenderer trail = trailGo.AddComponent<TrailRenderer>();
        trail.time = 0.45f;
        trail.widthMultiplier = 0.6f;
        trail.minVertexDistance = 0.05f;
        trail.shadowCastingMode = ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.alignment = LineAlignment.View;

        AnimationCurve widthCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        );
        trail.widthCurve = widthCurve;

        Material trailMat = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        trailMat.color = new Color(0.5f, 0.75f, 1f, 0.7f);

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.6f, 0.85f, 1f), 0f),
                new GradientColorKey(new Color(0.3f, 0.5f, 1f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.8f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = gradient;
        trail.material = trailMat;

        PlayerTrail playerTrail = trailGo.AddComponent<PlayerTrail>();
        using (var so = new SerializedObject(playerTrail))
        {
            so.FindProperty("trailRenderer").objectReferenceValue = trail;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        EditorUtility.SetDirty(player.gameObject);
        Undo.RegisterCreatedObjectUndo(trailGo, "Create Player Trail");
    }

    static void EnsureAbsorptionEffect()
    {
        if (Object.FindObjectOfType<AbsorptionEffect>() != null) return;

        GameObject go = new GameObject("AbsorptionEffect");
        go.transform.position = Vector3.zero;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        go.AddComponent<AbsorptionEffect>();

        var main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = 0.4f;
        main.startSpeed = 4f;
        main.startSize = 0.15f;
        main.startColor = new ParticleSystem.MinMaxGradient(
            new Color(0.6f, 0.85f, 1f, 1f),
            new Color(0.9f, 0.7f, 1f, 1f)
        );
        main.maxParticles = 50;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.enabled = true;
        emission.rateOverTime = 0f;
        emission.SetBursts(new ParticleSystem.Burst[] {
            new ParticleSystem.Burst(0f, 12)
        });

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.2f;

        var velocityOverLifetime = ps.velocityOverLifetime;
        velocityOverLifetime.enabled = false;

        var sizeOverLifetime = ps.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(1f, 0f)
        );
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        renderer.material.color = Color.white;
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.shadowCastingMode = ShadowCastingMode.Off;

        Undo.RegisterCreatedObjectUndo(go, "Create AbsorptionEffect");
    }

    static void UpdateWorldObjectMaterials()
    {
        if (!AssetDatabase.IsValidFolder("Assets/EvolutionGame/Materials"))
            AssetDatabase.CreateFolder("Assets/EvolutionGame", "Materials");

        CreateEmissiveMaterial("WorldObject_Small_Mat",
            new Color(0.3f, 0.6f, 1f),
            new Color(0.1f, 0.4f, 1f) * 1.5f,
            "Assets/EvolutionGame/Configs/WorldObject_Small.asset");

        CreateEmissiveMaterial("WorldObject_Medium_Mat",
            new Color(0.9f, 0.6f, 0.2f),
            new Color(0.9f, 0.4f, 0.05f) * 1.2f,
            "Assets/EvolutionGame/Configs/WorldObject_Medium.asset");

        CreateEmissiveMaterial("WorldObject_Large_Mat",
            new Color(1f, 0.25f, 0.25f),
            new Color(1f, 0.1f, 0.05f) * 1.0f,
            "Assets/EvolutionGame/Configs/WorldObject_Large.asset");

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    static void CreateEmissiveMaterial(string matName, Color baseColor, Color emissionColor, string configPath)
    {
        string matPath = "Assets/EvolutionGame/Materials/" + matName + ".mat";
        Material mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

        if (mat == null)
        {
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            AssetDatabase.CreateAsset(mat, matPath);
        }

        mat.color = baseColor;
        mat.SetFloat("_Smoothness", 0.85f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", emissionColor);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        EditorUtility.SetDirty(mat);

        WorldObjectConfig cfg = AssetDatabase.LoadAssetAtPath<WorldObjectConfig>(configPath);
        if (cfg == null) return;

        using (var so = new SerializedObject(cfg))
        {
            so.FindProperty("material").objectReferenceValue = mat;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        EditorUtility.SetDirty(cfg);
    }
}
