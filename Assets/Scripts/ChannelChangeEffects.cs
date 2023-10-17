using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ChannelChangeEffects : MonoBehaviour
{
    public float rampUpTime = 0.5f, rampDownTime = 0.5f;

    [SerializeField]
    private Volume channelChangeVolume;

    [SerializeField]
    private LeanTweenType easeType = LeanTweenType.easeInOutSine;

    // The delegate function that will be called every frame with the current tweened value
    //private void UpdateWeight(float value)
    //{
    //     Get the vignette effect from the volume profile
    //    Vignette vignette;
    //    if (volume.profile.TryGet(out vignette))
    //    {
    //         Set the weight parameter to the current tweened value
    //        vignette.weight.value = value;
    //    }
    //}

    private void SetWeight(float value)
    {
        channelChangeVolume.weight = value;
    }

    public void RampUpChannelChangeEffect()
    {
        LeanTween.value(gameObject, 0, 1, rampUpTime).setOnUpdate(SetWeight).setEase(easeType);
    }

    public void RampDownChannelChangeEffect()
    {
        LeanTween.value(gameObject, 1, 0, rampDownTime).setOnUpdate(SetWeight).setEase(easeType);
    }

    //private void OnRampUpCompleted()
    //{
    //    LeanTween.value(gameObject, 1, 0, rampDownTime).setOnUpdate(SetWeight).setEase(easeType);
    //}
    //private void OnChannelChangeStarted()
    //{
    //    LeanTween.value(gameObject, 0, 1, rampUpTime).setOnUpdate(SetWeight).setEase(easeType).setOnComplete(OnRampUpCompleted);
    //}

    //private void OnEnable()
    //{
    //    GameManager.ChannelChangeStarted += OnChannelChangeStarted;
    //}

    //private void OnDisable()
    //{
    //    GameManager.ChannelChangeStarted -= OnChannelChangeStarted;
    //}
}
