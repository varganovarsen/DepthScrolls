using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] Button startDigButton;
    [SerializeField] Button endDigButton;
    [SerializeField] TMP_Text energyText;
    [SerializeField] TMP_Text depthText;

    const int NUM_OF_DIGITS_AFTER_DECIMAL_POINT = 1;

    public void OnEnable()
    {
        startDigButton.gameObject.SetActive(true);
        endDigButton.gameObject.SetActive(false);

        startDigButton.onClick.AddListener(OnStartDigButtonClick);
        endDigButton.onClick.AddListener(OnEndDigButtonClick);

        PlayerController.OnEnergyChanged += UpdateEnergyMeter;
        PlayerController.OnDepthChanged += UpdateDepthText;

    }

    IEnumerator Start()
    {
        // yield return new WaitUntil(() => PlayerController.MaxEnergy > 0f);
        yield return new WaitForSeconds(1f);
        UpdateEnergyMeter(PlayerController.MaxEnergy, PlayerController.CurrentEnergy);
        UpdateDepthText(PlayerController.Depth);
    }

    void OnDisable()
    {
        startDigButton.onClick.RemoveAllListeners();
        endDigButton.onClick.RemoveAllListeners();
        PlayerController.OnDepthChanged -= UpdateDepthText;
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
        energyText.text = $"{Mathf.Floor(currentEnergy)} / {Mathf.Floor(maxEnergy)}"; 
    }

    private void UpdateDepthText(float depth)
    {
        float pow = Mathf.Pow(10, NUM_OF_DIGITS_AFTER_DECIMAL_POINT);
        depthText.text = (Mathf.Floor(depth * pow) / pow).ToString() + "m";
    }
}
