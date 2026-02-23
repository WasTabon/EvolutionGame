using UnityEngine;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    public float minInterval = 25f;
    public float maxInterval = 45f;
    public GameBalanceConfig balanceConfig;

    private float nextEventTime;
    private bool eventRunning;

    private StarStormEvent starStorm;
    private GravitationalWaveEvent gravWave;
    private HunterEvent hunter;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (balanceConfig != null)
        {
            minInterval = balanceConfig.eventMinInterval;
            maxInterval = balanceConfig.eventMaxInterval;
        }

        starStorm = gameObject.AddComponent<StarStormEvent>();
        gravWave  = gameObject.AddComponent<GravitationalWaveEvent>();
        hunter    = gameObject.AddComponent<HunterEvent>();

        if (balanceConfig != null)
        {
            starStorm.duration        = balanceConfig.starStormDuration;
            starStorm.spawnMultiplier = balanceConfig.starStormMultiplier;
            gravWave.duration         = balanceConfig.gravWaveDuration;
            gravWave.maxForce         = balanceConfig.gravWaveMaxForce;
            hunter.duration           = balanceConfig.hunterDuration;
            hunter.hunterSpeed        = balanceConfig.hunterSpeed;
            hunter.hunterScale        = balanceConfig.hunterScale;
        }
    }

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing) return;
        if (eventRunning) return;

        if (Time.time >= nextEventTime)
        {
            eventRunning = true;
            TriggerRandomEvent();
        }
    }

    void TriggerRandomEvent()
    {
        int idx = Random.Range(0, 3);
        switch (idx)
        {
            case 0: starStorm.Begin(); break;
            case 1: gravWave.Begin();  break;
            case 2: hunter.Begin();    break;
        }
    }

    public void OnEventFinished()
    {
        eventRunning = false;
        ScheduleNext();
    }

    void ScheduleNext()
    {
        nextEventTime = Time.time + Random.Range(minInterval, maxInterval);
    }
}
