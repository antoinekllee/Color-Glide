using UnityEngine;
using MyBox;
using Shapes;

public class LineObstacle : Obstacle
{
    [Header ("Specific")]
    [SerializeField, PositiveValueOnly] private float minSize = 1.5f;
    [SerializeField, PositiveValueOnly] private float maxSize = 3.5f;
    private float size = 0f;
    [Space (8)]
    [SerializeField] private float rotationFirst = 45f;
    [SerializeField] private float rotationSecond = -45f;
    
    protected override void SetupObstacle()
    {
        base.SetupObstacle();

        ObstaclePart part = parts[0];

        int shouldFlip = Random.Range(0, 2);
        part.transform.localRotation = Quaternion.Euler(0f, 0f, shouldFlip == 0 ? rotationFirst : rotationSecond);

        size = Random.Range(minSize, maxSize);
        Rectangle rectangle = (Rectangle)part.shape;
        rectangle.Width = size;
    }
}
