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

    Vector3 GoalPos;

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
    }

    float currentVolume;
    // Update is called once per frame
    void Update()
    {
        if (!MazeGenerator.Instance.IsPlayerInEndingHall())
        {
            float dis = Vector3.Distance(GoalPos, MazePlayerController.Instance.transform.position);
            float ratio = dis / initDis;
            float weight = (1 - ratio);
            Mathf.Clamp(weight, 0, 1);
            float pitch = lowestPitch + (1 - lowestPitch) * (1 - ratio) + 0.1f;
            pitch = Mathf.Clamp(pitch, 0.5f, 1.0f);
            musicSource.pitch = pitch;
            Mathf.Clamp(weight, 0, 0.8f);
            postProcess.weight = weight;
        }
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
        }
    }

    public void DuckMusicVolume()
    {
        duckVolume = true;
        musicSource.volume = 0;
    }
}
