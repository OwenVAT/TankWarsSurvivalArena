using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    private readonly Queue<GameObject> pool = new Queue<GameObject>();

    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;

    private Transform parentTransform;

    public void Initialize(GameObject prefab, int poolSize, Transform parentTransform = null)
    {
        this.prefab = prefab;
        this.poolSize = Mathf.Max(1, poolSize);
        this.parentTransform = parentTransform != null ? parentTransform : transform;

        pool.Clear();
        for (int i = 0; i < this.poolSize; i++)
            CreateNewObject();
    }

    private void CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, parentTransform);
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public GameObject GetObject()
    {
        if (pool.Count == 0) CreateNewObject();

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
