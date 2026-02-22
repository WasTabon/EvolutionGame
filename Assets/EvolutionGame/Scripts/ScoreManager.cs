using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float score;

    public System.Action<float> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        score = 0;
    }

    public void AddScore(float baseAmount, Vector3 worldPos)
    {
        float multiplier = ComboSystem.Instance != null
            ? ComboSystem.Instance.RegisterAbsorption()
            : 1f;

        float finalAmount = baseAmount * multiplier;
        score += finalAmount;
        OnScoreChanged?.Invoke(score);

        if (ScorePopupPool.Instance != null)
        {
            FloatingScoreText popup = ScorePopupPool.Instance.Get();
            popup.Show(worldPos, finalAmount, multiplier);
        }
    }

    public void AddScore(float amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    public float GetScore() => score;

    public float GetBestScore() => PlayerPrefs.GetFloat("BestScore", 0f);

    public void SaveBestScore()
    {
        if (score > GetBestScore())
        {
            PlayerPrefs.SetFloat("BestScore", score);
            PlayerPrefs.Save();
        }
    }
}
