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
}

public class Spawner : MonoBehaviour
{
    [Header ("Spawning")]
    [SerializeField] private ObstacleData[] obstacleData = null; 
    [SerializeField, MustBeAssigned] private Transform spawnPoint = null; 
    [SerializeField, MustBeAssigned] private Transform destroyPoint = null; 
    private bool lastWasBigObstacle = false;
    private int lastObstacleIndex = -1;

    private List<Transform> activeObstacles = new List<Transform>();

    [Header ("Intervals")]
    [SerializeField, PositiveValueOnly] private float minSpawnInterval = 1.2f; 
    [SerializeField, PositiveValueOnly] private float maxSpawnInterval = 3f;
    private float nextSpawnInterval = 0f;
    private float spawnTimer = 0f; 
    [Space (8)]
    [SerializeField] private float minScrollSpeed = 1.3f; 
    [SerializeField] private float maxScrollSpeed = 1.9f;

    [Header ("Levels")]
    [SerializeField, PositiveValueOnly] private float levelUpIntervalDecrease = 0.05f;
    [SerializeField, PositiveValueOnly] private float levelUpScrollSpeedIncrease = 0.05f;
    [SerializeField, PositiveValueOnly] private int stopLevelUpScore = 100; 

    [Header ("Game Over")]
    [SerializeField, PositiveValueOnly] private float dropDelay = 0.5f;
    [SerializeField, PositiveValueOnly] private float dropDuration = 2f;
    [SerializeField] private float destroyYOffset = -10f;
    [SerializeField, PositiveValueOnly] private AnimationCurve dropAnimationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private GameManager gameManager = null; 

    private Dictionary<GameObject, ObjectPool<Transform>> obstaclePools = new Dictionary<GameObject, ObjectPool<Transform>>();

    private void Awake()
    {
        foreach (var data in obstacleData)
        {
            foreach (var variant in data.prefabVariants)
            {
                var obstacleParent = new GameObject("Object_" + variant.name);
                Transform child = Instantiate(variant).transform;
                child.parent = obstacleParent.transform;
                child.gameObject.SetActive(false);  // Deactivate child
                var pool = new ObjectPool<Transform>(obstacleParent.transform);
                obstaclePools[variant] = pool;
            }
        }
    }

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
        while ((lastWasBigObstacle &&  obstacleData[index].isBigObstacle) || (index == lastObstacleIndex && obstacleData.Length > 1) || (gameManager.score < 5 && obstacleData[index].isBigObstacle))
            index = Random.Range(0, obstacleData.Length);
        
        lastObstacleIndex = index;

        ObstacleData data = obstacleData[index];
        lastWasBigObstacle = data.isBigObstacle;
        int variantInex = Random.Range(0, data.prefabVariants.Length);
        
        GameObject prefab = data.prefabVariants[variantInex];
        Transform obstacleTransform = obstaclePools[prefab].Get();
        obstacleTransform.gameObject.SetActive(true); // Ensure the object is active
        obstacleTransform.gameObject.name = "Object_" + prefab.name; // Optional naming step
        obstacleTransform.position = spawnPoint.position;
        obstacleTransform.parent = transform;    

        // Get the child from the pooled parent and activate it
        Transform obstacleVariantTransform = obstacleTransform.GetChild(0);
        obstacleVariantTransform.gameObject.SetActive(true);
        obstacleVariantTransform.position = obstacleTransform.position;

        Obstacle obstacle = obstacleVariantTransform.GetComponent<Obstacle>();

        float scrollSpeed = Random.Range(minScrollSpeed, maxScrollSpeed);
            
        obstacleTransform.DOMoveX(destroyPoint.position.x, scrollSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                activeObstacles.Remove(obstacleTransform);
                obstacle.ResetParts();
                ReturnToPool(prefab, obstacleTransform);
                obstacleVariantTransform.gameObject.SetActive(false);
            });

        activeObstacles.Add(obstacleTransform);
        nextSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void ReturnToPool(GameObject prefab, Transform parentItem)
    {
        Transform childObstacle = parentItem.GetChild(0);
        childObstacle.gameObject.SetActive(false);
        obstaclePools[prefab].ReturnToPool(parentItem);
    }

    public void OnGameOver()
    {
        foreach (Transform obstacle in activeObstacles)
        {
            obstacle.GetComponentInChildren<Collider2D>().enabled = false;

            obstacle.DOMoveY(obstacle.position.y + destroyYOffset, dropDuration)
                .SetDelay(dropDelay)
                .SetEase(dropAnimationCurve);

        }

        activeObstacles.Clear();
    }

    public bool CheckDifficulty ()
    {
        // Every 10 points (or if score ends in a 0), increase difficulty
        if (gameManager.score % 10 == 0 && gameManager.score != 0)
        {
            if (gameManager.score < stopLevelUpScore)
            {
                minSpawnInterval -= levelUpIntervalDecrease;
                maxSpawnInterval -= levelUpIntervalDecrease;
                minScrollSpeed += levelUpScrollSpeedIncrease;
                maxScrollSpeed += levelUpScrollSpeedIncrease;
            }

            gameManager.SpawnFloatingText("Level up!");

            return true;
        }

        return false;
    }
}