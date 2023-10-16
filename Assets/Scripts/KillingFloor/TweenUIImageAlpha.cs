using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenUIImageAlpha : MonoBehaviour
{
    public float tweenDuration = 0.5f;
    // Reference to the Image component
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        // Fade out the image from 1 to 0 in 1 second
        LeanTween.alpha(image.rectTransform, 1f, tweenDuration).setEaseInCubic().setLoopPingPong();
    }
}
