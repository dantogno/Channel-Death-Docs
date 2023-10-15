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
    private string initialOutcomeTextMessage;

    public int ClubsNumber => int.Parse(Passcode[0].ToString());
    public int DiamondsNumber => int.Parse(Passcode[1].ToString());
    public int HeartsNumber => int.Parse(Passcode[2].ToString());
    public int SpadesNumber => int.Parse(Passcode[3].ToString());

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
        initialOutcomeTextMessage = outcomeText.text;
    }

    public void CreateNewPasscode()
    {
        //Passcode = "1234";

        // create a random 4 digit passcode
        Passcode = string.Empty;
        for (int i = 0; i < 4; i++)
        {
            Passcode += UnityEngine.Random.Range(0, 10);
        }
        Debug.Log($"Passcode is {Passcode}");
    }

    public void Validate()
    {
        if (validationInProgress) { return; }
        validationInProgress = true;
        Debug.Log("Validating");
        if (enteredPasscode == Passcode)
        {
            outcomeText.text = $"But another head will be on the chopping block soon...";
            StartCoroutine(SetUpNextVictimAfterDelay());           
        }
        else
        {
            outcomeText.text = $"Wrong! Poor {GameManager.Instance.CurrentVictim.Name} will pay for your carelessness!";
            GameManager.Instance.IncurPenalty();
        }
        ValidationCompleted?.Invoke(enteredPasscode == Passcode);
        ClearEnteredCode();
    }

    

    private IEnumerator SetUpNextVictimAfterDelay()
    {
        yield return new WaitForSeconds(GameManager.Instance.DelayInSecondsBetweenVictims);
        GameManager.Instance.SetUpNextVictim();
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
    private void OnVictimDied(string obj)
    {
        outcomeText.text = "More blood on your hands...";
    }

    private void OnNewVictimSpawned()
    {
        CreateNewPasscode();
        ResetOutcomeMessage();
    }

    private void ResetOutcomeMessage()
    {
        outcomeText.text = initialOutcomeTextMessage;
    }

    private void OnEnable()
    {
        GameManager.NewVictimSpawned += OnNewVictimSpawned;
        GameManager.VictimDied += OnVictimDied;
    }


    private void OnDisable()
    {
        GameManager.NewVictimSpawned -= OnNewVictimSpawned;
        GameManager.VictimDied -= OnVictimDied;
    }
}
