using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    public Text scoreText;
    public Text stageText;
    public CanvasGroup canvasGroup;

    private int displayedScore;

    void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;
            ScoreManager.Instance.OnScoreChanged += OnScoreChanged;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStateChanged -= OnStateChanged;
            GameManager.Instance.OnStateChanged += OnStateChanged;
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;
    }

    void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.6f).SetDelay(0.2f);

        if (scoreText != null) scoreText.text = "0";
        if (stageText != null) stageText.text = "Spark";
    }

    void OnScoreChanged(float score)
    {
        if (scoreText == null) return;

        scoreText.text = Mathf.RoundToInt(score).ToString();
        scoreText.transform.DOKill();
        scoreText.transform.DOPunchScale(Vector3.one * 0.25f, 0.18f, 1, 0.5f);
    }

    void OnStateChanged(GameState state)
    {
        if (state == GameState.GameOver)
            canvasGroup.DOFade(0f, 0.3f);
    }

    public void SetStageText(string stageName)
    {
        if (stageText == null) return;
        stageText.text = stageName;
        stageText.transform.DOKill();
        stageText.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 1, 0.5f);
    }
}
