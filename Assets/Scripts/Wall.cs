using UnityEngine;
using Shapes;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;

public class Wall : MonoBehaviour
{
    [SerializeField] private float survivalTime = 3f;
    private float survivalTimer = 0f;
    private bool isTouching = false;

    [SerializeField] private Color dangerColour = Color.red;
    private Color normalColour = Color.white;

    [SerializeField] private ShapeRenderer shape = null;

    private GameManager gameManager = null;

    [Header("Vignette Settings")]
    public Volume volume;
    private Vignette vignette;
    [SerializeField] private float maxVignIntensity = .45f;
    private float defaultVignIntensity = 0f;

    [SerializeField] private float resetDuration = 0.5f;
    [SerializeField] private Ease resetEase = Ease.InOutSine;
    private bool isReset = false; 

    [Header ("Sound")]
    [SerializeField] private AudioSource sfx = null;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();

        normalColour = shape.Color;

        // Get the Vignette effect from the Volume
        if (volume.profile.TryGet(out vignette))
        {
            defaultVignIntensity = vignette.intensity.value;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        sfx.Play();

        isReset = false; 
        isTouching = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
            return;

        sfx.Stop();

        isTouching = false;
    }

    private void Update()
    {
        if (!isTouching || gameManager.isGameOver)
        {
            survivalTimer = 0f;

            if (!isReset)
            {
                DOTween.To(() => shape.Color, x => shape.Color = x, normalColour, resetDuration)
                    .SetEase(resetEase);

                DOTween.To(() => vignette.intensity.value, x => vignette.intensity.value = x, 0, resetDuration)
                    .SetEase(resetEase);

                isReset = true;
            }

            return;
        }

        survivalTimer += Time.deltaTime;

        float t = survivalTimer / survivalTime;
        Color colour = Color.Lerp(normalColour, dangerColour, t);
        shape.Color = colour;

        // Animate vignette intensity
        vignette.intensity.value = Mathf.Lerp(0, maxVignIntensity, t);

        if (survivalTimer >= survivalTime)
        {
            gameManager.GameOver();
        }
    }
}
