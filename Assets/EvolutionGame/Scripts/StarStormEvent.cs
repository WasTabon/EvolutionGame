using UnityEngine;
using System.Collections;

public class StarStormEvent : MonoBehaviour
{
    public float duration = 10f;
    public float spawnMultiplier = 3f;

    public void Begin()
    {
        EventAnnouncementUI.Instance?.Show("Star Storm", "Incoming debris field!", new Color(0.4f, 0.8f, 1f));
        StartCoroutine(RunEvent());
    }

    IEnumerator RunEvent()
    {
        SpawnManager.Instance?.SetSpawnMultiplier(spawnMultiplier);
        yield return new WaitForSeconds(duration);
        SpawnManager.Instance?.SetSpawnMultiplier(1f);
        GameEventManager.Instance?.OnEventFinished();
    }
}
