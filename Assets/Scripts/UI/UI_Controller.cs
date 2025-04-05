using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] Button startDigButton;
    [SerializeField] Button endDigButton;
    [SerializeField] TMP_Text energyText;

    public void OnEnable()
    {
        startDigButton.gameObject.SetActive(true);
        endDigButton.gameObject.SetActive(false);

        startDigButton.onClick.AddListener(OnStartDigButtonClick);
        endDigButton.onClick.AddListener(OnEndDigButtonClick);

        PlayerController.OnEnergyChanged += UpdateEnergyMeter;

    }

    void Start()
    {
        UpdateEnergyMeter(PlayerController.MaxEnergy, PlayerController.CurrentEnergy);
    }

    void OnDisable()
    {
        startDigButton.onClick.RemoveAllListeners();
        endDigButton.onClick.RemoveAllListeners();
        PlayerController.OnEnergyChanged -= UpdateEnergyMeter;
    }

    public void OnStartDigButtonClick()
    {
        GameController.Instance.StartDig();
        endDigButton.gameObject.SetActive(true);
        startDigButton.gameObject.SetActive(false);
    }

    public void OnEndDigButtonClick()
    {
        GameController.Instance.EndDig();
        endDigButton.gameObject.SetActive(false);
        startDigButton.gameObject.SetActive(true);
    }

    private void UpdateEnergyMeter(float maxEnergy, float currentEnergy)
    {
        energyText.text = Mathf.Floor(currentEnergy / maxEnergy * 100) + "%";
    }
}
