using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

public class PasscodeDigitEntry : MonoBehaviour
{
    [SerializeField]
    public GameObject wrongImage;

    [SerializeField]
    private GameObject selectImage;

    private TMPro.TMP_InputField inputField;

    void Awake()
    {
        inputField = GetComponentInChildren<TMPro.TMP_InputField>();
        inputField.interactable = false;
    }

    public void SetSelectedInputField(bool value)
    {
        selectImage.SetActive(value);
    }

    public void ClearInputField()
    {
        inputField.text = string.Empty;
    }

    public void SetInputFieldText(string text)
    {
        inputField.text = text;
    }

    private void OnValidationCompleted(bool isSuccessful)
    {
        // if unsuccesful, we start a cooldown before re-enabling the input field
        // if successful, the victim spawned event will do it for us
        if (!isSuccessful)
        {
            wrongImage.SetActive(true);
            //StartCoroutine(ResetInputFieldAfterCooldown());
        }
    }

    private void OnVictimDied(Victim obj)
    {
        ClearInputField();
    }

    private void OnEnable()
    {
        ClearInputField();
        PasscodeManager.ValidationCompleted += OnValidationCompleted;
        GameManager.VictimDied += OnVictimDied;
        GameManager.ChangedToKillingChannel += OnChangedToKillingChannel;
    }

    private void OnChangedToKillingChannel()
    {
        if (!GameManager.Instance.BlockPasscodeInput)
        {
            inputField.interactable = true;
        }
        ClearInputField();
    }

    private void OnDisable()
    {
        PasscodeManager.ValidationCompleted -= OnValidationCompleted;
        GameManager.VictimDied -= OnVictimDied;
        GameManager.ChangedToKillingChannel -= OnChangedToKillingChannel;
    }
}
