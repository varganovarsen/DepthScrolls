using System;
using System.CodeDom.Compiler;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rock : MonoBehaviour
{
    [SerializeField] FMODUnity.EventReference hitEvent;

    private static GameObject[] rockFillingsPrefabs;

    [SerializeField] float initialHealth = 10f;
    float health;

    [SerializeField] public GameObject hitEffectPrefab;

    [SerializeField] private AnimationCurve hitAnimationCurve;
    [SerializeField] private float hitAnimationDuration = 0.2f;

    Vector3 basicRotation;
    Vector3 basicScale;

    Coroutine hitAnimationCoroutine;

    GameObject fillingObject;

    void Awake()
    {
        if (rockFillingsPrefabs == null)
        {
            rockFillingsPrefabs = Resources.LoadAll<GameObject>("Prefabs/RockFillings");
        }

        basicRotation = transform.localEulerAngles;
        basicScale = transform.localScale;
    }

    public float depth;

    public static GameObject[] RockFillingsPrefabs
    {
        get
        {

            if (rockFillingsPrefabs == null)
            {
                rockFillingsPrefabs = Resources.LoadAll<GameObject>("Prefabs/RockFillings");
            }
            return rockFillingsPrefabs;
        }
    }

    public void SetUp()
    {
        health = initialHealth;
        transform.rotation = Quaternion.Euler(basicRotation);
        transform.localScale = basicScale;
        DepthController.AddMovingObject(new MovableObject(depth, transform));
        GenerateFilling();
    }

    private void GenerateFilling()
    {
        if (fillingObject != null)
            Destroy(fillingObject);
        
        fillingObject = Instantiate(GetRandomFillingPrefab(), transform.position, transform.rotation);
        fillingObject.transform.parent = transform;
        fillingObject.SetActive(false);       
    }

    private GameObject GetRandomFillingPrefab()
    {
        SpawnOption currentOption = RockController.CurrentSpawnOption;
        float rnd = Random.value;
        float sum = 0f;
        for (int i = 0; i < currentOption.rockFillingPrefabs.Length; i++)
        {
            sum += currentOption.rockFillingChanses[i];

            if (rnd <= sum)
            {
                return currentOption.rockFillingPrefabs[i];
            }
        }

        return currentOption.rockFillingPrefabs[0];
    }

    void OnDisable()
    {
        DepthController.RemoveMovingObject(transform);
    }

    internal void Dig(float damage)
    {

        health -= damage;

        if (hitAnimationCoroutine != null)
        {
            StopCoroutine(hitAnimationCoroutine);
        }

        hitAnimationCoroutine = StartCoroutine(OnHitAnimation());

        GameController.Instance.soundController.PlayeOneShot(hitEvent, transform.position);
        if (health <= 0f)
        {

            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles)
            {
                particle.gameObject.transform.parent = null;
            }

            transform.localScale = basicScale;
            fillingObject.transform.parent = null;
            DepthController.AddMovingObject(new MovableObject(depth, fillingObject.transform));
            fillingObject.SetActive(true);
            fillingObject = null;


            Destroy(gameObject);
        }
    }

    private IEnumerator OnHitAnimation()
    {

        Vector3 startRotation = transform.localEulerAngles;
        Vector3 startScale = transform.localScale;

        Vector3 middleRotation = transform.localEulerAngles + new Vector3(0, 0, UnityEngine.Random.Range(-30f, 30f));
        Vector3 middleScale = transform.localScale * .8f;


        float elapsedTime = 0f;
        while (elapsedTime < hitAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / hitAnimationDuration;
            float curveValue = hitAnimationCurve.Evaluate(t);


            if (curveValue >= 1f)
                break;

            transform.localEulerAngles = Vector3.Lerp(startRotation, middleRotation, curveValue);
            transform.localScale = Vector3.Lerp(startScale, middleScale, curveValue);

            yield return null;
        }

        while (elapsedTime < hitAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / hitAnimationDuration;
            float curveValue = hitAnimationCurve.Evaluate(t);

            float rotationZ = Mathf.LerpAngle(middleRotation.z, basicRotation.z, 1 - curveValue);
            Vector3 rotation = transform.localEulerAngles;
            rotation.z = rotationZ;
            transform.localEulerAngles = rotation;
            transform.localScale = Vector3.Lerp(middleScale, basicScale, 1 - curveValue);

            yield return null;
        }


    }



}
