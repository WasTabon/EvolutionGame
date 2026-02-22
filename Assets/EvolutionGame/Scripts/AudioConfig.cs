using UnityEngine;

[CreateAssetMenu(fileName = "AudioConfig", menuName = "EvolutionGame/AudioConfig")]
public class AudioConfig : ScriptableObject
{
    [Header("Music")]
    public AudioClip menuMusic;
    public AudioClip gameMusic;
    public AudioClip gameOverMusic;

    [Header("SFX - Absorption")]
    public AudioClip[] absorptionSFX;

    [Header("SFX - Player")]
    public AudioClip growSFX;
    public AudioClip evolutionSFX;
    public AudioClip deathSFX;

    [Header("SFX - UI")]
    public AudioClip buttonClickSFX;
}
