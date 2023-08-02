using UnityEngine;
using MyBox;
using Shapes;

public class CircleObstacle : Obstacle
{
    [SerializeField, PositiveValueOnly] private float minRadius = 1.5f;
    [SerializeField, PositiveValueOnly] private float maxRadius = 3.5f;
    private float size = 0f;
    
    protected override void SetupObstacle()
    {
        base.SetupObstacle();

        size = Random.Range(minRadius, maxRadius);

        foreach (ObstaclePart part in parts)
        {
            Disc disc = (Disc)part.shape;
            disc.Radius = size;
        }
    }
}
