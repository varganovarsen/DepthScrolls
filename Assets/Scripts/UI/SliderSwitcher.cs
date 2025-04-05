using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSwitcher : MonoBehaviour
{
    private Slider _slider;

    [SerializeField] Gradient gradient;


    PlayerController playerController;
    void OnEnable()
    {
        playerController = GameController.Instance.player;
        playerController.OnChangeControl += OnControlChanged;
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    void Start()
    {
        _slider.value = playerController.CurrentControl;
    }

    void OnDisable()
    {
        playerController.OnChangeControl -= OnControlChanged;
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }

    private void OnControlChanged(float value) => _slider.value = value;

    public void OnSliderValueChanged(float value)
    {
        playerController.CurrentControl = value;
    }


    void Update()
    {
        UpdateSliderSense();
    }

    public void UpdateSliderSense()
    {
            _slider.fillRect.GetComponent<Image>().color = gradient.Evaluate(_slider.value);
    }
}