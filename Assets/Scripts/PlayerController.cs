using UnityEngine;
using MyBox;
using DG.Tweening; 
using Shapes;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Disc))]
public class PlayerController : MonoBehaviour
{
    [Header ("Controls")]
    [SerializeField, PositiveValueOnly] private float thrust = 5.0f;
    [SerializeField] private PlayerColour colour = PlayerColour.Red;

    [Header ("Colours")]
    [SerializeField, PositiveValueOnly] private float colourChangeTime = 0.1f;
    [SerializeField] private Ease colourChangeEase = Ease.InOutSine;

    private float touchTimeStart = 0f; 
    private float touchTimeFinish = 0f; 
    private float timeInterval = 0f; 

    private Rigidbody2D rigidBody = null;
    private Disc sprite = null; 
    private new CircleCollider2D collider = null;

    private GameManager gameManager = null; 

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<Disc>();
        collider = GetComponent<CircleCollider2D>();

        gameManager = FindObjectOfType<GameManager>(); 

        sprite.Color = gameManager.GetColour(colour); // set default colour
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rigidBody.AddForce(Vector2.up * thrust); // apply upwards force on press
        }

        if (Input.GetMouseButtonDown(0))
            touchTimeStart = Time.time;

        if (Input.GetMouseButtonUp(0))
        {
            touchTimeFinish = Time.time;
            timeInterval = touchTimeFinish - touchTimeStart;

            if (timeInterval < 0.2f) // if it is a tap
            {
                CycleColour(); 
            }
        }
    }

    private void CycleColour ()
    {
        colour = (PlayerColour)(((int)colour + 1) % 3); // change colour to next in sequence
        DOTween.To(() => sprite.Color, x => sprite.Color = x, gameManager.GetColour(colour), colourChangeTime).SetEase(colourChangeEase);
    }
}
