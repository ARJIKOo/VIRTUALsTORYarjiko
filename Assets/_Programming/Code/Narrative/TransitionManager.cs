using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    public Image fadeOverlay;
    public float fadeOutTime = 1f;
    public float fadeInTime = 2f;

    public bool IsTransitioning { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void TransitionToNextEvent(System.Action onMidTransition)
    {
        StartCoroutine(DoTransition(onMidTransition));
    }

    private IEnumerator DoTransition(System.Action onMidTransition)
    {
        yield return StartCoroutine(FadeOut());
        onMidTransition?.Invoke();
        yield return StartCoroutine(FadeIn());
    }

    public IEnumerator FadeOut()
    {
        IsTransitioning = true;
        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.raycastTarget = true;

        Color c = fadeOverlay.color;
        c.a = 0;
        fadeOverlay.color = c;

        float t = 0;
        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, t / fadeOutTime);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = 1;
        fadeOverlay.color = c;
    }

    public IEnumerator FadeIn()
    {
        Color c = fadeOverlay.color;
        c.a = 1;
        fadeOverlay.color = c;

        float t = 0;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(1, 0, t / fadeInTime);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = 0;
        fadeOverlay.color = c;

        fadeOverlay.raycastTarget = false;
        IsTransitioning = false;
    }
}