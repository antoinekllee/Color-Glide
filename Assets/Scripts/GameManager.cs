using UnityEngine;

public enum PlayerColour { Red, Green, Blue }

public class GameManager : MonoBehaviour
{
    [Header ("Colours")]
    [SerializeField] private Color red = Color.red;
    [SerializeField] private Color green = Color.green;
    [SerializeField] private Color blue = Color.blue;

    public Color GetColour(PlayerColour playerColour)
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
