using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class OnboardingHint : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    private const string ShownKey = "OnboardingShown";

    void Start()
    {
        Debug.Assert(canvasGroup != null, "OnboardingHint: canvasGroup not assigned!");

        if (PlayerPrefs.GetInt(ShownKey, 0) == 1)
        {
            gameObject.SetActive(false);
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.DOFade(1f, 0.5f).SetDelay(1f);
    }

    void Update()
    {
        if (!gameObject.activeSelf) return;

        bool tapped = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        if (tapped)
            Dismiss();
    }

    void Dismiss()
    {
        PlayerPrefs.SetInt(ShownKey, 1);
        PlayerPrefs.Save();

        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.3f).OnComplete(() => gameObject.SetActive(false));
    }
}
