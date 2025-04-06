using System.Collections;
using UnityEngine;

public interface IPickupable
{
    public bool PickUp();

    
}

public class IPickupableExtensions{


    public const float PICKUP_ANIMATION_TIME = 0.3f;
    public static IEnumerator PickupAnimation(Transform gfx, float animationTime){
        gfx.parent = null;
        Vector3 startPosition = gfx.position;
        Vector3 startScale = gfx.localScale;

        float elapsedTime = 0f;

        while (elapsedTime < animationTime)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / animationTime);
            gfx.position = Vector3.Lerp(startPosition, GameController.Instance.player.transform.position, t);
            gfx.localScale = Vector3.Lerp(startScale, Vector3.zero, t);


            yield return null;
        }

        gfx.position = GameController.Instance.player.transform.position;
        gfx.localScale = Vector3.zero;

    }
}
