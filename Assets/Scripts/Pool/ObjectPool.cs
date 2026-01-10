using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    private GameObject prefab;
    private Transform parentTransform;

    public void Initialize(GameObject prefab, int size, Transform parent)
    {
        this.prefab = prefab;
        this.parentTransform = parent != null ? parent : transform;
        pool.Clear();
        for (int i = 0; i < Mathf.Max(1, size); i++) CreateNew();
    }

    private void CreateNew()
    {
        GameObject obj = GameObject.Instantiate(prefab, parentTransform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0) CreateNew();
        GameObject obj = pool.Dequeue();
        obj.transform.SetParent(parentTransform);
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