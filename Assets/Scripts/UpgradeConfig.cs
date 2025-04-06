using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "Configs/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject
{
    private static UpgradeConfig instance;
    public static UpgradeConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<UpgradeConfig>("UpgradeConfig");
                if (instance == null)
                {
                    Debug.LogError("UpgradeConfig not found");
                }
            }
            return instance;
        }
    }

    [SerializeField] public float energyUpgradeCost;
    [SerializeField] public float energyPerUpgrade;
    [SerializeField] public float digUpgradeCost;
    [SerializeField] public float digDamagePerUpgrade;
    [SerializeField] public float speedUpgradeCost;
    [SerializeField] public float speedPerUpgrade;
    [SerializeField] public float sonarUpgradeCost;

    [SerializeField] public AnimationCurve upgradeCostCurve;
    [SerializeField] public int upgradeCountCeil = 10;




}
