using UnityEngine;
using System;
using Shapes; 
using MyBox;
using Random = UnityEngine.Random; 
using DG.Tweening;

[Serializable]
public class BackgroundShape
{
    public GameObject prefab = null;
    public int weight = 1;
}

public class BackgroundSpawner : MonoBehaviour
{
    [Header ("Spawning")]
    [SerializeField] private BackgroundShape[] shapes = null; 
    [Space (8)]
    [SerializeField, PositiveValueOnly] private float spawnX = 8f; 
    [SerializeField] private float minSpawnY = -5f;
    [SerializeField] private float maxSpawnY = 5f;

    [Header ("Stats")]
    [SerializeField, PositiveValueOnly] private float minRotateSpeed = 10f; 
    [SerializeField, PositiveValueOnly] private float maxRotateSpeed = 20f;
    [Space (8)]
    [SerializeField, PositiveValueOnly] private float minSize = 0.5f;
    [SerializeField, PositiveValueOnly] private float maxSize = 1.5f;
    [Space (8)]
    [SerializeField, PositiveValueOnly] private float minSpeed = 0.5f;
    [SerializeField, PositiveValueOnly] private float maxSpeed = 1.5f;

    [Header ("Timing")]   
    [SerializeField, PositiveValueOnly] private float minSpawnInterval = 0.5f;
    [SerializeField, PositiveValueOnly] private float maxSpawnInterval = 1.5f;
    private float spawnInterval = 0f;
    private float spawnTimer = 0f;

    private void Start ()
    {
        spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);

        // Spawn initial shapes to fill the screen
        for (int i = 0; i < 10; i++)
        {
            float startSpawnX = Random.Range(-spawnX / 2f, spawnX / 2f); // spawn shapes in the middle of the screen (ish
            float spawnY = Random.Range(minSpawnY, maxSpawnY);
            Vector2 spawnPos = new Vector2(startSpawnX, spawnY);
            SpawnShape(spawnPos);
        }
    }

    private void Update ()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            float spawnY = Random.Range(minSpawnY, maxSpawnY);
            Vector2 spawnPos = new Vector2(spawnX, spawnY);
            SpawnShape(spawnPos);

            spawnTimer -= spawnInterval;
            spawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnShape (Vector2 spawnPos)
    {
        int totalWeight = 0;
        foreach (BackgroundShape shape in shapes)
            totalWeight += shape.weight;

        int randomWeight = Random.Range(0, totalWeight);

        BackgroundShape shapeToSpawn = null;
        int currentWeight = 0;
        foreach (BackgroundShape shape in shapes)
        {
            currentWeight += shape.weight;
            if (randomWeight < currentWeight)
            {
                shapeToSpawn = shape;
                break;
            }
        }

        if (shapeToSpawn == null)
        {
            Debug.LogWarning("No shape to spawn!");
            return;
        }

        Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        Transform shapeTransform = Instantiate(shapeToSpawn.prefab, spawnPos, rotation, transform).transform;

        float rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        bool rotateClockwise = Random.Range(0, 2) == 0;
        shapeTransform.DORotate(new Vector3(0f, 0f, rotateClockwise ? 360f : -360f), rotateSpeed, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental);

        float size = Random.Range(minSize, maxSize);
        shapeTransform.localScale = Vector3.one * size;

        float speed = Random.Range(minSpeed, maxSpeed);
        shapeTransform.DOMoveX(-spawnX, speed).SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .OnComplete(() => Destroy(shapeTransform.gameObject));
    }
}
