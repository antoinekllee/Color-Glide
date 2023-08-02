using UnityEngine;
using MyBox;

public enum Colour { Red, Green, Blue }

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, PositiveValueOnly] private float thrust = 5.0f;
    [SerializeField] private Colour colour = Colour.Red;

    private float touchTimeStart = 0f; 
    private float touchTimeFinish = 0f; 
    private float timeInterval = 0f; 

    private Rigidbody2D rigidBody = null;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
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
                colour = (Colour)(((int)colour + 1) % 3); // change colour
                Debug.Log ("CHANGING COLOURS"); 
            }
        }
    }
}
