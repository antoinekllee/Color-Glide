using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    private Queue<T> poolQueue = new Queue<T>();
    private T prefab;

    public ObjectPool(T prefab)
    {
        this.prefab = prefab;
    }

    public T Get()
    {
        if (poolQueue.Count == 0)
        {
            T newItem = GameObject.Instantiate(prefab);
            newItem.gameObject.SetActive(false);
            poolQueue.Enqueue(newItem);
        }

        return poolQueue.Dequeue();
    }

    public void ReturnToPool(T item)
    {
        item.gameObject.SetActive(false);
        poolQueue.Enqueue(item);
    }
}
