using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{

    public static PlayerConfig instance;
    public static PlayerConfig Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<PlayerConfig>("PlayerConfig");
                if (instance == null)
                {
                    Debug.LogError("PlayerConfig not found");
                }
            }

            return instance;
        }
    }
    [SerializeField] public int startMoney = 100; 
    [SerializeField] public float basicMaxEnergy;
    [SerializeField] public float basicEnergyUsePerSecond;
    [SerializeField] public float basicDamagePerClick;
    [SerializeField] public float basicEnergyPerClick;

    [SerializeField] public float basicEnergyUsePerMeterRocket;
    [SerializeField] public float basicEnergyUsePerMeterDig;

    [SerializeField] public float basicMoneyPerMeter;


    // PlayerController.MaxEnergy = 100;
    //     PlayerController.EnergyUsePerSecond = 1f;
    //     PlayerController.DamagePerClick = 1f;
    //     PlayerController.EnergyPerClick = 3f;
}
