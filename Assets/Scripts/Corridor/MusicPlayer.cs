using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicPlayer : MonoBehaviour
{
    private AudioSource musicSource;
    float initDis;
    [SerializeField]
    float lowestPitch;
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
        float pitch = lowestPitch + (1-lowestPitch)* (1 - ratio) + 0.1f;
        pitch = Mathf.Clamp(pitch, 0.5f, 1.0f);
        musicSource.pitch = pitch;
    }
}
