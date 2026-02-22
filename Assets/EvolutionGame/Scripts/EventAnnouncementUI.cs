using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EventAnnouncementUI : MonoBehaviour
{
    public static EventAnnouncementUI Instance;

    public CanvasGroup canvasGroup;
    public RectTransform panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subtitleText;
    public Image accentLine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Debug.Assert(canvasGroup != null, "EventAnnouncementUI: canvasGroup not assigned!");
        canvasGroup.alpha = 0f;
        if (panel != null) panel.anchoredPosition = new Vector2(0f, 120f);
    }

    public void Show(string title, string subtitle, Color accentColor)
    {
        if (titleText != null) titleText.text = title.ToUpper();
        if (subtitleText != null) subtitleText.text = subtitle.ToUpper();
        if (accentLine != null) accentLine.color = accentColor;

        canvasGroup.DOKill();
        panel?.DOKill();

        if (panel != null) panel.anchoredPosition = new Vector2(0f, 120f);
        canvasGroup.alpha = 0f;

        canvasGroup.DOFade(1f, 0.3f);
        panel?.DOAnchorPosY(0f, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, Hide);
        });
    }

    public void Hide()
    {
        canvasGroup.DOKill();
        panel?.DOKill();

        canvasGroup.DOFade(0f, 0.3f);
        panel?.DOAnchorPosY(120f, 0.3f).SetEase(Ease.InBack);
    }
}
