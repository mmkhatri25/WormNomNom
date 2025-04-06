using System.Collections.Generic;
using UnityEngine;

public class WormPool : MonoBehaviour
{
    [SerializeField] private GameObject normalWormPrefab;
    [SerializeField] private GameObject poisonousWormPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> normalWormPool = new Queue<GameObject>();
    private Queue<GameObject> poisonousWormPool = new Queue<GameObject>();

    public static WormPool Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            GameObject worm = Instantiate(normalWormPrefab);
            worm.SetActive(false);
            normalWormPool.Enqueue(worm);

            GameObject poisonousWorm = Instantiate(poisonousWormPrefab);
            poisonousWorm.SetActive(false);
            poisonousWormPool.Enqueue(poisonousWorm);
        }
    }

    public GameObject GetWorm()
    {
        float poisonChance = 0.5f; // 2 out of 10 -> poisonous
        if (Random.value < poisonChance)
        {
            if (poisonousWormPool.Count > 0)
            {
                GameObject worm = poisonousWormPool.Dequeue();
                worm.SetActive(true);
                return worm;
            }
            else
            {
                return Instantiate(poisonousWormPrefab);
            }
        }
        else
        {
            if (normalWormPool.Count > 0)
            {
                GameObject worm = normalWormPool.Dequeue();
                worm.SetActive(true);
                return worm;
            }
            else
            {
                return Instantiate(normalWormPrefab);
            }
        }
    }

    public void ReturnWorm(GameObject worm)
    {
        worm.SetActive(false);
        //this.gameObject.GetComponent<Collider>().enabled = true;

        if (worm.CompareTag("PoisonousWorm"))
        {
            poisonousWormPool.Enqueue(worm);
        }
        else
        {
            normalWormPool.Enqueue(worm);
        }
    }
}
