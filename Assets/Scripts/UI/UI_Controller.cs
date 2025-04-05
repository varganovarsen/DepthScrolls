using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] Button startDigButton;

    public void OnEnable()
    {
        startDigButton.onClick.AddListener(GameController.Instance.StartDig);
    }

    void OnDisable()
    {
        startDigButton.onClick.RemoveAllListeners();
    }

}
