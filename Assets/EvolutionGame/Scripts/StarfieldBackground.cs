using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StarfieldBackground : MonoBehaviour
{
    public int starCount = 80;
    public float minSize = 2f;
    public float maxSize = 6f;
    public float minSpeed = 8f;
    public float maxSpeed = 35f;
    public float minAlpha = 0.15f;
    public float maxAlpha = 0.6f;

    private RectTransform rectTransform;
    private List<StarData> stars = new List<StarData>();
    private Canvas canvas;

    class StarData
    {
        public RectTransform rect;
        public Image image;
        public float speed;
        public float twinkleSpeed;
        public float twinkleOffset;
        public float baseAlpha;
    }

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
    }

    void Start()
    {
        for (int i = 0; i < starCount; i++)
            CreateStar();
    }

    void CreateStar()
    {
        GameObject go = new GameObject("Star");
        go.transform.SetParent(transform, false);

        RectTransform rect = go.AddComponent<RectTransform>();
        float size = Random.Range(minSize, maxSize);
        rect.sizeDelta = new Vector2(size, size);
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.anchoredPosition = GetRandomPosition();

        Image img = go.AddComponent<Image>();
        float alpha = Random.Range(minAlpha, maxAlpha);
        img.color = new Color(1f, 1f, 1f, alpha);
        img.raycastTarget = false;

        StarData star = new StarData
        {
            rect = rect,
            image = img,
            speed = Random.Range(minSpeed, maxSpeed),
            twinkleSpeed = Random.Range(0.5f, 2f),
            twinkleOffset = Random.Range(0f, Mathf.PI * 2f),
            baseAlpha = alpha
        };

        stars.Add(star);
    }

    void Update()
    {
        Vector2 screenSize = GetScreenSize();

        foreach (StarData star in stars)
        {
            Vector2 pos = star.rect.anchoredPosition;
            pos.y -= star.speed * Time.deltaTime;

            if (pos.y < -10f)
            {
                pos.y = screenSize.y + 10f;
                pos.x = Random.Range(0f, screenSize.x);
            }

            star.rect.anchoredPosition = pos;

            float twinkle = Mathf.Sin(Time.time * star.twinkleSpeed + star.twinkleOffset) * 0.15f;
            float alpha = Mathf.Clamp01(star.baseAlpha + twinkle);
            star.image.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    Vector2 GetScreenSize()
    {
        if (canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            return new Vector2(Screen.width, Screen.height) / canvas.scaleFactor;
        return new Vector2(Screen.width, Screen.height);
    }

    Vector2 GetRandomPosition()
    {
        Vector2 size = GetScreenSize();
        return new Vector2(Random.Range(0f, size.x), Random.Range(0f, size.y));
    }
}
