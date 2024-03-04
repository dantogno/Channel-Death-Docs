using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreams : MonoBehaviour
{
    public AudioClip male, female;
    private AudioSource audioSource;
    public AudioClip SawKill;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        GameManager.VictimDied += OnVictimDied;
    }

    private void OnVictimDied(Victim obj)
    {
        AudioClip clip = obj.isFemale ? female: male;
        audioSource.PlayOneShot(clip);
        audioSource.clip = SawKill;
        audioSource.time = 0.42f;
        audioSource.Play();
    }

    private void OnDisable()
    {
        GameManager.VictimDied -= OnVictimDied;
    }
}
