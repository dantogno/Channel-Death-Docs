using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light light;

    [SerializeField]
    private float frequency;
    [SerializeField]
    private float frequencyVariation;
    [SerializeField]
    private float flickerSpeed, offTimeVariation;
    [SerializeField]
    private float flickerIntensity;

    [SerializeField]
    float brightness;

    MeshRenderer renderer;

    Material lightMat;

    AudioSource audioSource;

    float baseVol,basePitch;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        baseVol = audioSource.volume;
        basePitch = audioSource.pitch;
        light = GetComponentInChildren<Light>();
        renderer = GetComponent<MeshRenderer>();
        lightMat = renderer.material;
        StartCoroutine(flickerLight(light));
    }

    private void OnEnable()
    {
        if(light != null)
        {
            StartCoroutine(flickerLight(light));
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator flickerLight(Light l)
    {
        setIntensity(l, flickerIntensity);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, brightness);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, flickerIntensity);
        audioSource.pitch = 1;
        yield return new WaitForSeconds(flickerSpeed + offTimeVariation);
        setIntensity(l, 0);
        audioSource.volume = 0;
        renderer.enabled = false;
        yield return new WaitForSeconds(flickerSpeed);
        audioSource.volume = baseVol;
        renderer.enabled = true;
        setIntensity(l, flickerIntensity);
        yield return new WaitForSeconds(flickerSpeed);
        setIntensity(l, brightness);
        audioSource.pitch = basePitch;
        yield return new WaitForSeconds(frequency + Random.Range(0, frequencyVariation));
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
