using UnityEngine;

public class ModelInitializer : MonoBehaviour
{
    public ModelConfig modelConfig;

    public WorldObjectConfig smallConfig;
    public WorldObjectConfig mediumConfig;
    public WorldObjectConfig largeConfig;

    public EvolutionConfig evolutionConfig;

    void Awake()
    {
        if (modelConfig == null) return;

        ApplyWorldObjectModels();
        ApplyPlayerModels();
    }

    void ApplyWorldObjectModels()
    {
        ApplyToWorldConfig(smallConfig,  modelConfig.smallMesh,  modelConfig.smallMaterial);
        ApplyToWorldConfig(mediumConfig, modelConfig.mediumMesh, modelConfig.mediumMaterial);
        ApplyToWorldConfig(largeConfig,  modelConfig.largeMesh,  modelConfig.largeMaterial);
    }

    void ApplyToWorldConfig(WorldObjectConfig cfg, Mesh mesh, Material mat)
    {
        if (cfg == null) return;
        if (mesh != null) cfg.mesh = mesh;
        if (mat != null)  cfg.material = mat;
    }

    void ApplyPlayerModels()
    {
        if (evolutionConfig == null) return;

        for (int i = 0; i < evolutionConfig.stages.Length && i < modelConfig.stageModels.Length; i++)
        {
            StageModel sm = modelConfig.stageModels[i];
            if (sm == null) continue;

            if (sm.mesh != null)     evolutionConfig.stages[i].playerMesh = sm.mesh;
            if (sm.material != null) evolutionConfig.stages[i].playerMaterial = sm.material;
        }
    }
}
