using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.UI;

public class PasscodeDigitEntry : MonoBehaviour
{
    public bool isFirstDigit;
    public GameObject wrongImage;

    public TMPro.TMP_InputField inputField;

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
            wrongImage.SetActive(true);
            //StartCoroutine(ResetInputFieldAfterCooldown());
        }
    }

    private void Update()
    {
        if (GameManager.Instance.BlockPasscodeInput || GameManager.Instance.ChannelIndex != GameManager.Instance.KillingFloorChannelIndex)
        {
            inputField.interactable = false;
        }

        

        Debug.Log($"current selected obj: {EventSystem.current.currentSelectedGameObject}");
        if ( EventSystem.current.currentSelectedGameObject == inputField.gameObject) 
        { 
            if (InputManager.InputActions.Gameplay.Input0.WasReleasedThisFrame())
            {
                inputField.text += "0";
            }
            if (InputManager.InputActions.Gameplay.Input1.WasReleasedThisFrame())
            {
                inputField.text += "1";
            }
            if (InputManager.InputActions.Gameplay.Input2.WasReleasedThisFrame())
            {
                inputField.text += "2";
            }
            if (InputManager.InputActions.Gameplay.Input3.WasReleasedThisFrame())
            {
                inputField.text += "3";
            }
            if (InputManager.InputActions.Gameplay.Input4.WasReleasedThisFrame())
            {
                inputField.text += "4";
            }
            if (InputManager.InputActions.Gameplay.Input5.WasReleasedThisFrame())
            {
                inputField.text += "5";
            }
            if (InputManager.InputActions.Gameplay.Input6.WasReleasedThisFrame())
            {
                inputField.text += "6";
            }
            if (InputManager.InputActions.Gameplay.Input7.WasReleasedThisFrame())
            {
                inputField.text += "7";
            }
            if (InputManager.InputActions.Gameplay.Input8.WasReleasedThisFrame())
            {
                inputField.text += "8";
            }
            if (InputManager.InputActions.Gameplay.Input9.WasReleasedThisFrame())
            {
                inputField.text += "9";
            }
        }
    }

    //private IEnumerator ResetInputFieldAfterCooldown()
    //{
    //    yield return new WaitForSeconds(GameManager.Instance.timePenaltyDuration);
    //    ResetInputField();
    //}

    //private void OnNewVictimSpawed()
    //{
    //    ResetInputField();
    //}

    //private void ResetInputField()
    //{
    //    wrongImage.SetActive(false);
    //    ClearInputField();
    //    inputField.interactable = true;
    //    if (isFirstDigit)
    //    {
    //        inputField.Select();
    //        ActivateInputField();
    //    }
    //    else
    //    {
    //        inputField.DeactivateInputField();
    //    }
        
    //}

    private void OnVictimDied(Victim obj)
    {
        inputField.interactable = false;
        ClearInputField();
    }

    private void OnEnable()
    {
        ClearInputField();
        if (isFirstDigit) { StartCoroutine(DelayBeforeActivating()); }
        PasscodeManager.ValidationCompleted += OnValidationCompleted;
        //GameManager.NewVictimSpawned += OnNewVictimSpawed;
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
        if (isFirstDigit)
        {
            ActivateInputField();
        }
    }

    private void OnDisable()
    {
        PasscodeManager.ValidationCompleted -= OnValidationCompleted;
        //GameManager.NewVictimSpawned -= OnNewVictimSpawed;
        GameManager.VictimDied -= OnVictimDied;
        GameManager.ChangedToKillingChannel -= OnChangedToKillingChannel;
    }
}
