using UnityEngine;
using System.Collections.Generic;

public class WormPool : MonoBehaviour
{
    [Header("Worm Prefabs")]
    [SerializeField] private GameObject normalWormPrefab;
    [SerializeField] private GameObject poisonousWormPrefab;
    [SerializeField] private GameObject vainPrefab;

    [Header("Worm Textures (For Normal Worms Only)")]
    public Texture2D[] wormTextures;

    [Header("Pool Size")]
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> normalWormPool = new Queue<GameObject>();
    private Queue<GameObject> poisonousWormPool = new Queue<GameObject>();
    private Queue<GameObject> vainPool = new Queue<GameObject>();

    [Header("Spawn Probabilities")]
    [Range(0f, 1f)] public float normalWormProbability = 0.7f;
    [Range(0f, 1f)] public float poisonousWormProbability = 0.2f;
    [Range(0f, 1f)] public float vainProbability = 0.1f;

    public static WormPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject normal = Instantiate(normalWormPrefab);
            ApplyRandomTexture(normal);
            normal.SetActive(false);
            normalWormPool.Enqueue(normal);

            GameObject poisonous = Instantiate(poisonousWormPrefab);
            poisonous.SetActive(false);
            poisonousWormPool.Enqueue(poisonous);

            GameObject vain = Instantiate(vainPrefab);
            vain.SetActive(false);
            vainPool.Enqueue(vain);
        }
    }

    public GameObject GetWorm()
    {
        float total = normalWormProbability + poisonousWormProbability + vainProbability;
        float rand = Random.value;

        float normNormal = normalWormProbability / total;
        float normPoison = poisonousWormProbability / total;

        if (rand < normNormal)
        {
            return GetFromPoolOrNew(normalWormPool, normalWormPrefab, wormTextures);
        }
        else if (rand < normNormal + normPoison)
        {
            return GetFromPoolOrNew(poisonousWormPool, poisonousWormPrefab);
        }
        else
        {
            return GetFromPoolOrNew(vainPool, vainPrefab);
        }
    }

    private GameObject GetFromPoolOrNew(Queue<GameObject> pool, GameObject prefab, Texture2D[] textures = null)
    {
        GameObject worm = null;

        while (pool.Count > 0)
        {
            worm = pool.Dequeue();
            if (worm != null)
                break; // valid worm found
        }

        if (worm == null)
        {
            worm = Instantiate(prefab);
        }

        if (textures != null && textures.Length > 0)
        {
            ApplyRandomTexture(worm);
        }

        worm.SetActive(true);
        return worm;
    }

    private void ApplyRandomTexture(GameObject worm)
    {
        if (wormTextures == null || wormTextures.Length == 0) return;

        int randomIndex = Random.Range(0, wormTextures.Length);
        Renderer wormRenderer = worm.GetComponentInChildren<Renderer>();
        if (wormRenderer != null)
        {
            wormRenderer.material.mainTexture = wormTextures[randomIndex];
        }
    }

    public void ReturnWorm(GameObject worm)
    {
        if (worm == null) return;

        worm.SetActive(false);

        if (worm.CompareTag("PoisonousWorm"))
        {
            poisonousWormPool.Enqueue(worm);
        }
        else if (worm.CompareTag("Vain"))
        {
            vainPool.Enqueue(worm);
        }
        else
        {
            normalWormPool.Enqueue(worm);
        }
    }
}
