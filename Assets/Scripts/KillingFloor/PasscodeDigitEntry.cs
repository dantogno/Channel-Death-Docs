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

    public void ClearInputField()
    {
        inputField.text = string.Empty;
    }

    private void OnValidationCompleted(bool isSuccessful)
    {
        inputField.interactable = false;

        // if unsuccesful, we start a cooldown before re-enabling the input field
        // if successful, the victim spawned event will do it for us
        if (!isSuccessful)
        {
            StartCoroutine(ResetInputFieldAfterCooldown());
        }

    }

    private IEnumerator ResetInputFieldAfterCooldown()
    {
        yield return new WaitForSeconds(0.25f);
        ResetInputField();
    }

    private void OnNewVictimSpawed()
    {
        ResetInputField();
    }

    private void ResetInputField()
    {
        inputField.interactable = true;
        ClearInputField();
        if (isFirstDigit)
        {
            ActivateInputField();
        }
    }

    private void OnVictimDied(string obj)
    {
        inputField.interactable = false;
        ClearInputField();
    }

    private void OnEnable()
    {
        ClearInputField();
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
