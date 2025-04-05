using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool canDig = false;
    float currentControl = 0f;
    public float CurrentControl
    {
        get { return currentControl; }

        set
        {
            currentControl = Mathf.Clamp(value, 0, 1);
            OnChangeControl?.Invoke(currentControl);
            Debug.Log($"CurrentControl: {currentControl}");
        }
    }
    public event Action<float> OnChangeControl;

    [SerializeField, Range(0f, 5f)] float speed;

    private float depth = 0f;
    public float Depth
    {
        get
        {
            return depth;
        }

        set
        {
            depth = value;
            OnDepthChanged?.Invoke(depth);
        }
    }

    public static event Action<float> OnDepthChanged;



    public void StartDigging()
    {
        canDig = true;
    }

    public void EndDigging()
    {
        canDig = false;
    }

    void Update()
    {
        if (!canDig)
            return;

        float move = CurrentControl * speed * Time.deltaTime;

        Depth += move;
        Depth = Depth < 0f? 0f : Depth;

    }
}
