using UnityEngine;
using MyBox; 
using System;

public enum ObjectColour { Red, Green, Blue }

public class GameManager : MonoBehaviour
{
    [Header ("Colours")]
    [SerializeField] private Color red = Color.red;
    [SerializeField] private Color green = Color.green;
    [SerializeField] private Color blue = Color.blue;

    [Header ("Scrolling")]
    [PositiveValueOnly] public float scrollSpeed = 1f; 

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
            default:
                return Color.white;
        }
    }
}
