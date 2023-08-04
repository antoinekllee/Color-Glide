using UnityEngine;
using MyBox;
using DG.Tweening; 
using Shapes;
using MoreMountains.Feedbacks; 

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header ("Controls")]
    [SerializeField, PositiveValueOnly] private float jumpForce = 20f;
    private bool justTapped = false;
    [Space(8)]
    [SerializeField, PositiveValueOnly] private float gravity = 2f;
    // [SerializeField, PositiveValueOnly] private float normalGravity = 2f; 
    // [SerializeField, PositiveValueOnly] private float reducedGravity = 0.5f;
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

    [Header ("Resetting")]
    [SerializeField, PositiveValueOnly] private float resetDelay = 2f;
    private Vector3 startPos = Vector3.zero;

    private Rigidbody2D rigidBody = null;
    private Disc shape = null; 
    private new CircleCollider2D collider = null;
    private Animator animator = null; 

    private GameManager gameManager = null; 

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        shape = GetComponentInChildren<Disc>();
        collider = GetComponentInChildren<CircleCollider2D>();
        animator = GetComponent <Animator>(); 

        gameManager = FindObjectOfType<GameManager>(); 

        colour = gameManager.GetColour(objectColour);
        CycleColour (); // set initial colour

        rigidBody.gravityScale = 0f;

        startPos = transform.position;
    }

    private void Update()
    {
        if (gameManager.isGameOver)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            justTapped = true;
            // shouldThrust = true;
            // if (!jetpackParticles.isPlaying)
            //     jetpackParticles.Play();
        }
        // else
        // {
            // shouldThrust = false;
            // if (jetpackParticles.isPlaying)
            //     jetpackParticles.Stop();
        // }

        // On Mouse Down, record the position
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPosition = Input.mousePosition;
        }

        // On Mouse Up, check if it's a swipe down gesture
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPosition = Input.mousePosition;

            if (Vector2.Distance(startTouchPosition, endTouchPosition) > minSwipeDistance)
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

    private void FixedUpdate() 
    {
        if (gameManager.isGameOver)
            return;

        if (justTapped)
        {
            rigidBody.velocity = Vector2.zero;
            rigidBody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            justTapped = false;
        }

        // if (shouldThrust)
        // {
        //     rigidBody.AddForce(Vector2.up * thrust); // apply upwards force on press
        //     rigidBody.gravityScale = reducedGravity;
        // }
        // else
        // {
        //     rigidBody.gravityScale = normalGravity;
        // }
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

        jetpackParticles.Stop();

        DOVirtual.DelayedCall(resetDelay, () => 
        {
            rigidBody.gravityScale = 0f;

            transform.position = startPos;

            collider.enabled = true;
            shape.enabled = true; 

            animator.SetTrigger ("reset");
        }, false);
    }

    public void PlayJetpackParticles ()
    {
        jetpackParticles.Play();
    }

    public void StartGame()
    {
        animator.SetTrigger ("start");
        rigidBody.gravityScale = gravity;
    }
}
