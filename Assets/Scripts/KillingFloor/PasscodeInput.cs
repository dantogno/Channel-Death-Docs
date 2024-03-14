using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class PasscodeInput : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Must be in order from left to right")]
    private PasscodeDigitEntry[] DigitEntryFields;

    private bool blockInput = false;
    private int passcodeEntryIndex = 0;

    [SerializeField]
    ParticleSystem confetti;

    [SerializeField]
    ParticleSystem smoke;

    
    public void ResetInputFields()
    {
        for (int i = 0; i < DigitEntryFields.Length; i++)
        {
            DigitEntryFields[i].ClearInputField();
            DigitEntryFields[i].wrongImage.SetActive(false);
            DigitEntryFields[i].SetSelectedInputField(false);

        }
        passcodeEntryIndex = 0;

        if (!GameManager.Instance.BlockPasscodeInput)
            DigitEntryFields[0].SetSelectedInputField(true);
    }

    private void OnNewVictimSpawned()
    {
        ResetInputFields();
    }

    private void OnEnable()
    {
        ResetInputFields();
        GameManager.NewVictimSpawned += OnNewVictimSpawned;
        PasscodeManager.ValidationCompleted += OnValidationCompleted;
        GameManager.bonusSpeed = 2.5f;
    }

    private void OnValidationCompleted(bool isCorrectCode)
    {
        if (!isCorrectCode)
        {
            blockInput = true;
            StartCoroutine(ResetInputFieldsAfterPenaltyCooldown());
        }
        if (isCorrectCode)
        {
            confetti.Play();
            Vector3 smokepos = GameManager.Instance.LastVictimRescuePos;
            smoke.transform.position = smokepos;
            smoke.Play();
            StartCoroutine(waitForConfetti());
        }
    }

    IEnumerator waitForConfetti()
    {
        yield return new WaitForSeconds(4);
        confetti.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
        GameManager.NewVictimSpawned -= OnNewVictimSpawned;
        PasscodeManager.ValidationCompleted += OnValidationCompleted;
        GameManager.bonusSpeed = 1f;
    }

    private void AddInputToDigitEntryField(PasscodeDigitEntry digitEntryField, string input)
    {
        digitEntryField.SetInputFieldText(input);
        PasscodeManager.Instance.EnteredPasscode += input;
        DigitEntryFields[passcodeEntryIndex].SetSelectedInputField(false);

        if (passcodeEntryIndex == DigitEntryFields.Length - 1)
        {
            passcodeEntryIndex = 0;
        }
        else
        {
            passcodeEntryIndex++;
            DigitEntryFields[passcodeEntryIndex].SetSelectedInputField(true);
        }
    }

    private IEnumerator ResetInputFieldsAfterPenaltyCooldown()
    {
        yield return new WaitForSeconds(GameManager.Instance.timePenaltyDuration);
        ResetInputFields();
        blockInput = false;
    }

    private void Update()
    {
        if (!GameManager.Instance.BlockPasscodeInput && !blockInput)
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
}
