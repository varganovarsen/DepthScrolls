using Unity.VisualScripting;
using UnityEngine;

public class Brilliant : MonoBehaviour, IPickupable
{
    [SerializeField] public GameObject pickupEffectPrefab;
    public Collider2D collider;
    [SerializeField] Transform gfx;
    [SerializeField] float pickupTime = 1f;
    public float depth;

    private void Update()
    {
        if (GameController.Instance.player.Depth >= depth)
        {
            PickUp();
        }
    }

    public bool PickUp()
    {

        if (pickupEffectPrefab)
        {
            Instantiate(pickupEffectPrefab, transform.position, transform.rotation);
        }

        StartCoroutine(IPickupableExtensions.PickupAnimation(gfx.transform, pickupTime));
        GameController.Instance.StartWinAnimation(pickupTime + 1f);

        Destroy(gameObject, pickupTime + .1f);

        collider.enabled = false;
        return true;

    }





    protected virtual void Awake()
    {
        collider = GetComponent<Collider2D>();

    }


    private void OnLevelReset()
    {
        Destroy(gameObject);
    }


    void OnDisable()
    {
        DepthController.RemoveMovingObject(transform);
        GameController.OnLevelReset -= OnLevelReset;
    }

    void OnEnable()
    {
        GameController.OnLevelReset += OnLevelReset;
    }
}
