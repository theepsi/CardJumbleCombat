using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [Serializable]
    public struct ObjectPool
    {
        public GameObject PooledObject;
        public List<GameObject> Pool;
        public int PooledAmount;

        public void ObjectPooler()
        {
            Pool = new List<GameObject>();
        }
    }

    public List<ObjectPool> PooledObjects;

    private Dictionary<string, ObjectPool> _pools;

    void Awake()
    {
        // Singleton
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(this);

        // Initialize pools
        _pools = new Dictionary<string, ObjectPool>();

        for (int i = 0; i < PooledObjects.Count; ++i)
        {
            //create container for pooled opbjects
            for (int j = 0; j < PooledObjects[i].PooledAmount; ++j)
            {
                GameObject go = Instantiate(PooledObjects[i].PooledObject);
                go.SetActive(false);
                PooledObjects[i].Pool.Add(go);
            }
            _pools.Add(PooledObjects[i].PooledObject.name, PooledObjects[i]);
        }
    }

    public GameObject GetPooledObject(string type)
    {
        for (int i = 0; i < _pools[type].Pool.Count; ++i)
        {
            if (!_pools[type].Pool[i].activeInHierarchy)
            {
                return _pools[type].Pool[i];
            }
        }
        GameObject go = Instantiate(_pools[type].PooledObject);
        go.SetActive(false);
        _pools[type].Pool.Add(go);
        return go;
    }
}
