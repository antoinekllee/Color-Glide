using UnityEngine;
using MyBox; 
using DG.Tweening; 
using System; 
using Random = UnityEngine.Random; 
using System.Collections.Generic;

[Serializable] 
public class ObstacleData
{
    public GameObject[] prefabVariants = null; 
    public bool isBigObstacle = false;
    [Space (8)]
    public bool useCustomIntervals = true; 
    [ConditionalField(nameof(useCustomIntervals))] public float minSpawnInterval = 5f;
    [ConditionalField(nameof(useCustomIntervals))] public float maxSpawnInterval = 10f;
    [Space (8)]
    public bool useCustomScrollSpeed = true;
    [ConditionalField(nameof(useCustomScrollSpeed))] public float minScrollSpeed = 1f;
    [ConditionalField(nameof(useCustomScrollSpeed))] public float maxScrollSpeed = 3f;
}

public class Spawner : MonoBehaviour
{
    [Header ("Spawning")]
    [SerializeField] private ObstacleData[] obstacleData = null; 
    [SerializeField, MustBeAssigned] private Transform spawnPoint = null; 
    [SerializeField, MustBeAssigned] private Transform destroyPoint = null; 
    private bool lastWasBigObstacle = false;

    private List<Transform> activeObstacles = new List<Transform>();

    [Header ("Intervals")]
    [SerializeField, PositiveValueOnly] private float minSpawnInterval = 5f; 
    [SerializeField, PositiveValueOnly] private float maxSpawnInterval = 10f;
    private float nextSpawnInterval = 0f;
    private float spawnTimer = 0f; 
    [Space (8)]
    [SerializeField] private float minScrollSpeed = 1f; 
    [SerializeField] private float maxScrollSpeed = 3f;

    [Header ("Game Over")]
    [SerializeField, PositiveValueOnly] private float dropDelay = 0.5f;
    [SerializeField, PositiveValueOnly] private float dropSpeed = 2f;
    [SerializeField] private float destroyY = -10f;
    [SerializeField, PositiveValueOnly] private AnimationCurve dropAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private GameManager gameManager = null; 

    private void Start ()
    {
        gameManager = FindObjectOfType<GameManager>();

        nextSpawnInterval = 0f;
    }

    private void Update ()
    {
        if (gameManager.isGameOver)
            return; 

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= nextSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer -= nextSpawnInterval;
        }
    }

    private void SpawnObstacle ()
    {
        int index = Random.Range(0, obstacleData.Length);
        while (lastWasBigObstacle && obstacleData[index].isBigObstacle)
            index = Random.Range(0, obstacleData.Length);

        ObstacleData data = obstacleData[index];
        lastWasBigObstacle = data.isBigObstacle;
        int variantInex = Random.Range(0, data.prefabVariants.Length);
        GameObject prefab = data.prefabVariants[variantInex];

        // Create empty parent object to translate right to left
        Transform obstacleTransform = Instantiate(new GameObject("Obstacle"), spawnPoint.position, Quaternion.identity, transform).transform;
        // Create obstacle as child of parent object so it can rotate locally
        Instantiate(prefab, obstacleTransform.position, Quaternion.identity, obstacleTransform);
        
        float scrollSpeed = data.useCustomScrollSpeed ? Random.Range(data.minScrollSpeed, data.maxScrollSpeed) : Random.Range(minScrollSpeed, maxScrollSpeed);

        obstacleTransform.DOMoveX(destroyPoint.position.x, scrollSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                activeObstacles.Remove(obstacleTransform);
                Destroy(obstacleTransform.gameObject);
            });

        activeObstacles.Add(obstacleTransform);

        nextSpawnInterval = data.useCustomIntervals ? Random.Range(data.minSpawnInterval, data.maxSpawnInterval) : Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    public void OnGameOver ()
    {
        foreach (Transform obstacle in activeObstacles)
        {
            DOTween.Kill(obstacle);

            obstacle.DOMoveY(destroyY, dropSpeed)
                .SetDelay(dropDelay)
                .SetSpeedBased(true)
                .SetEase(dropAnimationCurve)
                .OnComplete(() => Destroy(obstacle.gameObject));
        }

        activeObstacles.Clear();
    }
}
