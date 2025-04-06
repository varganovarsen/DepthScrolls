using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_FinePanel : MonoBehaviour
{
    [SerializeField] TMP_Text fineText;
    [SerializeField] Button finePanelButton;
    int fine;

    public bool isClicked;
    void Awake()
    {
        finePanelButton.onClick.AddListener(OnFinePanelButtonClick);
    }

    void OnDisable()
    {
        finePanelButton.onClick.RemoveAllListeners();
    }

    public void Initialize(int setFine, float depth) 
    {
        fine = setFine;
        fineText.text = $"{fine.ToString()}$ for {Mathf.RoundToInt(depth)}m"; 

        if(fine > MoneyController.Money){
            fineText.color = Color.red;
            finePanelButton.GetComponentInChildren<TMP_Text>().text = "Restart";
        }
        
    }

    public void OnFinePanelButtonClick()
    {
        isClicked = true;
        Destroy(gameObject);
    }
}
