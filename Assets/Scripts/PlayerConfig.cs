using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [SerializeField] public float basicMaxEnergy;
    [SerializeField] public float basicEnergyUsePerSecond;
    [SerializeField] public float basicDamagePerClick;
    [SerializeField] public float basicEnergyPerClick;

    [SerializeField] public float basicEnergyUsePerUnitDepth;


    // PlayerController.MaxEnergy = 100;
    //     PlayerController.EnergyUsePerSecond = 1f;
    //     PlayerController.DamagePerClick = 1f;
    //     PlayerController.EnergyPerClick = 3f;
}
