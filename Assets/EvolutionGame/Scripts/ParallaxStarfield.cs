using UnityEngine;
using System.Collections.Generic;

public class ParallaxStarfield : MonoBehaviour
{
    [System.Serializable]
    public class StarLayer
    {
        public int count = 60;
        public float parallaxFactor = 0.2f;
        public float minSize = 0.05f;
        public float maxSize = 0.15f;
        public float minAlpha = 0.2f;
        public float maxAlpha = 0.7f;
        public float twinkleSpeed = 1f;
    }

    public StarLayer[] layers = new StarLayer[]
    {
        new StarLayer { count = 80, parallaxFactor = 0.05f, minSize = 0.04f, maxSize = 0.10f, minAlpha = 0.15f, maxAlpha = 0.35f, twinkleSpeed = 0.5f },
        new StarLayer { count = 50, parallaxFactor = 0.15f, minSize = 0.08f, maxSize = 0.18f, minAlpha = 0.3f,  maxAlpha = 0.6f,  twinkleSpeed = 1.0f },
        new StarLayer { count = 25, parallaxFactor = 0.30f, minSize = 0.14f, maxSize = 0.28f, minAlpha = 0.4f,  maxAlpha = 0.8f,  twinkleSpeed = 1.5f },
    };

    public float fieldRadius = 35f;

    private class StarData
    {
        public Transform transform;
        public MeshRenderer renderer;
        public Vector3 baseOffset;
        public float twinkleOffset;
        public float twinkleSpeed;
        public float baseAlpha;
        public Color color;
    }

    private List<List<StarData>> layerStars = new List<List<StarData>>();
    private Transform cameraTransform;
    private Vector3 lastCamPos;

    void Start()
    {
        cameraTransform = Camera.main != null ? Camera.main.transform : null;
        if (cameraTransform != null)
            lastCamPos = cameraTransform.position;

        for (int i = 0; i < layers.Length; i++)
            layerStars.Add(CreateLayer(layers[i], i));
    }

    List<StarData> CreateLayer(StarLayer layer, int layerIndex)
    {
        List<StarData> stars = new List<StarData>();

        GameObject layerParent = new GameObject("StarLayer_" + layerIndex);
        layerParent.transform.SetParent(transform);

        Material sharedMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        sharedMat.color = Color.white;
        sharedMat.EnableKeyword("_EMISSION");
        sharedMat.SetColor("_EmissionColor", Color.white * 0.5f);
        sharedMat.SetFloat("_Smoothness", 0f);

        for (int i = 0; i < layer.count; i++)
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "Star";
            go.transform.SetParent(layerParent.transform);

            Collider col = go.GetComponent<Collider>();
            if (col != null) Destroy(col);

            float size = Random.Range(layer.minSize, layer.maxSize);
            go.transform.localScale = Vector3.one * size;

            Vector2 randCircle = Random.insideUnitCircle * fieldRadius;
            Vector3 offset = new Vector3(randCircle.x, 0f, randCircle.y);
            go.transform.position = (cameraTransform != null ? cameraTransform.position : Vector3.zero) + offset;
            go.transform.position = new Vector3(go.transform.position.x, -1f - layerIndex * 0.5f, go.transform.position.z);

            float alpha = Random.Range(layer.minAlpha, layer.maxAlpha);

            MeshRenderer mr = go.GetComponent<MeshRenderer>();
            mr.sharedMaterial = sharedMat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            stars.Add(new StarData
            {
                transform     = go.transform,
                renderer      = mr,
                baseOffset    = offset,
                twinkleOffset = Random.Range(0f, Mathf.PI * 2f),
                twinkleSpeed  = layer.twinkleSpeed * Random.Range(0.7f, 1.3f),
                baseAlpha     = alpha,
                color         = Color.white
            });
        }

        return stars;
    }

    void Update()
    {
        if (cameraTransform == null) return;

        Vector3 camPos = cameraTransform.position;
        Vector3 camDelta = camPos - lastCamPos;
        lastCamPos = camPos;

        for (int li = 0; li < layers.Length && li < layerStars.Count; li++)
        {
            float pf = layers[li].parallaxFactor;
            float r = fieldRadius;

            foreach (StarData star in layerStars[li])
            {
                Vector3 pos = star.transform.position;
                pos.x -= camDelta.x * pf;
                pos.z -= camDelta.z * pf;

                Vector3 relative = pos - new Vector3(camPos.x, pos.y, camPos.z);
                if (relative.magnitude > r)
                {
                    Vector2 rand = Random.insideUnitCircle.normalized * r * Random.Range(0.85f, 1f);
                    pos.x = camPos.x + rand.x;
                    pos.z = camPos.z + rand.y;
                }

                star.transform.position = pos;
            }
        }
    }
}
