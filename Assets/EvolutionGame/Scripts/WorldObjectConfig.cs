using UnityEngine;

public enum WorldObjectType { Small, Medium, Large }

[CreateAssetMenu(fileName = "WorldObjectConfig", menuName = "EvolutionGame/WorldObjectConfig")]
public class WorldObjectConfig : ScriptableObject
{
    public WorldObjectType type;
    public float scale = 0.5f;
    public float points = 10f;
    public float growthAmount = 0.05f;
    public float moveSpeed = 1f;
    public Mesh mesh;
    public Material material;
}
