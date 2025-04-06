using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UI_Fader : MonoBehaviour
{
    [SerializeField] float fadeDuration = 1f;
    public float FadeDuration { get => fadeDuration; set => fadeDuration = value; }

    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }



    public void SetToOpaue()
    {
        canvasGroup.alpha = 1f;
        ToggleInteractivity(true);
    }

    public void SetToTransparent()
    {
        canvasGroup.alpha = 0f;
        ToggleInteractivity(false);
    }

    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeAlpha(canvasGroup.alpha, 0f, FadeDuration));
    }

    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(ChangeAlpha(canvasGroup.alpha, 1f, FadeDuration));
    }

    private IEnumerator ChangeAlpha(float from, float to, float duration)
    {
        bool setInteractivityTo = to == 0f ? false : true;

        ToggleInteractivity(!setInteractivityTo);


        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            canvasGroup.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        ToggleInteractivity(setInteractivityTo);
    }

    public void ToggleInteractivity(bool setTo)
    {
        canvasGroup.interactable = setTo;
        canvasGroup.blocksRaycasts = setTo;
    }
}
