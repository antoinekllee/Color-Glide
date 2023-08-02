using UnityEngine;
using MyBox;
using Shapes;

public class SquareObstacle : Obstacle
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
            boxCollider.size = new Vector2 (size, rectangle.Thickness);

            float offset = (size / 2f) - (rectangle.Thickness / 2f); 
            
            if (part.id == 0)
                partTransform.localPosition = new Vector2 (0f, partTransform.localPosition.y > 0 ? offset : -offset);
            else 
                partTransform.localPosition = new Vector2 (partTransform.localPosition.x > 0 ? offset : -offset, 0f);
        }
    }
}
