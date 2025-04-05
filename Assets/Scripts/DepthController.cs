using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DepthController : MonoBehaviour
{
    List<MovableObject> movingObjects = new List<MovableObject>();

    void Awake()
    {
        PlayerController.OnDepthChanged += OnDepthChange;
    }

    void OnDisable()
    {
        PlayerController.OnDepthChanged -= OnDepthChange;
    }

    private void OnDepthChange(float currentDepth)
    {

        List<MovableObject> objectsToRemove = new List<MovableObject>();

        foreach (MovableObject obj in movingObjects)
        {
            if (currentDepth - obj.depth > RockController.GetCameraTopEdgeY() + 5f)
            {
                obj.transform.gameObject.SetActive(false);
                objectsToRemove.Add(obj);
                continue;
            }

            obj.transform.position = new Vector3(obj.transform.position.x, currentDepth - obj.depth, obj.transform.position.z);
        }

        foreach (MovableObject obj in objectsToRemove)
        {
            movingObjects.Remove(obj);
        }
    }

    public void AddMovingObject(MovableObject obj)
    {

        MovableObject existingObject = movingObjects.FirstOrDefault(m => m.transform == obj.transform);

        if (existingObject != null)
        {
            movingObjects.Remove(existingObject);
        }
        movingObjects.Add(obj);
    }

    // Vector3 newPos = startPos;
    // float y = depth - startDepth;
    // newPos.y += y;
    // transform.position = newPos;

}


public class MovableObject
{
    public float depth;
    public Transform transform;

    public MovableObject(float depth, Transform transform)
    {
        this.depth = depth;
        this.transform = transform;
    }
}
