using UnityEngine;

[CreateAssetMenu(fileName = "ModelConfig", menuName = "EvolutionGame/ModelConfig")]
public class ModelConfig : ScriptableObject
{
    [Header("World Objects")]
    public Mesh smallMesh;
    public Material smallMaterial;

    public Mesh mediumMesh;
    public Material mediumMaterial;

    public Mesh largeMesh;
    public Material largeMaterial;

    [Header("Player per Evolution Stage")]
    public StageModel[] stageModels = new StageModel[5];
}

[System.Serializable]
public class StageModel
{
    public string stageName;
    public Mesh mesh;
    public Material material;
}
