using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasscodeManager : Singleton<PasscodeManager>
{
    public TMP_Text outcomeText;
    public string Passcode { get; private set; }

    private string enteredPasscode = string.Empty;

    private bool validationInProgress = false;

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

    /// <summary>
    /// true if the passcode was correct, false otherwise
    /// </summary>
    public static event Action<bool> ValidationCompleted;


    private void Start()
    {
        Passcode = "1234";
    }

    public void Validate()
    {
        if (validationInProgress) { return; }
        validationInProgress = true;
        Debug.Log("Validating");
        if (enteredPasscode == Passcode)
        {
            outcomeText.text = "Correct!";
            GameManager.Instance.SetUpNextVictim();
        }
        else
        {
            outcomeText.text = "Wrong!";
            GameManager.Instance.IncurPenalty();
        }
        ValidationCompleted?.Invoke(enteredPasscode == Passcode);
        ClearEnteredCode();
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

}
