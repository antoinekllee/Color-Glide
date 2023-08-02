using UnityEngine;
using MyBox; 
using DG.Tweening; 
using System; 
using Random = UnityEngine.Random; 

[Serializable] 
public class ObstacleData
{
    public GameObject prefab = null; 
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

    [Header ("Intervals")]
    [SerializeField, PositiveValueOnly] private float minSpawnInterval = 5f; 
    [SerializeField, PositiveValueOnly] private float maxSpawnInterval = 10f;
    private float nextSpawnInterval = 0f;
    private float spawnTimer = 0f; 
    [Space (8)]
    [SerializeField] private float minScrollSpeed = 1f; 
    [SerializeField] private float maxScrollSpeed = 3f;

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

        // Create empty parent object to translate right to left
        Transform obstacleTransform = Instantiate(new GameObject("Obstacle"), spawnPoint.position, Quaternion.identity, transform).transform;
        // Create obstacle as child of parent object so it can rotate locally
        Instantiate(data.prefab, obstacleTransform.position, Quaternion.identity, obstacleTransform);
        
        float scrollSpeed = data.useCustomScrollSpeed ? Random.Range(data.minScrollSpeed, data.maxScrollSpeed) : Random.Range(minScrollSpeed, maxScrollSpeed);

        obstacleTransform.DOMoveX(destroyPoint.position.x, scrollSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(obstacleTransform.gameObject));

        nextSpawnInterval = data.useCustomIntervals ? Random.Range(data.minSpawnInterval, data.maxSpawnInterval) : Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
