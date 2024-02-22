using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class Headlights : MonoBehaviour
{
    Light[] spotlights;

    [SerializeField]
    private float frequency;
    [SerializeField]
    private float frequencyVariation;
    [SerializeField]
    private float flickerSpeed;
    [SerializeField]
    private float flickerIntensity;

    [SerializeField]
    float brightness;
    // Start is called before the first frame update
    void Start()
    {
        spotlights = GetComponentsInChildren<Light>();
        StartCoroutine(flickerLight(spotlights[0]));
        StartCoroutine(flickerLight(spotlights[1]));
    }

    IEnumerator flickerLight(Light l)
    {
        setIntensity(l, flickerIntensity);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, brightness);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, flickerIntensity);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, 0);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, flickerIntensity);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, brightness);
        yield return new WaitForSeconds(frequency + Random.Range(0,frequencyVariation));
        StartCoroutine(flickerLight(l));
    }

    private void setIntensity(Light l, float intensity)
    {
        l.intensity = intensity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
