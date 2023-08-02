using UnityEngine;
using MyBox;
using DG.Tweening; 

public enum PlayerColour { Red, Green, Blue }

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    [Header ("Controls")]
    [SerializeField, PositiveValueOnly] private float thrust = 5.0f;
    [SerializeField] private PlayerColour colour = PlayerColour.Red;

    [Header ("Colours")]
    [SerializeField] private Color red = Color.red;
    [SerializeField] private Color green = Color.green;
    [SerializeField] private Color blue = Color.blue;
    [Space(8)]
    [SerializeField, PositiveValueOnly] private float colourChangeTime = 0.1f;
    [SerializeField] private Ease colourChangeEase = Ease.InOutSine;

    private float touchTimeStart = 0f; 
    private float touchTimeFinish = 0f; 
    private float timeInterval = 0f; 

    private Rigidbody2D rigidBody = null;
    private SpriteRenderer spriteRenderer = null;
    private new CircleCollider2D collider = null;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<CircleCollider2D>();

        spriteRenderer.color = GetColour(colour); // set default colour
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
        spriteRenderer.DOColor(GetColour(colour), colourChangeTime).SetEase(colourChangeEase); 
    }

    private Color GetColour(PlayerColour playerColour)
    {
        switch (playerColour)
        {
            case PlayerColour.Red:
                return red; 
            case PlayerColour.Green:
                return green; 
            case PlayerColour.Blue:
                return blue; 
            default:
                return Color.white;
        }
    }
}
