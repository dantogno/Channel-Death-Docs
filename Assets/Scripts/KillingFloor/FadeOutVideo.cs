using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FadeOutVideo : MonoBehaviour
{
    public float fadeOutDuration = 2;
    public float waitTimeBeforeFadeOut = 3;
    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
    }
    private void OnEnable()
    {
        StopAllCoroutines();
     
        //StartCoroutine(FadeAfterPlaying());
    }

    private void SetVideoAlpha(float value)
    {
        videoPlayer.targetCameraAlpha = value;
    }

    private IEnumerator FadeAfterPlaying()
    {
        yield return new WaitForSeconds(waitTimeBeforeFadeOut);
        LeanTween.value(gameObject, 1, 0, fadeOutDuration).setOnUpdate(SetVideoAlpha).setEase(LeanTweenType.easeOutCubic);
    }
}
