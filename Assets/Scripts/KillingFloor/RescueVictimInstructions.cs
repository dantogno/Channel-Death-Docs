using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RescueVictimInstructions : MonoBehaviour
{
    public TMP_Text instructionsText;

    private void ShowRescueInstructions(string name)
    {
        instructionsText.text = $"Enter the passcode to save {name}.";
    }

    private void ShowDeathMessage(string name)
    {
        instructionsText.text = $"Oh no! {name} has died!";
    }

    private void ShowRescueMessage(bool wasRescued)
    {
        if (wasRescued)
            instructionsText.text = $"Correct! {GameManager.Instance.CurrentVictim.Name} has been set free.";
    }

    private void OnEnable()
    {
        Victim.VictimNameUpdated += ShowRescueInstructions;
        GameManager.VictimDied += ShowDeathMessage;
        PasscodeManager.ValidationCompleted += ShowRescueMessage;
    }
    private void OnDisable()
    {
        Victim.VictimNameUpdated -= ShowRescueInstructions;
        GameManager.VictimDied -= ShowDeathMessage;
        PasscodeManager.ValidationCompleted -= ShowRescueMessage;
    }
}
