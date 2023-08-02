using UnityEngine;
using MyBox; 
using DG.Tweening; 

public class Spawner : MonoBehaviour
{
    [Header ("Spawning")]
    [SerializeField] private GameObject[] obstaclePrefabs = null;
    [SerializeField, MustBeAssigned] private Transform spawnPoint = null; 
    [SerializeField, MustBeAssigned] private Transform destroyPoint = null; 

    [Header ("Intervals")]
    [SerializeField, PositiveValueOnly] private float minSpawnInterval = 5f; 
    [SerializeField, PositiveValueOnly] private float maxSpawnInterval = 10f;
    private float nextSpawnInterval = 0f;
    private float spawnTimer = 0f; 
    [Space (8)]
    [SerializeField] private float minScrollSpeed = 1f; 
    [SerializeField] private float maxScrollSpeed = 3f;

    private void Start ()
    {
        nextSpawnInterval = 0f;
    }

    private void Update ()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= nextSpawnInterval)
        {
            SpawnObstacle();
            spawnTimer -= nextSpawnInterval;
            nextSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    private void SpawnObstacle ()
    {
        int index = Random.Range(0, obstaclePrefabs.Length);

        // Create empty parent object to translate right to left
        Transform obstacleTransform = Instantiate(new GameObject("Obstacle"), spawnPoint.position, Quaternion.identity, transform).transform;
        // Create obstacle as child of parent object so it can rotate locally
        Instantiate(obstaclePrefabs[index], obstacleTransform.position, Quaternion.identity, obstacleTransform);
        
        float scrollSpeed = Random.Range(minScrollSpeed, maxScrollSpeed);

        obstacleTransform.DOMoveX(destroyPoint.position.x, scrollSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(obstacleTransform.gameObject));
    }
}
