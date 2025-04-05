using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] LayerMask rockLayerMask;
    bool canDig = false;
    float currentControl = 0f;
    public float CurrentControl
    {
        get { return currentControl; }

        set
        {
            currentControl = Mathf.Clamp(value, 0, 1);
            OnChangeControl?.Invoke(currentControl);
        }
    }
    public static event Action<float> OnChangeControl;

    [SerializeField, Range(0f, 5f)] float speed;

    private static float depth = 0f;
    public static float Depth
    {
        get
        {
            return depth;
        }

        set
        {
            depth = value;
            OnDepthChanged?.Invoke(depth);
        }
    }

    public static event Action<float> OnDepthChanged;
    public static event Action<float, float> OnEnergyChanged;

    private static float _maxEnergy = 0;
    public static float MaxEnergy
    {
        get { return _maxEnergy; }
        set
        {
            _maxEnergy = value;
            OnEnergyChanged?.Invoke(MaxEnergy, CurrentEnergy);
        }
    }
    public static float _currentEnergy;

    public static float CurrentEnergy
    {
        get { return _currentEnergy; }
        set
        {
            _currentEnergy = value;
            OnEnergyChanged?.Invoke(MaxEnergy, CurrentEnergy);
        }
    }

    public static float _energyUsePerSecond = 1f;
    private static bool isOutOfEnergy => CurrentEnergy <= 0f;

    public static float EnergyUsePerSecond
    {
        get { return _energyUsePerSecond; }
        set { _energyUsePerSecond = value; }
    }

    private float damagePerClick = 2.5f;
    void Awake()
    {
        ResetEnergy();
    }
    public void StartDigging()
    {
        canDig = true;
    }

    public void ResetEnergy()
    {
        CurrentEnergy = MaxEnergy;
    }

    public void EndDigging()
    {
        canDig = false;
    }

    void Update()
    {
        if (!canDig || isOutOfEnergy)
            return;

        float move = CurrentControl * speed * Time.deltaTime;

        CurrentEnergy -= move * EnergyUsePerSecond;
        CurrentEnergy = CurrentEnergy < 0f ? 0f : CurrentEnergy;



        Depth += move;
        Depth = Depth < 0f ? 0f : Depth;


    }

    public void HandleMining()
    {

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Rock rock = hit.collider.GetComponentInParent<Rock>();
            if (rock != null)
            {
                if (rock.hitEffectPrefab)
                {
                    GameObject effect = Instantiate(rock.hitEffectPrefab, hit.point, Quaternion.identity);
                    effect.transform.parent = rock.transform;
                }
                rock.Dig(damagePerClick);
                
            }
        }
    }

    void OnDestroy()
    {
        OnChangeControl = null; // Clear event subscriptions
        OnDepthChanged = null;
        OnEnergyChanged = null;
    }


}


