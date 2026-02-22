using UnityEngine;
using System.Collections.Generic;

public class ScorePopupPool : MonoBehaviour
{
    public static ScorePopupPool Instance;

    public FloatingScoreText prefab;
    public int initialSize = 10;

    private Queue<FloatingScoreText> pool = new Queue<FloatingScoreText>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Debug.Assert(prefab != null, "ScorePopupPool: prefab not assigned!");
        for (int i = 0; i < initialSize; i++)
            pool.Enqueue(CreateNew());
    }

    public FloatingScoreText Get()
    {
        FloatingScoreText item = pool.Count > 0 ? pool.Dequeue() : CreateNew();
        return item;
    }

    public void Return(FloatingScoreText item)
    {
        item.gameObject.SetActive(false);
        pool.Enqueue(item);
    }

    FloatingScoreText CreateNew()
    {
        FloatingScoreText item = Instantiate(prefab, transform);
        item.gameObject.SetActive(false);
        return item;
    }
}
