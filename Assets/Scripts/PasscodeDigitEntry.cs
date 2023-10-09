using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PasscodeDigitEntry : MonoBehaviour
{
    public bool isFirstDigit;

    private TMPro.TMP_InputField inputField;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GetComponentInChildren<TMPro.TMP_InputField>();

        if (isFirstDigit ) { StartCoroutine(DelayBeforeActivating()); }
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
        ResetInputField();
        if (isFirstDigit)
        {
            ActivateInputField();
        }
    }

    private void OnEnable()
    {
        PasscodeManager.ValidationCompleted += OnValidationCompleted;
    }


    private void OnDisable()
    {
        PasscodeManager.ValidationCompleted -= OnValidationCompleted;
    }
}
