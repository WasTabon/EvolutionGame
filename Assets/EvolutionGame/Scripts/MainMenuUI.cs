using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuUI : MonoBehaviour
{
    public Button startButton;
    public CanvasGroup canvasGroup;
    public RectTransform titleTransform;
    public RectTransform startButtonTransform;

    void Start()
    {
        Debug.Assert(startButton != null, "MainMenuUI: startButton not assigned!");
        Debug.Assert(canvasGroup != null, "MainMenuUI: canvasGroup not assigned!");

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.8f).SetEase(Ease.OutQuad);

        if (titleTransform != null)
        {
            titleTransform.localScale = Vector3.one * 0.8f;
            titleTransform.DOScale(1f, 0.6f).SetEase(Ease.OutBack).SetDelay(0.15f).OnComplete(() =>
            {
                titleTransform.DOScale(1.03f, 1.4f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
            });
        }

        if (startButtonTransform != null)
        {
            startButtonTransform.localScale = Vector3.zero;
            startButtonTransform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).SetDelay(0.4f);
        }

        startButton.onClick.AddListener(OnStartClicked);
    }

    void OnStartClicked()
    {
        startButton.interactable = false;
        AudioManager.Instance?.PlayButtonSFX();
        startButton.transform.DOScale(0.9f, 0.08f).SetEase(Ease.InBack).OnComplete(() =>
        {
            startButton.transform.DOScale(1f, 0.07f).OnComplete(() =>
            {
                canvasGroup.DOFade(0f, 0.3f).OnComplete(() =>
                {
                    GameManager.Instance.StartGame();
                });
            });
        });
    }
}
