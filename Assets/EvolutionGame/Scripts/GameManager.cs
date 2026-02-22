using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Menu, Playing, GameOver }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState CurrentState { get; private set; }

    public System.Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "Game")
            SetState(GameState.Playing);
        else
            SetState(GameState.Menu);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game")
            SetState(GameState.Playing);
        else if (scene.name == "MainMenu")
            SetState(GameState.Menu);
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
        OnStateChanged?.Invoke(state);
    }

    public void StartGame()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("Game");
        else
            SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    public void RestartGame()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("Game");
        else
            SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        if (SceneTransition.Instance != null)
            SceneTransition.Instance.LoadScene("MainMenu");
        else
            SceneManager.LoadScene("MainMenu");
    }
}
