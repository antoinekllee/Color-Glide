using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectPool<T> where T : Component
{
    private Queue<T> poolQueue = new Queue<T>();
    private T prefab;
    private Func<T> initFunction;

    public ObjectPool(T prefab)
    {
        this.prefab = prefab;
    }

    public ObjectPool(Func<T> initFunction)
    {
        this.initFunction = initFunction;
    }

    public T Get()
    {
        if (poolQueue.Count == 0)
        {
            if (prefab != null)
            {
                T newItem = GameObject.Instantiate(prefab);
                newItem.gameObject.SetActive(false);
                poolQueue.Enqueue(newItem);
            }
            else if (initFunction != null)
            {
                T newItem = initFunction();
                poolQueue.Enqueue(newItem);
            }
        }

        return poolQueue.Dequeue();
    }

    public void ReturnToPool(T item)
    {
        item.gameObject.SetActive(false);
        poolQueue.Enqueue(item);
    }
}