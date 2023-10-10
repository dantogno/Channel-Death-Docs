using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasscodeDigitEntry : MonoBehaviour
{
    public bool isFirstDigit;

    private TMPro.TMP_InputField inputField;

    void Awake()
    {
        inputField = GetComponentInChildren<TMPro.TMP_InputField>();

    }

    // coroutine to delay activation of first digit for one frame
    private IEnumerator DelayBeforeActivating()
    {
        yield return null;
        ActivateInputField();
    }

    public void ActivateInputField()
    {
        inputField.ActivateInputField();
    }

    public void ResetInputField()
    {
        inputField.text = string.Empty;
    }

    private void OnValidationCompleted(bool isSuccessful)
    {
        if (isSuccessful)
        {
            inputField.interactable = false;
        }
        else
        {
            ResetInputField();
            if (isFirstDigit)
            {
                ActivateInputField();
            }
        }
    }
    private void OnNewVictimSpawed()
    {
        inputField.interactable = true;
        ResetInputField();
        if (isFirstDigit)
        {
            ActivateInputField();
        }
    }
    private void OnVictimDied(string obj)
    {
        inputField.interactable = false;
        ResetInputField();
    }

    private void OnEnable()
    {
        ResetInputField();
        if (isFirstDigit) { StartCoroutine(DelayBeforeActivating()); }
        PasscodeManager.ValidationCompleted += OnValidationCompleted;
        GameManager.NewVictimSpawned += OnNewVictimSpawed;
        GameManager.VictimDied += OnVictimDied;
    }


    private void OnDisable()
    {
        PasscodeManager.ValidationCompleted -= OnValidationCompleted;
        GameManager.NewVictimSpawned -= OnNewVictimSpawed;
        GameManager.VictimDied -= OnVictimDied;
    }
}
