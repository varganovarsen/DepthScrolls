using System;
using UnityEngine;

public class Rock : MonoBehaviour
{

    public float depth;
    void OnEnable()
    {
        GameController.Instance.depthController.AddMovingObject(new MovableObject(depth, transform));
    }

    void OnDisable()
    {
        GameController.Instance.depthController.RemoveMovingObject(transform);
    }
}
