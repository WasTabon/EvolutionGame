using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class SettingsPanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Button closeButton;
    public Button bgButton;

    void OnEnable()
    {
        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
            closeButton.onClick.AddListener(Hide);
        }

        if (bgButton != null)
        {
            bgButton.onClick.RemoveListener(Hide);
            bgButton.onClick.AddListener(Hide);
        }

        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    void Start()
    {
        Debug.Assert(canvasGroup != null, "SettingsPanel: canvasGroup not assigned!");
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.DOKill();
        canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.OutQuad);
        transform.DOKill();
        transform.localScale = Vector3.one * 0.9f;
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
    }

    public void Hide()
    {
        canvasGroup.DOKill();
        canvasGroup.DOFade(0f, 0.2f).SetEase(Ease.InQuad).OnComplete(() =>
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        });
        transform.DOKill();
        transform.DOScale(0.9f, 0.2f).SetEase(Ease.InQuad);
    }

    void OnMusicChanged(float val)
    {
        PlayerPrefs.SetFloat("MusicVolume", val);
        AudioManager.Instance?.SetMusicVolume(val);
    }

    void OnSFXChanged(float val)
    {
        PlayerPrefs.SetFloat("SFXVolume", val);
        AudioManager.Instance?.SetSFXVolume(val);
    }
}

