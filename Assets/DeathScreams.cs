using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathScreams : MonoBehaviour
{
    public AudioClip male, female;
    private AudioSource audioSource;
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
        audioSource.clip = obj.isFemale ? female: male;
        audioSource.Play();
    }

    private void OnDisable()
    {
        GameManager.VictimDied -= OnVictimDied;
    }
}
