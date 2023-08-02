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

    private GameManager gameManager = null; 

    private void Start ()
    {
        nextSpawnInterval = 0f;

        gameManager = FindObjectOfType<GameManager>();
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

        // Spawn a new empty gameobject called "obstacle" at the spawnpoint position, with no rotation, as a child of the spawner
        // put the obstacle prefab as a child of the spawner
        
        // First, we instantiate an empty transform at spawnpoint (NOT THE PREFAB)
        Transform obstacleTransform = Instantiate(new GameObject("Obstacle"), spawnPoint.position, Quaternion.identity, transform).transform;
        // Then, we instantiate the obstacle prefab as a child of the empty transform
        Instantiate(obstaclePrefabs[index], obstacleTransform.position, Quaternion.identity, obstacleTransform);
        
        // obstacleTransform.Translate(Vector2.left * gameManager.scrollSpeed * Time.deltaTime);

        // use dotween to move the obstacle transform to the left by gamemanager.scollspeed then destroy it after it reaches destoypoint x position. make it speed based
        obstacleTransform.DOMoveX(destroyPoint.position.x, gameManager.scrollSpeed)
            .SetSpeedBased(true)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(obstacleTransform.gameObject));
    }
}
