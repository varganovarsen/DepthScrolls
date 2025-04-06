using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_UpgradeButton : MonoBehaviour
{
    [SerializeField] TMP_Text priceText;
    [SerializeField] Button upgradeButton;
    [SerializeField] UpgradeType upgradeType;
    ColorBlock basicColorBlock;
    bool basicColorsSet;

    void OnEnable()
    {
        if(!basicColorsSet){
            basicColorBlock = upgradeButton.colors;
            basicColorsSet = true;
        }

        UpdatePrice();
        upgradeButton.onClick.AddListener(OnUpgradeButtonClick);
    }

    void OnDisable()
    {
        upgradeButton.onClick.RemoveAllListeners();
    }

    public void OnUpgradeButtonClick()
    {
        GameController.Instance.UpgradeController.Upgrade(upgradeType);
        UpdatePrice();
    }

    public void UpdatePrice(){

        float price = GameController.Instance.UpgradeController.upgradeCosts[upgradeType];
        priceText.text = $"{price}$";

        if(MoneyController.Money < price){
            ColorBlock colorBlock = upgradeButton.colors;
            colorBlock.normalColor = Color.red;
            colorBlock.highlightedColor = Color.red;
            colorBlock.pressedColor = Color.red;
            colorBlock.selectedColor = Color.red;
            upgradeButton.colors = colorBlock;
        } else
        {
            upgradeButton.colors = basicColorBlock;
        }


    }

}
