using UnityEngine;

public class WormSpawner : MonoBehaviour
{
    [Header("Spawn Timing")]
    [SerializeField] private float initialSpawnInterval = 2f;
    [SerializeField] private float minSpawnInterval = 0.5f;
    [SerializeField] private float difficultyRampSpeed = 0.01f;

    [Header("Spawn Area")]
    [SerializeField] private Vector2 xSpawnRange = new Vector2(-4f, 4f);
    [SerializeField] private float ySpawnOffset = 1f;

    private float spawnTimer;
    private float currentSpawnInterval;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        currentSpawnInterval = initialSpawnInterval;
    }

    private void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= currentSpawnInterval)
        {
            spawnTimer = 0f;
            SpawnWorm();

            // Gradually reduce spawn interval to ramp up difficulty
            currentSpawnInterval = Mathf.Max(minSpawnInterval, currentSpawnInterval - difficultyRampSpeed * Time.deltaTime);
        }
    }

    private void SpawnWorm()
    {
        float randomX = Random.Range(xSpawnRange.x, xSpawnRange.y);
        float yTop = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 1f, 0)).y + ySpawnOffset;
        Vector3 spawnPos = new Vector3(randomX, yTop, 0f);

        GameObject worm = WormPool.Instance.GetWorm();
        worm.transform.position = spawnPos;
      //  worm.transform.rotation = Quaternion.identity;

        // Reset any worm-specific behaviors like falling
        WormFaller faller = worm.GetComponent<WormFaller>();
        if (faller != null)
        {
            faller.ResetWorm();
        }
    }
}
