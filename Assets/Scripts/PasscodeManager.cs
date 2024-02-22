using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.Rendering.DebugUI;

public class PasscodeManager : Singleton<PasscodeManager>
{
    /// <summary>
    /// true if the passcode was correct, false otherwise
    /// </summary>
    public static event Action<bool> ValidationCompleted;
    public static event Action<string> NewPasscodeSet;

    public string EnteredPasscode
    {
        get => enteredPasscode;
        set
        {
            enteredPasscode = value;
            if (enteredPasscode.Length == 4)
            {
                Validate();
            }
        }
    }

    public AudioClip correct, wrong;
    private AudioSource audioSource;
    public string Passcode { get; private set; } = 1234.ToString();

    private string enteredPasscode = string.Empty;

    private bool validationInProgress = false;

    public string ClubsNumber => Passcode[0].ToString();
    public string DiamondsNumber => Passcode[1].ToString();
    public string HeartsNumber => Passcode[2].ToString();
    public string SpadesNumber => Passcode[3].ToString();


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CreateNewPasscode();
    }

    private void CreateNewPasscode()
    {
        // create a random 4 digit passcode
        Passcode = string.Empty;
        for (int i = 0; i < 4; i++)
        {
            Passcode += UnityEngine.Random.Range(0, 10);
        }
        Debug.Log($"Passcode is {Passcode}");
        NewPasscodeSet?.Invoke(Passcode);
    }

    public void Validate()
    {
        if (validationInProgress) { return; }
        validationInProgress = true;
        Debug.Log("Validating");
        var isCorrectCode = enteredPasscode == Passcode;

        // correct code... victim rescued
        if (isCorrectCode)
        {
            GameManager.Instance.SpawnNewVictimAfterDelay();
            audioSource.clip = correct;
            CreateNewPasscode();
        }
        // incorrect code...
        else
        {
            GameManager.Instance.IncurPenalty();
            audioSource.clip = wrong;
        }
        ClearEnteredCode();
        audioSource.Play();
        ValidationCompleted?.Invoke(isCorrectCode);
    }


    private void ClearEnteredCode()
    {
        enteredPasscode = string.Empty;
        validationInProgress = false;
    }  

    private void OnNewVictimSpawned()
    {
        ClearEnteredCode() ;
    }

    private void OnEnable()
    {
        GameManager.NewVictimSpawned += OnNewVictimSpawned;
        GameManager.ChangedToKillingChannel += ClearEnteredCode;
    }


    private void OnDisable()
    {
        GameManager.NewVictimSpawned -= OnNewVictimSpawned;
        GameManager.ChangedToKillingChannel -= ClearEnteredCode;
    }
}
