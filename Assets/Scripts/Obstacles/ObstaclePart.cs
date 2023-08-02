using UnityEngine;
using Shapes;
using MyBox; 

[RequireComponent(typeof(ShapeRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireTag("Obstacle")]
public class ObstaclePart : MonoBehaviour
{
    [Range (1, 3)] public int id = 1;
    private ObjectColour obstacleColour = ObjectColour.Red;

    private ShapeRenderer sprite = null;
    private new Collider2D collider = null;

    private GameManager gameManager = null; 

    private void Start ()
    {
        sprite = GetComponent<ShapeRenderer>();
        collider = GetComponent<Collider2D>();

        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetObjectColour (ObjectColour objectColour)
    {
        obstacleColour = objectColour;

        Color colour = gameManager.GetColour(obstacleColour);
        sprite.Color = colour;
    }
}
