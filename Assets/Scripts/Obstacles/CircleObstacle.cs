using UnityEngine;
using MyBox; 
using Shapes; 

public class CircleObstacle : Obstacle
{
    [SerializeField, PositiveValueOnly] private float minRadius = 1.5f;
    [SerializeField, PositiveValueOnly] private float maxRadius = 3.5f;
    [SerializeField, PositiveValueOnly] private int resolution = 20;
    [SerializeField, PositiveValueOnly] private float thickness = 0.2f;

    private float size = 0f;

    protected override void SetupObstacle()
    {
        base.SetupObstacle();

        size = Random.Range(minRadius, maxRadius);

        foreach (ObstaclePart part in parts)
        {
            // Set up the shape
            Disc disc = (Disc)part.shape;
            disc.Radius = size;
            disc.Thickness = thickness;

            // Now, we need to set the collider size
            PolygonCollider2D collider = (PolygonCollider2D)part.collider;
            collider.points = CreateArcPoints(size - (thickness / 2f), size + (thickness / 2f), resolution);
        }
    }

    private Vector2[] CreateArcPoints(float innerRadius, float outerRadius, int resolution)
    {
        // Add 1 to resolution to ensure 90 degrees
        Vector2[] points = new Vector2[(resolution + 1) * 2];

        float angleStep = 90f / resolution;

        for (int i = 0; i <= resolution; i++)
        {
            float angle = angleStep * i * Mathf.Deg2Rad;
            points[i] = new Vector2(Mathf.Cos(angle) * outerRadius, Mathf.Sin(angle) * outerRadius);
            points[points.Length - i - 1] = new Vector2(Mathf.Cos(angle) * innerRadius, Mathf.Sin(angle) * innerRadius);
        }

        return points;
    }
}