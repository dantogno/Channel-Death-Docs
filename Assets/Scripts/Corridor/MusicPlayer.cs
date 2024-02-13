using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer instance;

    private AudioSource musicSource;
    float initDis;
    [SerializeField]
    float lowestPitch;

    [SerializeField]
    Volume postProcess;

    bool duckVolume;

    [SerializeField]
    float volumeRecoverRate;

    float initialVolume;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();        
        initDis = Vector3.Distance(transform.position, MazePlayerController.Instance.transform.position);
        initialVolume = musicSource.volume;
    }

    float currentVolume;
    // Update is called once per frame
    void Update()
    {
        float dis = Vector3.Distance(transform.position, MazePlayerController.Instance.transform.position);
        float ratio = dis / initDis;
        float weight = (1 - ratio);
        Mathf.Clamp(weight, 0, 1);
        float pitch = lowestPitch + (1-lowestPitch)* (1 - ratio) + 0.1f;
        pitch = Mathf.Clamp(pitch, 0.5f, 1.0f);
        musicSource.pitch = pitch;
        postProcess.weight = weight;

    }

    private void FixedUpdate()
    {
        if (duckVolume)
        {
            currentVolume += volumeRecoverRate;
            if (currentVolume > initialVolume)
            {
                currentVolume = initialVolume;
                duckVolume = false;
            }
            Debug.Log(currentVolume);
            musicSource.volume = currentVolume;
        }
    }

    public void DuckMusicVolume()
    {
        duckVolume = true;
        musicSource.volume = 0;
    }
}
