using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PeriodicPulse : MonoBehaviour
{
    Image image; // Reference to your image component
    float repeatDelay = 5;
    bool hasStarted;
    void Start()
    {
        image = GetComponent<Image>();
        // Set initial properties
        image.rectTransform.localScale = new Vector3(4f, 4f, 1f);
        image.color = new Color(0.5f, 0.5f, 0.5f, 0f);
        if (!hasStarted)
        {
            StartCoroutine(repeatAnim());
            hasStarted = true;
        }
    }

    private void OnEnable()
    {
        if(hasStarted)
        {
            StartCoroutine(repeatAnim());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator repeatAnim()
    {
        while (true)
        {
            // Set initial properties
            image.rectTransform.localScale = new Vector3(4f, 4f, 1f);
            //image.color = new Color(1f, 1f, 1f, 0f);

            // Define the animation sequence
            LeanTween.scale(image.rectTransform, new Vector3(96f, 96f, 1f), 1.4f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.alpha(image.rectTransform, 0.2f, 1.4f).setEase(LeanTweenType.easeOutQuad);
            LeanTween.scale(image.rectTransform, new Vector3(120f, 120f, 1f), 0.6f).setEase(LeanTweenType.easeInQuad).setDelay(1.4f);
            LeanTween.alpha(image.rectTransform, 0f, 0.6f).setEase(LeanTweenType.easeInQuad).setDelay(1.4f);

            // Remove the image object when the animation completes


            // Wait for the animation to finish
            yield return new WaitForSeconds(2.0f);

            // Wait for the specified delay before repeating the animation
            yield return new WaitForSeconds(repeatDelay);
        }
    }
}
