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
    [Space(8)]
    [SerializeField, PositiveValueOnly] private float normalGravity = 2f; 
    [SerializeField, PositiveValueOnly] private float reducedGravity = 0.5f;
    [Space(8)]
    public ObjectColour colour = ObjectColour.Red;

    [Header ("Colours")]
    [SerializeField, PositiveValueOnly] private float colourChangeTime = 0.1f;
    [SerializeField] private Ease colourChangeEase = Ease.InOutSine;

    // Define minimum swipe distance
    [SerializeField] private float minSwipeDistance = 200f;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

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
        rigidBody.gravityScale = normalGravity;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            rigidBody.AddForce(Vector2.up * thrust); // apply upwards force on press
            rigidBody.gravityScale = reducedGravity;

            Debug.Log ("Mouse Down");
        }
        else
        {
            rigidBody.gravityScale = normalGravity;
        }

        // On Mouse Down, record the position
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        // On Mouse Up, check if it's a swipe down gesture
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;
            float swipeDistance = startTouchPosition.y - endTouchPosition.y;

            if (swipeDistance > minSwipeDistance)
            {
                CycleColour(); 
            }
        }

        // If Spacebar is pressed, change the color
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CycleColour();
        }
    }

    private void CycleColour ()
    {
        colour = (ObjectColour)(((int)colour + 1) % 3); // change colour to next in sequence
        DOTween.To(() => sprite.Color, x => sprite.Color = x, gameManager.GetColour(colour), colourChangeTime).SetEase(colourChangeEase);
    }
}
