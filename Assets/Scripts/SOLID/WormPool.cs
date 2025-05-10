using System.Collections.Generic;
using UnityEngine;

public class WormPool : MonoBehaviour
{
    [SerializeField] private GameObject normalWormPrefab;
    [SerializeField] private GameObject poisonousWormPrefab;
    [SerializeField] private GameObject vainPrefab;
    [SerializeField] private int poolSize = 20;

    private Queue<GameObject> normalWormPool = new Queue<GameObject>();
    private Queue<GameObject> poisonousWormPool = new Queue<GameObject>();
    private Queue<GameObject> vainPool = new Queue<GameObject>();

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

            GameObject vain= Instantiate(vainPrefab);
            vain.SetActive(false);
            vainPool.Enqueue(vain);
        }
    }

    public GameObject GetWorm()
    {
        float rand = Random.value; // 0.0 to 1.0

        if (rand < 0.5f) // 0.0 - 0.5 (50%)
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
        else if (rand < 0.8f) // 0.5 - 0.8 (30%)
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
        else // 0.8 - 1.0 (20%)
        {
            if (vainPool.Count > 0)
            {
                GameObject worm = vainPool.Dequeue();
                worm.SetActive(true);
                return worm;
            }
            else
            {
                return Instantiate(vainPrefab);
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
