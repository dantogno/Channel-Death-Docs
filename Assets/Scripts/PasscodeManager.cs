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

    public AudioClip correct, wrong;
    private AudioSource audioSource;
    public string Passcode { get; private set; } = 1234.ToString();

    private string enteredPasscode = string.Empty;

    private bool validationInProgress = false;

    public string ClubsNumber => Passcode[0].ToString();
    public string DiamondsNumber => Passcode[1].ToString();
    public string HeartsNumber => Passcode[2].ToString();
    public string SpadesNumber => Passcode[3].ToString();

    [Tooltip("Must be in order from left to right")]
    public PasscodeDigitEntry[] DigitEntryFields;

    private int passcodeEntryIndex = 0;

    //private string EnteredPasscode
    //{
    //    get => enteredPasscode;
    //    set
    //    {
    //        enteredPasscode = value;
    //        if (enteredPasscode.Length == 4)
    //        {
    //            Validate();
    //        }
    //    }
    //}
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        CreateNewPasscode();
        ResetInputFields();
    }

    private void Update()
    {
        if (!GameManager.Instance.BlockPasscodeInput && GameManager.Instance.ChannelIndex == GameManager.Instance.KillingFloorChannelIndex)
        {
            if (InputManager.InputActions.Gameplay.Input0.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "0");
            }
            if (InputManager.InputActions.Gameplay.Input1.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "1");
            }
            if (InputManager.InputActions.Gameplay.Input2.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "2");
            }
            if (InputManager.InputActions.Gameplay.Input3.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "3");
            }
            if (InputManager.InputActions.Gameplay.Input4.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "4");
            }
            if (InputManager.InputActions.Gameplay.Input5.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "5");
            }
            if (InputManager.InputActions.Gameplay.Input6.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "6");
            }
            if (InputManager.InputActions.Gameplay.Input7.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "7");
            }
            if (InputManager.InputActions.Gameplay.Input8.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "8");
            }
            if (InputManager.InputActions.Gameplay.Input9.WasPressedThisFrame())
            {
                AddInputToDigitEntryField(DigitEntryFields[passcodeEntryIndex], "9");
            }
        }
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
            StartCoroutine(ResetInputFieldsAfterPenaltyCooldown());
            audioSource.clip = wrong;
        }
        ClearEnteredCode();
        audioSource.Play();
        ValidationCompleted?.Invoke(isCorrectCode);
    }


    private IEnumerator ResetInputFieldsAfterPenaltyCooldown()
    {
        yield return new WaitForSeconds(GameManager.Instance.timePenaltyDuration);
        ResetInputFields();
    }

    public void ResetInputFields()
    {
        for (int i = 0; i < DigitEntryFields.Length; i++)
        {
            //if (!GameManager.Instance.BlockPasscodeInput)
            //    DigitEntryFields[i].inputField.interactable = true;
            DigitEntryFields[i].ClearInputField();
            DigitEntryFields[i].wrongImage.SetActive(false);
        }
        passcodeEntryIndex = 0;

        if (!GameManager.Instance.BlockPasscodeInput)
            DigitEntryFields[0].SetSelectedInputField(true);
    }

    private void AddInputToDigitEntryField(PasscodeDigitEntry digitEntryField, string input)
    {
        digitEntryField.SetInputFieldText(input);
        enteredPasscode += input;
        DigitEntryFields[passcodeEntryIndex].SetSelectedInputField(false);

        if (passcodeEntryIndex == DigitEntryFields.Length - 1)
        {
            passcodeEntryIndex = 0;
            Validate();
        }
        else
        {
            passcodeEntryIndex++;
            DigitEntryFields[passcodeEntryIndex].SetSelectedInputField(true);
        }
    }

    private void ClearEnteredCode()
    {
        enteredPasscode = string.Empty;
        validationInProgress = false;
    }  

    private void OnNewVictimSpawned()
    {
        ClearEnteredCode() ;
        ResetInputFields();
    }

    private void OnChangedToKillingChannel()
    {
        ResetInputFields();
    }

    private void OnEnable()
    {
        GameManager.NewVictimSpawned += OnNewVictimSpawned;
        GameManager.ChangedToKillingChannel += ClearEnteredCode;
        GameManager.ChangedToKillingChannel += OnChangedToKillingChannel;
    }


    private void OnDisable()
    {
        GameManager.NewVictimSpawned -= OnNewVictimSpawned;
        GameManager.ChangedToKillingChannel -= ClearEnteredCode;
        GameManager.ChangedToKillingChannel -= OnChangedToKillingChannel;
    }
}
