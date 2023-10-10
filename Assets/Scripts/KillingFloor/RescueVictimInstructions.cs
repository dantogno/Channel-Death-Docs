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

    private void OnEnable()
    {
        Victim.VictimNameUpdated += ShowRescueInstructions;
        GameManager.VictimDied += ShowDeathMessage;
    }
    private void OnDisable()
    {
        Victim.VictimNameUpdated -= ShowRescueInstructions;
        GameManager.VictimDied -= ShowDeathMessage;
    }
}
