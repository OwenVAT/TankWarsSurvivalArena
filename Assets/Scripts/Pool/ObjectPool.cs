using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    Queue<GameObject> pool = new Queue<GameObject>();
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize;
    Transform parentTransform;
    public void Initialize(GameObject prefab, int poolSize, Transform parentTransform = null)
    {
        this.prefab = prefab;
        this.poolSize = poolSize;
        if (parentTransform != null)
        {
            this.parentTransform = parentTransform;
        }
        else
        {
            this.parentTransform = transform;
        }
        
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }
    private void CreateNewObject()
    {
        GameObject obj = Instantiate(prefab);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
    public GameObject GetObject() 
    {  
        if (pool.Count == 0)
        {
            CreateNewObject();
        }
        GameObject obj = pool.Dequeue();
        obj.SetActive(true);
        return obj; 
    }
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(parentTransform);
        pool.Enqueue(obj);
    }
}