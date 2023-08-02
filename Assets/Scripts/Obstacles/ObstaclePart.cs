using UnityEngine;
using Shapes;
using MyBox; 
using DG.Tweening; 

[RequireComponent(typeof(ShapeRenderer))]
[RequireComponent(typeof(Collider2D))]
[RequireTag("Obstacle")]
public class ObstaclePart : MonoBehaviour
{
    [Range (0, 2)] public int id = 0;
    private ObjectColour obstacleColour = ObjectColour.Red;

    [MustBeAssigned] public ShapeRenderer shape = null;
    [MustBeAssigned] public new Collider2D collider = null;

    private GameManager gameManager = null; 
    private PlayerController playerController = null; 

    private void Awake ()
    {
        shape = GetComponent<ShapeRenderer>();
        collider = GetComponent<Collider2D>();

        gameManager = FindObjectOfType<GameManager>();
        playerController = gameManager.playerController; 
    }

    public void SetObjectColour (ObjectColour objectColour)
    {
        obstacleColour = objectColour;

        Color colour = gameManager.GetColour(obstacleColour);
        shape.Color = colour;
    }

    private void SameColourCollision ()
    {
        collider.enabled = false;

        Color greyColour = gameManager.GetColour(ObjectColour.Grey); 
        DOTween.To(() => shape.Color, x => shape.Color = x, greyColour, gameManager.obstacleFadeDuration)
            .SetEase(gameManager.obstacleFadeEase); 

        gameManager.IncreaseScore(); 
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerController.colour == obstacleColour)
            {
                SameColourCollision();
            }
            else
            {
                Debug.Log ("GAME OVER");
            }
        }
    }
}
