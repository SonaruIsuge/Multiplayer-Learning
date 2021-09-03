using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI.Spawning;
using MLAPI;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject poolItem = null;
    [SerializeField] private int maxPoolAmount = 10;

    private Queue<GameObject> pool;
    
    [HideInInspector] public static ObjectPool Instance;
    
    private void Awake()
    {
        // Singleton
        if(Instance == null)
            Instance = this;
        else
            Destroy(this);

        pool = new Queue<GameObject>();

        for(int i = 0; i < maxPoolAmount; i++)
        {
            GameObject go = Instantiate(poolItem);
            go.SetActive(false);
            pool.Enqueue(go);
        }

    }

    private void Start()
    {
        NetworkSpawnManager.RegisterSpawnHandler(NetworkSpawnManager.GetPrefabHashFromGenerator("TestWeapon"), (postition, rotation) => 
        {
            return FetchObject(postition, rotation);
        });

        NetworkSpawnManager.RegisterDestroyHandler(NetworkSpawnManager.GetPrefabHashFromGenerator("TestWeapon"), (NetworkObject) => 
        {
            PoolObject(NetworkObject.gameObject);
        });
    }

    private NetworkObject FetchObject(Vector3 position, Quaternion rotation)
    {
        Debug.Log("Fetching from pool");

        GameObject go;
        if(pool.Count > 0)
        {
            go = pool.Dequeue();
            go.SetActive(true);
        }
        else
        {
            return null;
        }
        go.transform.position = position;
        go.transform.rotation = rotation;

        return go.GetComponent<NetworkObject>();
    }

    private NetworkObject FetchObject()
    {
        Debug.Log("Fetching from pool");

        GameObject go;
        if(pool.Count > 0)
        {
            go = pool.Dequeue();
            go.SetActive(true);
        }
        else
        {
            return null;
        }

        return go.GetComponent<NetworkObject>();
    }

    private void PoolObject(GameObject go)
    {
        Debug.Log("Putting back in pool");
        go.SetActive(false);
        pool.Enqueue(go);
    }

    public GameObject FetchFromPool()
    {
        NetworkObject no = FetchObject();
        no.Spawn();

        return no.gameObject;
    }

    public GameObject FetchFromPoolWithOwnership(ulong netId)
    {
        NetworkObject no = FetchObject();
        no.SpawnWithOwnership(netId);

        return no.gameObject;
    }

    public void ReturnToPool(GameObject go)
    {
        PoolObject(go);
        go.GetComponent<NetworkObject>().Despawn();
    }

}
