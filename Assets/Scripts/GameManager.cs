using UnityEngine;

public enum ObjectColour { Red, Green, Blue }

public class GameManager : MonoBehaviour
{
    [Header ("Colours")]
    [SerializeField] private Color red = Color.red;
    [SerializeField] private Color green = Color.green;
    [SerializeField] private Color blue = Color.blue;

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
