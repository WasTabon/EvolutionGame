using UnityEngine;

[CreateAssetMenu(fileName = "GameBalanceConfig", menuName = "EvolutionGame/GameBalanceConfig")]
public class GameBalanceConfig : ScriptableObject
{
    [Header("Player Movement")]
    public float baseSpeed = 5f;
    public float minSpeed = 1.5f;
    public float speedScalePenalty = 0.4f;
    public float inertiaSmoothing = 8f;

    [Header("Absorption")]
    public float absorptionThreshold = 0.9f;
    public float deathThreshold = 1.1f;

    [Header("Spawn")]
    public float spawnRadius = 18f;
    public float despawnRadius = 28f;
    public float baseSpawnInterval = 0.8f;
    public int baseMaxObjects = 35;

    [Header("Combo")]
    public float comboResetTime = 2.5f;

    [Header("Events")]
    public float eventMinInterval = 25f;
    public float eventMaxInterval = 45f;
    public float starStormDuration = 10f;
    public float starStormMultiplier = 3f;
    public float gravWaveDuration = 6f;
    public float gravWaveMaxForce = 4f;
    public float hunterDuration = 15f;
    public float hunterSpeed = 2.5f;
    public float hunterScale = 2.2f;
}
