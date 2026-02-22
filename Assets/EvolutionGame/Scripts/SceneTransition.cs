using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    private Image overlay;
    private bool isTransitioning;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureOverlay();
        FadeIn();
    }

    void EnsureOverlay()
    {
        if (overlay != null) return;

        GameObject canvasGo = new GameObject("TransitionCanvas");
        canvasGo.transform.SetParent(transform);
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;
        canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        GameObject overlayGo = new GameObject("Overlay");
        overlayGo.transform.SetParent(canvasGo.transform, false);
        RectTransform rect = overlayGo.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        overlay = overlayGo.AddComponent<Image>();
        overlay.color = new Color(0f, 0f, 0f, 1f);
        overlay.raycastTarget = false;
    }

    void FadeIn()
    {
        EnsureOverlay();
        overlay.DOKill();
        overlay.color = new Color(0f, 0f, 0f, 1f);
        overlay.DOFade(0f, 0.4f).SetEase(Ease.OutQuad);
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        EnsureOverlay();
        overlay.DOKill();
        overlay.DOFade(1f, 0.3f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            isTransitioning = false;
            SceneManager.LoadScene(sceneName);
        });
    }
}
