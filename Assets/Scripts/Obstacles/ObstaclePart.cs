using UnityEngine;
using Shapes;
using MyBox; 

[RequireComponent(typeof(ShapeRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireTag("Obstacle")]
public class ObstaclePart : MonoBehaviour
{
    [Range (0, 2)] public int id = 0;
    private ObjectColour obstacleColour = ObjectColour.Red;

    [MustBeAssigned] public ShapeRenderer shape = null;
    private new Collider2D collider = null;

    private GameManager gameManager = null; 

    private void Awake ()
    {
        shape = GetComponent<ShapeRenderer>();
        collider = GetComponent<Collider2D>();

        gameManager = FindObjectOfType<GameManager>();
    }

    public void SetObjectColour (ObjectColour objectColour)
    {
        obstacleColour = objectColour;

        Color colour = gameManager.GetColour(obstacleColour);
        shape.Color = colour;
    }
}
