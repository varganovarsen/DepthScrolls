using UnityEngine;

public class EnergyContainer : RockFilling
{

    [SerializeField] float energy;

    public override bool PickUp()
    {
        base.PickUp();

        GameController.Instance.player.CurrentEnergy += energy;
        return true;
    }
}
