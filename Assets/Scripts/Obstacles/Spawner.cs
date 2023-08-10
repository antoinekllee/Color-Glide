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
                var pool = new ObjectPool<Transform>(variant.transform);
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
        Transform obstacleTransform = new GameObject("Obstacle").transform;
        obstacleTransform.position = spawnPoint.position;
        obstacleTransform.parent = transform;

        Transform obstacleVariantTransform = obstaclePools[prefab].Get();
        obstacleVariantTransform.gameObject.SetActive(true);
        obstacleVariantTransform.position = obstacleTransform.position;
        obstacleVariantTransform.parent = obstacleTransform;

        Obstacle obstacle = obstacleVariantTransform.GetComponent<Obstacle>();

        float scrollSpeed = Random.Range(minScrollSpeed, maxScrollSpeed);
            
        obstacleTransform.DOMoveX(destroyPoint.position.x, scrollSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => 
            {
                activeObstacles.Remove(obstacleTransform);
                obstacle.ResetParts();
                ReturnToPool(prefab, obstacleVariantTransform);
                obstacleVariantTransform.gameObject.SetActive(false);
                obstacleTransform.gameObject.SetActive(false); // Deactivate instead of Destroy
            });

        activeObstacles.Add(obstacleTransform);
        nextSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    private void ReturnToPool(GameObject prefab, Transform item)
    {
        obstaclePools[prefab].ReturnToPool(item);
    }

    public void OnGameOver()
    {
        foreach (Transform obstacle in activeObstacles)
        {
            obstacle.GetComponentInChildren<Collider2D>().enabled = false;

            // DOTween.Kill(obstacle); // Kill all ongoing animations for the obstacle

            obstacle.DOMoveY(obstacle.position.y + destroyYOffset, dropDuration)
                .SetDelay(dropDelay)
                .SetEase(dropAnimationCurve); 
                // .OnComplete(() => 
                // {
                //     GetComponent<Obstacle>().ResetParts();
                //     Transform childObstacle = obstacle.GetChild(0);
                //     Debug.Log ("B");
                //     Debug.Log (childObstacle.gameObject.name);
                //     ReturnToPool(childObstacle.gameObject, childObstacle);
                //     childObstacle.gameObject.SetActive(false);
                //     obstacle.gameObject.SetActive(false); 
                // });
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
