using UnityEngine;
using Shapes; 

public class Wall : MonoBehaviour
{
    [SerializeField] private float survivalTime = 3f; 
    private float survivalTimer = 0f; 
    private bool isTouching = false; 

    [SerializeField] private Color dangerColour = Color.red;
    private Color normalColour = Color.white;

    [SerializeField] private ShapeRenderer shape = null;

    private GameManager gameManager = null;    

    private void Start ()
    {
        gameManager = FindObjectOfType<GameManager>(); 

        normalColour = shape.Color;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!other.gameObject.CompareTag("Player"))
            return; 

        isTouching = true;
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (!other.gameObject.CompareTag("Player"))
            return; 

        isTouching = false;
    }

    private void Update ()
    {
        if (!isTouching)
        {
            shape.Color = normalColour;

            survivalTimer = 0f; 
            return;
        }

        survivalTimer += Time.deltaTime;

        float t = survivalTimer / survivalTime;
        Color colour = Color.Lerp(normalColour, dangerColour, t);
        shape.Color = colour;

        if (survivalTimer >= survivalTime)
        {
            gameManager.GameOver();
        }
    }
}
