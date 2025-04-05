using UnityEngine;

public class BackgroungController : MonoBehaviour
{
    [SerializeField] float minY;
    [SerializeField] Transform bgSpriteTransform;
    PlayerController playerController;

    void OnEnable()
    {
        PlayerController.OnDepthChanged += OnDepthChanged;
    }

    void OnDisable()
    {

        PlayerController.OnDepthChanged -= OnDepthChanged;
    }


    private void OnDepthChanged(float depth){
        Vector3 newBGPos = bgSpriteTransform.position;
        float y = Mathf.Clamp(depth + minY, minY, 0f);
        newBGPos.y = y;
        bgSpriteTransform.position = newBGPos;

    }    
}
