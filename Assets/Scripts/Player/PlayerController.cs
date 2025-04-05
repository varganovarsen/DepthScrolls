using UnityEngine;

public class PlayerController : MonoBehaviour
{
    bool canDig = false;


    public void StartDigging(){
        canDig = true;
    }

    public void EndDigging(){
        canDig = false;
    }

    void Update()
    {
        if(!canDig)
            return;
    }
}
