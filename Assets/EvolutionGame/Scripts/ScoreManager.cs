using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    private float score;

    public System.Action<float> OnScoreChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        score = 0;
    }

    public void AddScore(float amount)
    {
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    public float GetScore() => score;
}
