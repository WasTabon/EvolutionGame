using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    public Text scoreText;
    public Text bestScoreText;
    public Button restartButton;
    public Button menuButton;
    public CanvasGroup canvasGroup;
    public RectTransform panel;
    public Text stageReachedText;

    void OnEnable()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnStateChanged;
            GameManager.Instance.OnStateChanged += OnStateChanged;
        }
    }

    void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    void Start()
    {
        Debug.Assert(restartButton != null, "GameOverUI: restartButton not assigned!");
        Debug.Assert(menuButton != null, "GameOverUI: menuButton not assigned!");

        restartButton.onClick.AddListener(() =>
        {
            restartButton.interactable = false;
            menuButton.interactable = false;
            AudioManager.Instance?.PlayButtonSFX();
            GameManager.Instance.RestartGame();
        });

        menuButton.onClick.AddListener(() =>
        {
            restartButton.interactable = false;
            menuButton.interactable = false;
            AudioManager.Instance?.PlayButtonSFX();
            GameManager.Instance.GoToMenu();
        });
    }

    void OnStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
            ShowGameOver();
    }

    void ShowGameOver()
    {
        float score = ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0f;
        float best = ScoreManager.Instance != null ? ScoreManager.Instance.GetBestScore() : 0f;

        if (stageReachedText != null && EvolutionManager.Instance != null)
        {
            int idx = EvolutionManager.Instance.GetCurrentStageIndex();
            string stageName = EvolutionManager.Instance.GetCurrentStageName();
            stageReachedText.text = stageName.ToUpper();
        }

        if (bestScoreText != null) bestScoreText.text = Mathf.RoundToInt(best).ToString();

        if (panel != null) panel.localScale = Vector3.one * 0.6f;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, 0.4f).SetDelay(0.3f);

        if (panel != null)
            panel.DOScale(1f, 0.45f).SetEase(Ease.OutBack).SetDelay(0.3f);

        if (scoreText != null)
        {
            float displayScore = 0f;
            DOTween.To(() => displayScore, x =>
            {
                displayScore = x;
                scoreText.text = Mathf.RoundToInt(displayScore).ToString();
            }, score, 1.2f).SetEase(Ease.OutQuart).SetDelay(0.5f);
        }
    }
}
