using UnityEngine;
using System;
using Random = UnityEngine.Random;
using MyBox;

public class Obstacle : MonoBehaviour
{
    [Header ("Global")]
    [SerializeField, AutoProperty] protected ObstaclePart[] parts = null;
    [Space (8)]
    [SerializeField] private float minYOffset = -3f;
    [SerializeField] private float maxYOffset = 3f; 
    [Space(8)]
    [SerializeField] private bool shouldRotate = false; 
    [ConditionalField(nameof(shouldRotate))] public float minRotateSpeed = 10f;
    [ConditionalField(nameof(shouldRotate))] public float maxRotateSpeed = 20f;
    private float rotateSpeed = 0f;
    // [Space(8)]
    // [SerializeField] private bool shouldMove = false;
    // [ConditionalField(nameof(shouldMove))] public float minMoveSpeed = 1f;
    // [ConditionalField(nameof(shouldMove))] public float maxMoveSpeed = 2f;
    // private float moveSpeed = 0f;

    [NonSerialized] protected GameManager gameManager = null; 

    protected virtual void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();
        
        SetupObstacle();

        if (shouldRotate)
        {
            rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
            if (Random.Range(0, 2) == 0)
                rotateSpeed *= -1f;
        }
        
        // if (shouldMove)
        //     moveSpeed = Random.Range(minMoveSpeed, maxMoveSpeed);
    }

    protected virtual void SetupObstacle()
    {
        float yOffset = Random.Range(minYOffset, maxYOffset);
        transform.position = new Vector2(transform.position.x, yOffset);

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

        // if (shouldMove)
        //     transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    public void ResetParts()
    {
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i].ResetPart();
        }
    }
}