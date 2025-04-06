using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Treasure : RockFilling
{
    
    [SerializeField] int moneyReward;
    public override bool PickUp()
    {
        bool result = base.PickUp();

        MoneyController.Money += moneyReward;

        return result;
    }


}
