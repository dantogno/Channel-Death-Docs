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
    float lowestPitch, lowestCutoff;

    [SerializeField]
    Volume postProcess;

    bool duckVolume;

    [SerializeField]
    float volumeRecoverRate;

    float initialVolume;

    Vector3 GoalPos;

    AudioLowPassFilter filter;
    AudioReverbFilter reverbFilter;
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GoalPos = MazeGenerator.Instance.GetGoalPos();
        musicSource = GetComponent<AudioSource>();        
        initDis = Vector3.Distance(GoalPos, MazePlayerController.Instance.transform.position);
        initialVolume = musicSource.volume;
        filter = GetComponent<AudioLowPassFilter>();
        reverbFilter = GetComponent<AudioReverbFilter>();
    }

    float currentVolume;
    // Update is called once per frame
    void Update()
    {
        if(MazeGenerator.Instance == null)
        {
            Debug.LogError("MAZEGENERATORISNULL");
            return;
        }
        if (!MazeGenerator.Instance.IsPlayerInEndingHall())
        {
            float dis = Vector3.Distance(GoalPos, MazePlayerController.Instance.transform.position);
            float ratio = dis / initDis;
            float weight = (1 - ratio);
            Mathf.Clamp(weight, 0, 1);
            float cutoff = MapValueReverseExponential(weight, 0, 1, lowestCutoff, 22000);
            float pitch = lowestPitch + (1 - lowestPitch) * (1 - ratio) + 0.1f;
            pitch = Mathf.Clamp(pitch, 0.5f, 1.0f);
            musicSource.pitch = pitch;
            Mathf.Clamp(weight, 0, 0.8f);
            postProcess.weight = weight;
            //Mathf.Clamp(cutoff, )
            filter.cutoffFrequency = cutoff;
            float dry = MapValue(weight, 0, 1, -10000, -100);
            reverbFilter.dryLevel = dry;
        }
        if(JumpScare.Instance != null)
        {
            if (JumpScare.Instance.ScareComplete)
            {
                postProcess.weight -= 0.02f;
                if(postProcess.weight < 0)
                {
                    postProcess.weight = 0;
                }
            }
        }
    }

    public float MapValue(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // First, normalize the input value within the original range
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);

        // Then, scale the normalized value to the new range
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;

        return mappedValue;
    }

    private void OnDisable()
    {
        if (musicSource != null)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }
    }

    private void OnEnable()
    {
        if(musicSource != null)
        {
            musicSource.Play();
        }
    }

    public static float MapValueReverseExponential(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // First, normalize the input value within the original range
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);

        // Apply reverse exponential scaling
        normalizedValue = Mathf.Pow(normalizedValue, 3f);

        // Then, scale the normalized value to the new range
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;

        return mappedValue;
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
            musicSource.volume = currentVolume;
            Debug.Log("VOL " + currentVolume);
        }
    }

    public void DuckMusicVolume()
    {
        duckVolume = true;
        musicSource.volume = 0.01f;
    }
}
