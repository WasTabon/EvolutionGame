using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameHUD : MonoBehaviour
{
    public Text scoreText;
    public Text stageText;
    public CanvasGroup canvasGroup;
    public Image progressBarFill;
    public TextMeshProUGUI comboText;
    public CanvasGroup comboCG;

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

        if (ComboSystem.Instance != null)
        {
            ComboSystem.Instance.OnComboChanged -= OnComboChanged;
            ComboSystem.Instance.OnComboChanged += OnComboChanged;
            ComboSystem.Instance.OnComboReset -= OnComboReset;
            ComboSystem.Instance.OnComboReset += OnComboReset;
        }
    }

    void OnDisable()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreChanged -= OnScoreChanged;

        if (GameManager.Instance != null)
            GameManager.Instance.OnStateChanged -= OnStateChanged;

        if (ComboSystem.Instance != null)
        {
            ComboSystem.Instance.OnComboChanged -= OnComboChanged;
            ComboSystem.Instance.OnComboReset -= OnComboReset;
        }
    }

    void Start()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.6f).SetDelay(0.2f);

        if (scoreText != null) scoreText.text = "0";
        if (stageText != null) stageText.text = "Spark";
        if (progressBarFill != null) progressBarFill.fillAmount = 0f;
        if (comboCG != null) comboCG.alpha = 0f;
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
        {
            ScoreManager.Instance?.SaveBestScore();
            canvasGroup.DOFade(0f, 0.3f);
        }
    }

    void OnComboChanged(float multiplier, int count)
    {
        if (comboText == null || comboCG == null) return;

        comboText.text = "x" + multiplier.ToString("0.#");
        comboText.color = GetMultiplierColor(multiplier);

        comboCG.DOKill();
        comboCG.DOFade(1f, 0.15f);

        comboText.transform.DOKill();
        comboText.transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 1, 0.5f);
    }

    void OnComboReset()
    {
        if (comboCG == null) return;
        comboCG.DOKill();
        comboCG.DOFade(0f, 0.4f);
    }

    Color GetMultiplierColor(float mult)
    {
        if (mult >= 3f)  return new Color(1f, 0.4f, 1f);
        if (mult >= 2.5f) return new Color(1f, 0.55f, 0.1f);
        if (mult >= 2f)  return new Color(1f, 0.85f, 0.1f);
        if (mult >= 1.5f) return new Color(0.5f, 1f, 0.5f);
        return Color.white;
    }

    public void SetStageText(string stageName)
    {
        if (stageText == null) return;
        stageText.text = stageName;
        stageText.transform.DOKill();
        stageText.transform.DOPunchScale(Vector3.one * 0.3f, 0.25f, 1, 0.5f);
    }

    public void SetProgress(float value)
    {
        if (progressBarFill == null) return;
        progressBarFill.DOKill();
        progressBarFill.DOFillAmount(Mathf.Clamp01(value), 0.3f).SetEase(Ease.OutQuad);
    }
}
