using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class RockFilling : MonoBehaviour, IPickupable
{
    [SerializeField] public GameObject pickupEffectPrefab;
    [SerializeField] Transform gfx;
    public Collider2D collider;


    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();

    }


    private void OnLevelReset()
    {
        Destroy(gameObject);
    }

    public virtual bool PickUp()
    {
        if (pickupEffectPrefab)
        {
            Instantiate(pickupEffectPrefab, transform.position, transform.rotation);
        }


        Coroutine pickupAnimation = StartCoroutine(IPickupableExtensions.PickupAnimation(gfx, IPickupableExtensions.PICKUP_ANIMATION_TIME));


        collider.enabled = false;
        Destroy(gameObject, IPickupableExtensions.PICKUP_ANIMATION_TIME + .1f);

        return true;
    }


    void OnDisable()
    {
        DepthController.RemoveMovingObject(transform);
        GameController.OnLevelReset -= OnLevelReset;
    }

    void OnEnable()
    {
        GameController.OnLevelReset += OnLevelReset;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, UnityEngine.Random.Range(-180f, 180f)));
    }



}
