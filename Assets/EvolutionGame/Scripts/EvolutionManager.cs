using UnityEngine;
using DG.Tweening;

public class EvolutionManager : MonoBehaviour
{
    public static EvolutionManager Instance;

    public EvolutionConfig config;

    private int currentStageIndex;
    private PlayerController player;
    private TrailRenderer trailRenderer;
    private MeshRenderer playerRenderer;

    public System.Action<int, EvolutionStageData> OnStageChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Debug.Assert(config != null, "EvolutionManager: config not assigned!");
        Debug.Assert(config.stages != null && config.stages.Length > 0, "EvolutionManager: stages are empty!");

        player = Object.FindObjectOfType<PlayerController>();
        if (player != null)
        {
            trailRenderer = player.GetComponentInChildren<TrailRenderer>();
            playerRenderer = player.GetComponent<MeshRenderer>();
        }

        currentStageIndex = 0;
        ApplyStage(currentStageIndex, false);
        UpdateHUD();
    }

    public void OnPlayerScaleChanged(float newScale)
    {
        int nextIndex = currentStageIndex + 1;
        if (nextIndex >= config.stages.Length) return;

        if (newScale >= config.stages[nextIndex].scaleThreshold)
        {
            currentStageIndex = nextIndex;
            ApplyStage(currentStageIndex, true);
            UpdateHUD();
        }
        else
        {
            UpdateProgressBar(newScale);
        }
    }

    void ApplyStage(int index, bool animated)
    {
        EvolutionStageData stage = config.stages[index];

        if (playerRenderer != null && stage.playerMaterial != null)
            playerRenderer.material = stage.playerMaterial;

        if (player != null && stage.playerMesh != null)
        {
            MeshFilter mf = player.GetComponent<MeshFilter>();
            if (mf != null) mf.mesh = stage.playerMesh;
        }

        if (playerRenderer != null && playerRenderer.material != null)
        {
            Material mat = playerRenderer.material;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", stage.trailColor * stage.emissionIntensity);
            }
        }

        if (trailRenderer != null)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] {
                    new GradientColorKey(stage.trailColor, 0f),
                    new GradientColorKey(stage.trailColor, 1f)
                },
                new GradientAlphaKey[] {
                    new GradientAlphaKey(0.8f, 0f),
                    new GradientAlphaKey(0f, 1f)
                }
            );
            trailRenderer.colorGradient = gradient;
            trailRenderer.widthMultiplier = stage.trailWidth;
        }

        if (animated)
        {
            OnStageChanged?.Invoke(index, stage);
            AudioManager.Instance?.PlayEvolutionSFX();

            if (player != null)
                player.transform.DOPunchScale(Vector3.one * 0.35f, 0.4f, 6, 0.5f);
        }
    }

    void UpdateHUD()
    {
        GameHUD hud = Object.FindObjectOfType<GameHUD>();
        if (hud == null) return;

        EvolutionStageData stage = config.stages[currentStageIndex];
        hud.SetStageText(stage.stageName);
        UpdateProgressBar(player != null ? player.GetCurrentScale() : 0f);
    }

    void UpdateProgressBar(float currentScale)
    {
        GameHUD hud = Object.FindObjectOfType<GameHUD>();
        if (hud == null) return;

        int nextIndex = currentStageIndex + 1;
        if (nextIndex >= config.stages.Length)
        {
            hud.SetProgress(1f);
            return;
        }

        float currentThreshold = config.stages[currentStageIndex].scaleThreshold;
        float nextThreshold = config.stages[nextIndex].scaleThreshold;
        float progress = Mathf.InverseLerp(currentThreshold, nextThreshold, currentScale);
        hud.SetProgress(progress);
    }

    public int GetCurrentStageIndex() => currentStageIndex;

    public string GetCurrentStageName()
    {
        if (config == null || config.stages.Length == 0) return "";
        return config.stages[Mathf.Clamp(currentStageIndex, 0, config.stages.Length - 1)].stageName;
    }
}
