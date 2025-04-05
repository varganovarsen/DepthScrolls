using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Controller : MonoBehaviour
{
    [SerializeField] Button startDigButton;
    [SerializeField] Button endDigButton;
    [SerializeField] GameObject sliderPrefab;

    public void OnEnable()
    {
        startDigButton.gameObject.SetActive(true);
        endDigButton.gameObject.SetActive(false);

        startDigButton.onClick.AddListener(OnStartDigButtonClick);
        endDigButton.onClick.AddListener(OnEndDigButtonClick);
    }

    void OnDisable()
    {
        startDigButton.onClick.RemoveAllListeners();
        endDigButton.onClick.RemoveAllListeners();
    }

    public void OnStartDigButtonClick()
    {
        GameController.Instance.StartDig();
        endDigButton.gameObject.SetActive(true);
        startDigButton.gameObject.SetActive(false);
    }

    public void OnEndDigButtonClick()
    {
        GameController.Instance.EndDig();
        endDigButton.gameObject.SetActive(false);
        startDigButton.gameObject.SetActive(true);
    }

}
