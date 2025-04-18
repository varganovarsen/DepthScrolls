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
    [SerializeField] TMP_Text moneyText;

    [SerializeField] GameObject finePanelPrefab;
    [SerializeField] GameObject winPanelPrefab;

    private UI_Fader fader;
    public UI_Fader Fader => fader;

    const int NUM_OF_DIGITS_AFTER_DECIMAL_POINT = 1;

    PlayerController player;

    [SerializeField] GameObject upgradePanel;
    UI_UpgradeButton[] upgradeButtons;



    public void Init(PlayerController setPlayer)
    {
        if (player != null)
            Unsubscribe();

        player = setPlayer;
        fader = GetComponentInChildren<UI_Fader>();

        startDigButton.gameObject.SetActive(true);
        endDigButton.gameObject.SetActive(false);

        upgradeButtons = upgradePanel.GetComponentsInChildren<UI_UpgradeButton>();

        Subscribe();
    }

    public UI_FinePanel ShowFinePanel() => Instantiate(finePanelPrefab, transform).GetComponent<UI_FinePanel>();
    public UI_WinPanel ShowWinPanel() => Instantiate(winPanelPrefab, transform).GetComponent<UI_WinPanel>();

    public void ShowUpgradePanel()
    {
        StartCoroutine(UI_Fader.ChangeAlpha(upgradePanel.GetComponent<CanvasGroup>(), upgradePanel.GetComponent<CanvasGroup>().alpha, 1f, .3f));
        foreach (UI_UpgradeButton upgradeButton in upgradeButtons)
        {
            upgradeButton.UpdatePrice();
        }
    }

    public void HideUpgradePanel()
    {
        StartCoroutine(UI_Fader.ChangeAlpha(upgradePanel.GetComponent<CanvasGroup>(), upgradePanel.GetComponent<CanvasGroup>().alpha, 0f, .3f));
        // upgradePanel.SetActive(false);
    }

    IEnumerator Start()
    {
        // yield return new WaitUntil(() => PlayerController.MaxEnergy > 0f);
        yield return new WaitForSeconds(1f);
        UpdateEnergyMeter(player.MaxEnergy, player.CurrentEnergy);
        UpdateDepthText(player.Depth);
        UpdateMoneyText(MoneyController.Money);
    }

    void OnDisable()
    {
        Unsubscribe();
    }

    public void Subscribe(){
        startDigButton.onClick.AddListener(OnStartDigButtonClick);
        endDigButton.onClick.AddListener(OnEndDigButtonClick);

        player = GameController.Instance.player;

        player.OnEnergyChanged += UpdateEnergyMeter;
        player.OnDepthChanged += UpdateDepthText;
        MoneyController.OnMoneyChanged += UpdateMoneyText;
        GameController.OnLevelReset += ShowUpgradePanel;
    }

    private void Unsubscribe()
    {
        startDigButton.onClick.RemoveAllListeners();
        endDigButton.onClick.RemoveAllListeners();
        player.OnDepthChanged -= UpdateDepthText;
        player.OnEnergyChanged -= UpdateEnergyMeter;
        MoneyController.OnMoneyChanged -= UpdateMoneyText;
        GameController.OnLevelReset -= ShowUpgradePanel;
    }

    public void OnStartDigButtonClick()
    {
        GameController.Instance.StartDig();
        endDigButton.gameObject.SetActive(true);
        startDigButton.gameObject.SetActive(false);
        HideUpgradePanel();
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

    private void UpdateMoneyText(int money)
    {
        moneyText.text = money.ToString() + "$";
    }
}
