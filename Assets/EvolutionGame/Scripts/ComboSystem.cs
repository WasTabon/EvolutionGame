using UnityEngine;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance;

    public float comboResetTime = 2.5f;
    public GameBalanceConfig balanceConfig;

    private int comboCount;
    private float lastAbsorptionTime;

    private static readonly float[] multipliers = { 1f, 1.5f, 2f, 2.5f, 3f };

    public System.Action<float, int> OnComboChanged;
    public System.Action OnComboReset;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (balanceConfig != null) comboResetTime = balanceConfig.comboResetTime;
    }

    void Update()
    {
        if (comboCount > 0 && Time.time - lastAbsorptionTime > comboResetTime)
            ResetCombo();
    }

    public float RegisterAbsorption()
    {
        lastAbsorptionTime = Time.time;
        comboCount = Mathf.Min(comboCount + 1, multipliers.Length - 1);

        float mult = GetMultiplier();
        OnComboChanged?.Invoke(mult, comboCount);
        return mult;
    }

    public float GetMultiplier()
    {
        return multipliers[Mathf.Clamp(comboCount, 0, multipliers.Length - 1)];
    }

    void ResetCombo()
    {
        comboCount = 0;
        OnComboReset?.Invoke();
    }

    public int GetComboCount() => comboCount;
}
