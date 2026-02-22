using UnityEngine;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioConfig config;

    private AudioSource musicSource;
    private AudioSource sfxSource;

    private float musicVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.loop = false;
        sfxSource.playOnAwake = false;

        LoadVolumes();
    }

    void LoadVolumes()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
    }

    public void SetMusicVolume(float vol)
    {
        musicVolume = vol;
        musicSource.volume = vol;
        PlayerPrefs.SetFloat("MusicVolume", vol);
    }

    public void SetSFXVolume(float vol)
    {
        sfxVolume = vol;
        sfxSource.volume = vol;
        PlayerPrefs.SetFloat("SFXVolume", vol);
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 0.5f)
    {
        if (clip == null)
        {
            StopMusic(fadeDuration);
            return;
        }

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (musicSource.isPlaying)
        {
            musicSource.DOFade(0f, fadeDuration).OnComplete(() =>
            {
                musicSource.clip = clip;
                musicSource.volume = 0f;
                musicSource.Play();
                musicSource.DOFade(musicVolume, fadeDuration);
            });
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = 0f;
            musicSource.Play();
            musicSource.DOFade(musicVolume, fadeDuration);
        }
    }

    public void StopMusic(float fadeDuration = 0.5f)
    {
        musicSource.DOFade(0f, fadeDuration).OnComplete(() => musicSource.Stop());
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxVolume <= 0f) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayRandomSFX(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;
        AudioClip clip = clips[Random.Range(0, clips.Length)];
        PlaySFX(clip);
    }

    public void PlayMenuMusic()
    {
        if (config != null) PlayMusic(config.menuMusic);
    }

    public void PlayGameMusic()
    {
        if (config != null) PlayMusic(config.gameMusic);
    }

    public void PlayGameOverMusic()
    {
        if (config != null) PlayMusic(config.gameOverMusic);
    }

    public void PlayAbsorptionSFX()
    {
        if (config != null) PlayRandomSFX(config.absorptionSFX);
    }

    public void PlayGrowSFX()
    {
        if (config != null) PlaySFX(config.growSFX);
    }

    public void PlayEvolutionSFX()
    {
        if (config != null) PlaySFX(config.evolutionSFX);
    }

    public void PlayDeathSFX()
    {
        if (config != null) PlaySFX(config.deathSFX);
    }

    public void PlayButtonSFX()
    {
        if (config != null) PlaySFX(config.buttonClickSFX);
    }
}
