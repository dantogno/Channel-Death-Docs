using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInCanvasGroup : MonoBehaviour
{
    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        group.alpha = 0;
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float elaspedTime = 0;
        while (elaspedTime < 3f) {
            group.alpha = Mathf.Lerp(0, 1, elaspedTime / 3f);
            elaspedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
