using System;
using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour
{
    const float INITIAL_HEALTH = 10f;
    float health = INITIAL_HEALTH;

    [SerializeField] public GameObject hitEffectPrefab;

    [SerializeField] private AnimationCurve hitAnimationCurve;
    [SerializeField] private float hitAnimationDuration = 0.2f;

    Vector3 basicRotation;
    Vector3 basicScale;

    Coroutine hitAnimationCoroutine;

    void Awake()
    {
        basicRotation = transform.localEulerAngles;
        basicScale = transform.localScale;
    }

    public float depth;
    void OnEnable()
    {
        health = INITIAL_HEALTH;
        DepthController.AddMovingObject(new MovableObject(depth, transform));
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

        if (health <= 0f)
        {
            ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem particle in particles)
            {
                particle.gameObject.transform.parent = null;
            }

            gameObject.SetActive(false);
        }
    }

    private IEnumerator OnHitAnimation()
    {

        bool isMiddleDone = false;

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
            if (curveValue == 1f)
                isMiddleDone = true;

            if (!isMiddleDone)
            {
                transform.localEulerAngles = Vector3.Lerp(startRotation, middleRotation, curveValue);
                transform.localScale = Vector3.Lerp(startScale, middleScale, curveValue);
            }
            else
            {
                transform.localEulerAngles = Vector3.Lerp(middleRotation, basicRotation, curveValue);
                transform.localScale = Vector3.Lerp(middleScale, basicScale, curveValue);
            }

            yield return null;
        }

        transform.localEulerAngles = basicRotation;
        transform.localScale = basicScale;

    }



}
