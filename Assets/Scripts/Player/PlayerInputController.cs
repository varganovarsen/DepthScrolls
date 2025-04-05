using System;
using JetBrains.Annotations;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : MonoBehaviour
{
    [SerializeField] float changeControlSpeed;
    const string changeSpeedActionName = "ChangeSpeed";
    const string digActionName = "DigRock";

    PlayerInput playerInput;


    private PlayerController player;


    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        player = GetComponent<PlayerController>();

    }

    public void OnChangeSpeed()
    {
        float mouseInput = playerInput.actions.FindAction(changeSpeedActionName).ReadValue<float>();
        player.CurrentControl += mouseInput * changeControlSpeed;

    }

    public void OnDigRock() => player.HandleMining();
}
