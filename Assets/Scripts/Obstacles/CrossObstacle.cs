using UnityEngine;
using MyBox;
using Shapes;

public class CrossObstacle : Obstacle
{
    [Header ("Specific")]
    [SerializeField, PositiveValueOnly] private float minSize = 1.5f;
    [SerializeField, PositiveValueOnly] private float maxSize = 3.5f;
    private float size = 0f;
    
    protected override void SetupObstacle()
    {
        base.SetupObstacle();

        size = Random.Range(minSize, maxSize);

        foreach (ObstaclePart part in parts)
        {
            Transform partTransform = part.transform;

            Rectangle rectangle = (Rectangle)part.shape;
            rectangle.Width = size;

            BoxCollider2D boxCollider = (BoxCollider2D)part.collider;
            boxCollider.offset = new Vector2 (size / 2f, rectangle.Thickness / 2f);
            boxCollider.size = new Vector2 (size, rectangle.Thickness);
        }
    }
}
