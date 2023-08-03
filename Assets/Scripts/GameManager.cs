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
    [SerializeField, MustBeAssigned] private TextMeshProUGUI scoreText = null;
    [SerializeField, MustBeAssigned] private Animator scoreTextAnimator = null; 
    [Space (8)]
    [SerializeField, MustBeAssigned] private TextMeshProUGUI highScoreText = null;
    [Space (8)]
    [SerializeField, MustBeAssigned] private TextMeshProUGUI gameOverScoreText = null;
    [SerializeField, MustBeAssigned] private GameObject gameOverPanel = null; 

    [Header ("Streaks")]
    [SerializeField, MustBeAssigned] private float maxStreakInterval = 0.2f; // max time between obstacles to count as a streak
    [Space (8)]
    [SerializeField, MustBeAssigned] private MMFloatingTextSpawner streakTextSpawner = null;
    [SerializeField, PositiveValueOnly] private Vector2 streakTextOffset = Vector2.one; 
    [SerializeField, MustBeAssigned] private MMFeedbacks streakFeedbacks = null;
    private float streakTimer = 0f;
    private int currStreak = 0;

    private int highScore = 0;
    private int score = 0; 

    [NonSerialized] public bool isGameOver = false; 

    [NonSerialized] public PlayerController playerController = null; 
    private Transform playerTransform = null; 
    private Spawner spawner = null; 

    private void Start ()
    {
        playerController = FindObjectOfType<PlayerController>();
        playerTransform = playerController.transform;
        
        spawner = FindObjectOfType<Spawner>();

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
        scoreFeedbacks?.PlayFeedbacks();

        score++;

        scoreText.text = score.ToString();
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
            return; 

        streakTimer += Time.deltaTime; 
    }

    public void GameOver ()
    {
        gameOverFeedbacks.PlayFeedbacks();
        playerController.Die(); 
        spawner.OnGameOver(); 

        if (score > highScore)
        {
            highScore = score;
            highScoreText.text = highScore.ToString();

            SaveHighScore();
        }

        gameOverScoreText.text = score.ToString(); 

        gameOverPanel.SetActive(true);

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
        }
    }
}
