using UnityEngine;

[System.Serializable]
public class EvolutionStageData
{
    public string stageName = "Spark";
    public float scaleThreshold = 0.5f;
    public Mesh playerMesh;
    public Material playerMaterial;
    public Color trailColor = Color.white;
    public float trailWidth = 0.6f;
    public float emissionIntensity = 1f;
}

[CreateAssetMenu(fileName = "EvolutionConfig", menuName = "EvolutionGame/EvolutionConfig")]
public class EvolutionConfig : ScriptableObject
{
    public EvolutionStageData[] stages;
}
