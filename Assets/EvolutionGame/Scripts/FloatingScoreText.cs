using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingScoreText : MonoBehaviour
{
    public TextMeshProUGUI text;

    private RectTransform rectTransform;
    private Vector2 startAnchoredPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Show(Vector3 worldPos, float points, float multiplier)
    {
        gameObject.SetActive(true);

        Vector2 screenPos = Camera.main.WorldToScreenPoint(worldPos);
        rectTransform.position = screenPos;
        startAnchoredPos = rectTransform.anchoredPosition;

        string pointStr = "+" + Mathf.RoundToInt(points);
        if (multiplier > 1f)
            pointStr += " x" + multiplier.ToString("0.#");

        text.text = pointStr;
        text.color = GetMultiplierColor(multiplier);

        float scale = Mathf.Lerp(0.85f, 1.4f, (multiplier - 1f) / 2f);
        rectTransform.localScale = Vector3.one * scale * 0.6f;

        text.DOKill();
        rectTransform.DOKill();

        rectTransform.DOAnchorPosY(startAnchoredPos.y + 80f, 0.7f).SetEase(Ease.OutQuad);
        rectTransform.DOScale(scale, 0.15f).SetEase(Ease.OutBack);

        text.DOFade(1f, 0.1f).OnComplete(() =>
        {
            text.DOFade(0f, 0.35f).SetDelay(0.25f).OnComplete(() =>
            {
                ScorePopupPool.Instance.Return(this);
            });
        });
    }

    Color GetMultiplierColor(float mult)
    {
        if (mult >= 3f) return new Color(1f, 0.4f, 1f);
        if (mult >= 2.5f) return new Color(1f, 0.55f, 0.1f);
        if (mult >= 2f) return new Color(1f, 0.85f, 0.1f);
        if (mult >= 1.5f) return new Color(0.5f, 1f, 0.5f);
        return Color.white;
    }
}
