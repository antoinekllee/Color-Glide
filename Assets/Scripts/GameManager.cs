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
    [SerializeField, MustBeAssigned] private TextMeshProUGUI gameOverScoreText = null;
    [SerializeField, MustBeAssigned] private GameObject gameOverPanel = null; 

    private int score = 0; 

    [NonSerialized] public bool isGameOver = false; 

    [NonSerialized] public PlayerController playerController = null; 

    private void Start ()
    {
        playerController = FindObjectOfType<PlayerController>();
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
        Debug.Log ("Score: " + score);

        scoreText.text = score.ToString();
    }

    public void GameOver ()
    {
        Debug.Log ("Game Over!");

        gameOverFeedbacks.PlayFeedbacks();
        playerController.Die(); 

        gameOverScoreText.text = score.ToString(); 

        gameOverPanel.SetActive(true);

        isGameOver = true; 
    }
}
