using UnityEngine;
using System;
using Random = UnityEngine.Random;
using MyBox;

public class Obstacle : MonoBehaviour
{
    [SerializeField, AutoProperty] private ObstaclePart[] parts = null;

    private GameManager gameManager = null; 

    private void Start ()
    {
        int[] ids = new int[] { 1, 2, 3 };
        Array.Sort(ids, (a, b) => Random.Range(-1, 2));

        foreach (ObstaclePart part in parts)
        {
            int index = Array.IndexOf(ids, part.id);
            part.SetObjectColour((ObjectColour)index);
        }
    }
}
