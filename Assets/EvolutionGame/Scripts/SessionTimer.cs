using UnityEngine;

public class SessionTimer : MonoBehaviour
{
    public static SessionTimer Instance;

    private float startTime;
    private float endTime;
    private bool running;

    public System.Action<float> OnTick;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnEnable()
    {
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

    void OnStateChanged(GameState state)
    {
        if (state == GameState.Playing)
        {
            startTime = Time.time;
            running = true;
        }
        else if (state == GameState.GameOver)
        {
            endTime = Time.time;
            running = false;
        }
    }

    void Update()
    {
        if (!running) return;
        OnTick?.Invoke(GetElapsed());
    }

    public float GetElapsed() => running ? Time.time - startTime : endTime - startTime;

    public string GetFormatted()
    {
        float t = GetElapsed();
        int minutes = Mathf.FloorToInt(t / 60f);
        int seconds = Mathf.FloorToInt(t % 60f);
        return string.Format("{0}:{1:00}", minutes, seconds);
    }
}
