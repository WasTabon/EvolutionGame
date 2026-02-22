using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EvolutionStageTransition : MonoBehaviour
{
    public CanvasGroup flashOverlay;
    public TextMeshProUGUI stageNameText;
    public TextMeshProUGUI stageSubtitleText;
    public CanvasGroup textGroup;

    void OnEnable()
    {
        if (EvolutionManager.Instance != null)
        {
            EvolutionManager.Instance.OnStageChanged -= OnStageChanged;
            EvolutionManager.Instance.OnStageChanged += OnStageChanged;
        }
    }

    void OnDisable()
    {
        if (EvolutionManager.Instance != null)
            EvolutionManager.Instance.OnStageChanged -= OnStageChanged;
    }

    void Start()
    {
        Debug.Assert(flashOverlay != null, "EvolutionStageTransition: flashOverlay not assigned!");
        Debug.Assert(stageNameText != null, "EvolutionStageTransition: stageNameText not assigned!");

        flashOverlay.alpha = 0f;
        textGroup.alpha = 0f;
    }

    void OnStageChanged(int index, EvolutionStageData stage)
    {
        PlayTransition(stage);
    }

    void PlayTransition(EvolutionStageData stage)
    {
        stageNameText.text = stage.stageName.ToUpper();
        if (stageSubtitleText != null)
            stageSubtitleText.text = "EVOLUTION";

        stageNameText.color = stage.trailColor;

        flashOverlay.DOKill();
        textGroup.DOKill();
        stageNameText.rectTransform.DOKill();

        flashOverlay.alpha = 0.55f;
        flashOverlay.DOFade(0f, 0.6f).SetEase(Ease.OutQuad);

        textGroup.alpha = 0f;
        stageNameText.rectTransform.anchoredPosition = Vector2.zero;

        textGroup.DOFade(1f, 0.2f).OnComplete(() =>
        {
            stageNameText.rectTransform.DOAnchorPosY(60f, 0.9f).SetEase(Ease.OutQuad);
            textGroup.DOFade(0f, 0.9f).SetDelay(0.3f);
        });
    }
}
