using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasscodeManager : Singleton<PasscodeManager>
{
    public AudioClip correct, wrong;
    private AudioSource audioSource;
    public string Passcode { get; private set; } = 1234.ToString();

    private string enteredPasscode = string.Empty;

    private bool validationInProgress = false;

    public string ClubsNumber => Passcode[0].ToString();
    public string DiamondsNumber => Passcode[1].ToString();
    public string HeartsNumber => Passcode[2].ToString();
    public string SpadesNumber => Passcode[3].ToString();

    public PasscodeDigitEntry[] DigitEntryFields;

    private string EnteredPasscode
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
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //CreateNewPasscode();
    }

    /// <summary>
    /// true if the passcode was correct, false otherwise
    /// </summary>
    public static event Action<bool> ValidationCompleted;

    public void CreateNewPasscode()
    {
        // create a random 4 digit passcode
        Passcode = string.Empty;
        for (int i = 0; i < 4; i++)
        {
            Passcode += UnityEngine.Random.Range(0, 10);
        }
        Debug.Log($"Passcode is {Passcode}");
    }

    public void Validate()
    {
        if (validationInProgress) { return; }
        validationInProgress = true;
        Debug.Log("Validating");
        if (enteredPasscode == Passcode)
        {
            StartCoroutine(SetUpNextVictimAfterDelay());
            audioSource.clip = correct;
            StartCoroutine(ResetInputFieldsAfterCooldown());
        }
        else
        {
            GameManager.Instance.IncurPenalty();
            StartCoroutine(ResetInputFieldsAfterCooldown());
            audioSource.clip = wrong;
        }
        ValidationCompleted?.Invoke(enteredPasscode == Passcode);
        ClearEnteredCode();
        audioSource.Play();
    }

    private IEnumerator ResetInputFieldsAfterCooldown()
    {
        yield return new WaitForSeconds(GameManager.Instance.timePenaltyDuration);
        ResetInputFields();
    }

    private void ResetInputFields()
    {
        for (int i = 0; i < DigitEntryFields.Length; i++)
        {
            DigitEntryFields[i].inputField.interactable = true;
            DigitEntryFields[i].ClearInputField();
            DigitEntryFields[i].wrongImage.SetActive(false);
        }
        DigitEntryFields[0].ActivateInputField();
    }

    private IEnumerator SetUpNextVictimAfterDelay()
    {
        yield return new WaitForSeconds(GameManager.Instance.DelayInSecondsBetweenVictims);
        GameManager.Instance.SetUpNextVictim();
    }

    public void EnterDigit(string entry)
    {
        EnteredPasscode += entry;
    }

    private void ClearEnteredCode()
    {
        enteredPasscode = string.Empty;
        validationInProgress = false;
    }

    private void OnNewVictimSpawned()
    {
        CreateNewPasscode();
        ClearEnteredCode() ;
        ResetInputFields();
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
