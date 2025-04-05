using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] PlayerConfig playerConfig;

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

    public static float DamagePerClick { get => damagePerClick; set => damagePerClick = value; }
    public static float EnergyPerClick { get => energyPerClick; set => energyPerClick = value; }
    public static float EnergyPerMeter { get => energyPerMeter; set => energyPerMeter = value; }

    private static float damagePerClick = 2.5f;

    private static float energyPerClick = 3f;

    private static float energyPerMeter = .1f;

    public bool CanRocketLaunchToSurface => Depth - CurrentEnergy / EnergyPerMeter <= 0f;

    void Awake()
    {
        PlayerConfig playerConfig = Resources.Load<PlayerConfig>("PlayerConfig");

        if (playerConfig)
        {
            DamagePerClick = playerConfig.basicDamagePerClick;
            EnergyPerClick = playerConfig.basicEnergyPerClick;
            EnergyUsePerSecond = playerConfig.basicEnergyUsePerSecond;
            MaxEnergy = playerConfig.basicMaxEnergy;
            EnergyPerMeter = playerConfig.basicEnergyUsePerUnitDepth;
        } else
        {
            Debug.LogError("PlayerConfig not found");
            DamagePerClick = 999f;
            EnergyPerClick = 999f;
            EnergyUsePerSecond = 999f;
            MaxEnergy = 999f;
            EnergyPerMeter = 999f;
        }


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

        if (CurrentEnergy < EnergyPerClick)
            return;


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
                rock.Dig(DamagePerClick);

                CurrentEnergy -= EnergyPerClick;
                CurrentEnergy = CurrentEnergy < 0f ? 0f : CurrentEnergy;
            }
        }
    }


    public  IEnumerator RocketLaunch(float timeToRestart)
    {
        EndDigging();

        float startDepth = Depth;
        float endDepth = startDepth - CurrentEnergy / EnergyPerMeter;
        endDepth = endDepth < 0f ? 0f : endDepth;
        
        float startEnergy = CurrentEnergy;
        float endEnergy = CurrentEnergy - Depth * energyPerMeter;

        float elapsedTime = 0f;

        while (elapsedTime < timeToRestart)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / timeToRestart);
            Depth = Mathf.Lerp(startDepth, endDepth, t);
            CurrentEnergy = Mathf.Lerp(startEnergy, endEnergy, t);
            CurrentEnergy = CurrentEnergy < 0f ? 0f : CurrentEnergy;
            Debug.Log(CurrentEnergy);
            yield return null;
        }

        Depth = endDepth;



    }


    void OnDestroy()
    {
        OnChangeControl = null; // Clear event subscriptions
        OnDepthChanged = null;
        OnEnergyChanged = null;
    }


}


