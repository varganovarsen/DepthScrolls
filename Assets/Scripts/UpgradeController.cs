using System.Collections.Generic;
using UnityEngine;

public class UpgradeController
{
    public Dictionary<UpgradeType, float> upgradeCosts = new Dictionary<UpgradeType, float>()
    {
        {UpgradeType.Energy, UpgradeConfig.Instance.energyUpgradeCost},
        {UpgradeType.Dig, UpgradeConfig.Instance.digUpgradeCost},
        {UpgradeType.Speed, UpgradeConfig.Instance.speedUpgradeCost},
        {UpgradeType.Sonar, UpgradeConfig.Instance.sonarUpgradeCost}
    };
    public Dictionary<UpgradeType, float> previousUpgradeCosts = new Dictionary<UpgradeType, float>()
    {
        {UpgradeType.Energy, UpgradeConfig.Instance.energyUpgradeCost},
        {UpgradeType.Dig, UpgradeConfig.Instance.energyUpgradeCost},
        {UpgradeType.Speed, UpgradeConfig.Instance.energyUpgradeCost},
        {UpgradeType.Sonar, UpgradeConfig.Instance.energyUpgradeCost}
    };

    public Dictionary<UpgradeType, int> upgradeTiers = new Dictionary<UpgradeType, int>()
    {
        {UpgradeType.Energy, 0},
        {UpgradeType.Dig, 0},
        {UpgradeType.Speed, 0},
        {UpgradeType.Sonar, 0}
    };

    public bool Upgrade(UpgradeType upgradeType)
    {

        if(MoneyController.Money < upgradeCosts[upgradeType])
            return false;


        MoneyController.Money -= Mathf.FloorToInt(upgradeCosts[upgradeType]);


        upgradeTiers[upgradeType]++;
        float upgradeCost = upgradeCosts[upgradeType];

        float modifier =  UpgradeConfig.Instance.upgradeCostCurve.Evaluate(Mathf.Clamp01(upgradeTiers[upgradeType] / UpgradeConfig.Instance.upgradeCountCeil));
        // Debug.Log(modifier);

        upgradeCosts[upgradeType] = RoundOff( i: Mathf.RoundToInt(upgradeCost + previousUpgradeCosts[upgradeType] * modifier), roundTo: 50);


        previousUpgradeCosts[upgradeType] = upgradeCost;


        switch (upgradeType)
        {
            case UpgradeType.Energy:
                GameController.Instance.player.MaxEnergy += UpgradeConfig.Instance.energyPerUpgrade;
                GameController.Instance.player.ResetEnergy();
                break;
            case UpgradeType.Dig:
                GameController.Instance.player.DamagePerClick += UpgradeConfig.Instance.digDamagePerUpgrade;
                break;
            case UpgradeType.Speed:
                GameController.Instance.player.Speed += UpgradeConfig.Instance.speedPerUpgrade;
                break;
            case UpgradeType.Sonar:
                break;
        }

        return true;
    }

    float easeInSine(float number)
    {
        return 1 - Mathf.Cos((number * Mathf.PI) / 2);
    }

    public static int RoundOff (int i, int roundTo)
    {
        return ((int)Mathf.Round(i / roundTo)) * roundTo;
    }
  
}





public enum UpgradeType
{
    Energy,
    Dig,
    Speed,
    Sonar
}
