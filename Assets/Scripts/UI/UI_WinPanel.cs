using System;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.UI;

public class UI_WinPanel : MonoBehaviour
{
    [SerializeField] Button restartButton;
    [SerializeField] Button continueButton;
    bool isClicked = false;

    public void SetUp(){
        restartButton.onClick.AddListener(OnRestartButtonClick);
        continueButton.onClick.AddListener(OnContinueButtonClick);
    }

    void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        continueButton.onClick.RemoveAllListeners();
    }

    public void OnRestartButtonClick()
    {
        isClicked = true;
        GameController.Instance.RestartGame();
        Destroy(gameObject);
    }
    public void OnContinueButtonClick()
    {
        isClicked = true;
        GameController.Instance.RestartLevelWithoutElevation();
        Destroy(gameObject);
    }
}
