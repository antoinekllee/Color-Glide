using UnityEngine;
using MyBox;
using DG.Tweening; 
using Shapes;
using MoreMountains.Feedbacks; 

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
    [SerializeField] private float minSwipeDistance = 200f;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    [Space(8)]
    public ObjectColour objectColour = ObjectColour.Red;
    public Color colour = Color.red;

    [Header ("Colours")]
    [SerializeField, PositiveValueOnly] private float colourChangeTime = 0.1f;
    [SerializeField] private Ease colourChangeEase = Ease.InOutSine;

    [Header ("Effects")]
    [SerializeField, MustBeAssigned] private ParticleSystem jetpackParticles = null;
    [Space (8)]
    [SerializeField, MustBeAssigned] private ParticleSystem scoreParticles = null; 
    [Space (8)]
    [SerializeField, MustBeAssigned] private MMF_Player swapFeedbacks = null;
    [SerializeField, MustBeAssigned] private ParticleSystem swapParticles = null;
    [Space (8)]
    [SerializeField, MustBeAssigned] private ParticleSystem deathParticles = null;

    private Rigidbody2D rigidBody = null;
    private Disc shape = null; 
    private new CircleCollider2D collider = null;

    private GameManager gameManager = null; 

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        shape = GetComponent<Disc>();
        collider = GetComponent<CircleCollider2D>();

        gameManager = FindObjectOfType<GameManager>(); 

        colour = gameManager.GetColour(objectColour); 
        shape.Color = colour; // set default colour
        rigidBody.gravityScale = normalGravity;
    }

    private void Update()
    {
        if (gameManager.isGameOver)
            return;

        if (Input.GetMouseButton(0))
        {
            rigidBody.AddForce(Vector2.up * thrust); // apply upwards force on press
            rigidBody.gravityScale = reducedGravity;

            if (!jetpackParticles.isPlaying)
                jetpackParticles.Play();
        }
        else
        {
            rigidBody.gravityScale = normalGravity;

            if (jetpackParticles.isPlaying)
                jetpackParticles.Stop();
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
        swapFeedbacks?.PlayFeedbacks();
        ParticleSystem.MainModule main = swapParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(colour);
        swapParticles?.Play();

        objectColour = (ObjectColour)(((int)objectColour + 1) % 3); // change colour to next in sequence
        colour = gameManager.GetColour(objectColour); 

        DOTween.To(() => shape.Color, x => shape.Color = x, colour, colourChangeTime).SetEase(colourChangeEase);

        // update colour of scoreParticles
        main = scoreParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(colour);

        // update colour of deathParticles
        main = deathParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(colour);

        // update colour of jetpackParticles
        main = jetpackParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(colour);
    }

    public void Die ()
    {
        collider.enabled = false; 
        shape.enabled = false;

        rigidBody.gravityScale = 0f;
        rigidBody.velocity = Vector2.zero;
        rigidBody.angularVelocity = 0f;
    }
}
