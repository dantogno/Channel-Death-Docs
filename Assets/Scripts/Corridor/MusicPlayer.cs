using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private AudioSource musicSource;
    float initDis;
    [SerializeField]
    float lowestPitch;

    [SerializeField]
    Volume postProcess;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();        
        initDis = Vector3.Distance(transform.position, MazePlayerController.Instance.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        float dis = Vector3.Distance(transform.position, MazePlayerController.Instance.transform.position);
        float ratio = dis / initDis;
        float weight = (1 - ratio);
        Debug.Log(weight);
        Mathf.Clamp(weight, 0, 1);
        float pitch = lowestPitch + (1-lowestPitch)* (1 - ratio) + 0.1f;
        pitch = Mathf.Clamp(pitch, 0.5f, 1.0f);
        musicSource.pitch = pitch;
        postProcess.weight = weight;
    }
}
