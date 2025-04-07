using UnityEngine;

public class SoundController : MonoBehaviour
{
    public void PlayeOneShot(FMODUnity.EventReference eventName, Vector3 position) =>  FMODUnity.RuntimeManager.PlayOneShot(eventName, position);
}
