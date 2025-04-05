using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    public float DigInputValue = 0f;
    
    const string digActionName = "Dig";

    PlayerInput playerInput;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }


    public void OnDig()
    {
        DigInputValue = playerInput.actions.FindAction(digActionName).ReadValue<float>();
        Debug.Log(DigInputValue);
    }
}
