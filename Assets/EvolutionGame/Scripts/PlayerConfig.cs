using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "EvolutionGame/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    public float baseSpeed = 5f;
    public float minSpeed = 1.5f;
    public float speedScalePenalty = 0.4f;
    public float inertiaSmoothing = 8f;
    public float baseScale = 0.5f;
    public Mesh mesh;
    public Material material;
}
