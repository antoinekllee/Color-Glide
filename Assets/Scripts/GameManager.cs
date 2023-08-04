using UnityEngine;
using MyBox; 
using System;
using DG.Tweening; 
using MoreMountains.Feedbacks;
using TMPro; 

public enum ObjectColour { Red, Green, Blue, Grey }

public class GameManager : MonoBehaviour
{
    [Header ("Colours")]
    [SerializeField] private Color red = Color.red;
    [SerializeField] private Color green = Color.green;
    [SerializeField] private Color blue = Color.blue;
    [SerializeField] private Color grey = Color.grey;

    [Header ("Effects")]
    [PositiveValueOnly] public float obstacleFadeDuration = 0.1f;
    public Ease obstacleFadeEase = Ease.InOutSine;
    [Space (8)]
    [SerializeField, MustBeAssigned] private MMF_Player scoreFeedbacks = null;
    [SerializeField, MustBeAssigned] private MMF_Player gameOverFeedbacks = null; 

    [Header ("UI")]
    [SerializeField, MustBeAssigned] private TextMeshProUGUI inGameScoreText = null;
    [Space (8)]
    [SerializeField, MustBeAssigned] private GameObject menuPanel = null;
    [SerializeField, MustBeAssigned] private Animator menuAnimator = null; 
    [SerializeField, MustBeAssigned] private TextMeshProUGUI restartText = null;
    [Space (8)]
    [SerializeField, MustBeAssigned] private GameObject startScoresPanel = null;
    [SerializeField, MustBeAssigned] private TextMeshProUGUI startHighScoreText = null;
    [Space (8)]
    [SerializeField, MustBeAssigned] private GameObject inGameScoresPanel = null;
    [SerializeField, MustBeAssigned] private TextMeshProUGUI highScoreText = null;
    [SerializeField, MustBeAssigned] private TextMeshProUGUI menuScoreText = null;
    [SerializeField, MustBeAssigned] private Animator scoreTextAnimator = null; 

    [Header ("Streaks")]
    [SerializeField, MustBeAssigned] private float maxStreakInterval = 0.2f; // max time between obstacles to count as a streak
    [Space (8)]
    [SerializeField, MustBeAssigned] private MMFloatingTextSpawner streakTextSpawner = null;
    [SerializeField, PositiveValueOnly] private Vector2 streakTextOffset = Vector2.one; 
    [SerializeField, MustBeAssigned] private MMFeedbacks streakFeedbacks = null;
    private float streakTimer = 0f;
    private int currStreak = 0;

    [Header ("Restarting")]
    [SerializeField, PositiveValueOnly] private float restartCooldown = 0.5f;
    private bool canRestart = false; 

    [Header ("Pausing")]
    [SerializeField, MustBeAssigned] private CanvasGroup pauseCanvasGroup = null;
    [SerializeField, MustBeAssigned] private GameObject pauseButton = null; 
    [Space (8)]
    [SerializeField, PositiveValueOnly] private float pauseFadeDuration = 0.5f;
    [SerializeField] private Ease pauseFadeEase = Ease.InOutSine;
    private bool isPaused = false; 

    private int highScore = 0;
    private int score = 0; 

    [NonSerialized] public bool isGameOver = true; 

    [NonSerialized] public PlayerController playerController = null; 
    private Transform playerTransform = null; 
    private Spawner spawner = null; 

    private void Start ()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerTransform = playerController.transform;
        
        spawner = FindObjectOfType<Spawner>();

        startScoresPanel.SetActive(true);
        inGameScoresPanel.SetActive(false);

        DOVirtual.DelayedCall(restartCooldown, () => canRestart = true, false);

        LoadHighScore();
    }

    public Color GetColour(ObjectColour playerColour)
    {
        switch (playerColour)
        {
            case ObjectColour.Red:
                return red; 
            case ObjectColour.Green:
                return green;
            case ObjectColour.Blue:
                return blue;
            case ObjectColour.Grey:
                return grey;
            default:
                return Color.white;
        }
    }

    public void IncreaseScore ()
    {
        if (isGameOver)
            return;

        scoreFeedbacks?.PlayFeedbacks();

        score++;

        inGameScoreText.text = score.ToString();
        scoreTextAnimator.SetTrigger ("spin"); 

        if (streakTimer <= maxStreakInterval)
        {
            currStreak++;

            streakTextSpawner.SpawnOffsetMin = playerController.transform.position + (Vector3)streakTextOffset;
            streakTextSpawner.SpawnOffsetMax = playerController.transform.position + (Vector3)streakTextOffset;

            // get floating text feedback and set value
            streakFeedbacks.GetComponent<MMFeedbackFloatingText>().Value = "x" + currStreak.ToString();

            streakFeedbacks?.PlayFeedbacks();
        }
        else
            currStreak = 1;

        streakTimer -= streakTimer; // reset timer
    }

    private void Update ()
    {
        if (isGameOver)
        {
            if (Input.GetMouseButtonDown(0) && canRestart)
            {
                isGameOver = false;
                menuAnimator.SetTrigger("start");
                pauseButton.SetActive(true);
                playerController.StartGame();
            }

            return; 
        }

        if (isPaused)
        {
            if (Input.GetMouseButtonDown(0))
                PauseGame();

            return;    
        }

        streakTimer += Time.deltaTime; 
    }

    public void GameOver ()
    {
        gameOverFeedbacks.PlayFeedbacks();
        menuAnimator.SetTrigger("gameOver");
        playerController.Die(); 
        spawner.OnGameOver(); 

        startScoresPanel.SetActive(false);
        inGameScoresPanel.SetActive(true);

        restartText.text = "Tap to restart!";

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();

            SaveHighScore();
        }

        menuScoreText.text = score.ToString();

        score = 0; // reset score
        inGameScoreText.text = score.ToString();

        menuPanel.SetActive(true);
        pauseButton.SetActive(false);
        pauseCanvasGroup.alpha = 0f;

        canRestart = false;
        DOVirtual.DelayedCall(restartCooldown, () => canRestart = true, false);

        isGameOver = true; 
    }

    private void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }

    private void LoadHighScore()
    {
        if(PlayerPrefs.HasKey("HighScore"))
        {
            highScore = PlayerPrefs.GetInt("HighScore");
            highScoreText.text = highScore.ToString();
            startHighScoreText.text = highScore.ToString();
        }
    }

    public void PauseGame ()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseCanvasGroup.DOFade(1f, pauseFadeDuration).SetUpdate(true).SetEase(pauseFadeEase);
        }
        else
        {
            Time.timeScale = 1f;
            pauseCanvasGroup.DOFade(0f, pauseFadeDuration).SetUpdate(true).SetEase(pauseFadeEase);
        }
    }
}
