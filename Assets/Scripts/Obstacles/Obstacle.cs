using UnityEngine;
using System;
using Random = UnityEngine.Random;
using MyBox;

public class Obstacle : MonoBehaviour
{
    [Header ("Global")]
    [SerializeField, AutoProperty] protected ObstaclePart[] parts = null;
    [Space(8)]
    [SerializeField] private bool shouldRotate = false; 
    [ConditionalField(nameof(shouldRotate))] public float minRotateSpeed = 10f;
    [ConditionalField(nameof(shouldRotate))] public float maxRotateSpeed = 20f;
    private float rotateSpeed = 0f;
    
    [Header ("Specific")]

    [NonSerialized] protected GameManager gameManager = null; 

    protected virtual void Start ()
    {
        SetupObstacle();

        if (shouldRotate)
            rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
    }

    protected virtual void SetupObstacle()
    {
        gameManager = FindObjectOfType<GameManager>();

        int[] ids = new int[] { 0, 1, 2 };
        Array.Sort(ids, (a, b) => Random.Range(-1, 2));

        foreach (ObstaclePart part in parts)
        {
            int index = Array.IndexOf(ids, part.id);
            part.SetObjectColour((ObjectColour)index);
        }

    }

    private void Update ()
    {
        if (shouldRotate)
            transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
    }
}