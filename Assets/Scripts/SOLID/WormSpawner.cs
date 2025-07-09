using UnityEngine;
using System.Collections.Generic;

public class WormSpawner : MonoBehaviour
{
    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 1f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float difficultyRampSpeed = 0.01f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 xSpawnRange = new Vector2(-1f, 1f);
    [SerializeField] private float ySpawnOffset = 1f;

    [Header("Spawn Limit")]
    [SerializeField] private int maxVisibleWorms = 15;
    private int currentMaxWorms;

    [Header("Worm Probabilities")]
    [Range(0f, 1f)] public float normalWormProbability = 0.7f;
    [Range(0f, 1f)] public float poisonousWormProbability = 0.2f;
    [Range(0f, 1f)] public float vainProbability = 0.1f;

    private float spawnTimer;
    private float currentSpawnInterval;
    private Camera mainCamera;
    private float wormLimitIncreaseTimer = 0f;
    private List<GameObject> activeWorms = new List<GameObject>();

    private void Start()
    {
        mainCamera = Camera.main;
        currentSpawnInterval = initialSpawnInterval;
        currentMaxWorms = Mathf.CeilToInt(maxVisibleWorms * 0.3f);

        if (WormPool.Instance != null)
        {
            WormPool.Instance.normalWormProbability = normalWormProbability;
            WormPool.Instance.poisonousWormProbability = poisonousWormProbability;
            WormPool.Instance.vainProbability = vainProbability;
        }
    }

    private void Update()
    {
        activeWorms.RemoveAll(w => w == null || !w.activeInHierarchy);

        if (activeWorms.Count < currentMaxWorms)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= currentSpawnInterval)
            {
                spawnTimer = 0f;
                SpawnWorm();
                currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - difficultyRampSpeed);
            }
        }

        wormLimitIncreaseTimer += Time.deltaTime;
        if (wormLimitIncreaseTimer >= 180f)
        {
            wormLimitIncreaseTimer = 0f;
            int increment = Mathf.CeilToInt(maxVisibleWorms * 0.01f);
            currentMaxWorms = Mathf.Min(maxVisibleWorms, currentMaxWorms + increment);
        }
    }

    private void SpawnWorm()
    {
        float randomX = Random.Range(xSpawnRange.x, xSpawnRange.y);
        float yTop = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0)).y + ySpawnOffset;
        Vector3 spawnPos = new Vector3(randomX, yTop, 0f);

        GameObject worm = WormPool.Instance.GetWorm();
        worm.transform.position = spawnPos;

        WormFaller faller = worm.GetComponent<WormFaller>();
        if (faller != null)
        {
            faller.ResetWorm();
        }

        activeWorms.Add(worm);
    }
}
