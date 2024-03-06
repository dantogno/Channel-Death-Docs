using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Victim;

[CreateAssetMenu(fileName = "PuzzleQuestion", menuName = "Scriptable Objects/Create PuzzleQuestion")]
public class PuzzleQuestion: ScriptableObject
{
    private const int AnswerBankSize = 8;
    public enum QuestionType
    {
        Normal,
        WhoDied,
        WhoSaved,
        InputAtSpecificTime,
        HowManyDied,
        HowManySaved
    }

    /// <summary>
    /// Used to determine if special logic is needed for the question
    /// </summary>
    public QuestionType Type;

    /// <summary>
    /// Question to be asked in the puzzle
    /// </summary>
    public string QuestionText;

    /// <summary>
    /// Eight (or more) possible answers to be used in the puzzle. Correct answer will be chosen from this list.
    /// </summary>
    public string[] AnswerBank = new string[AnswerBankSize];

    /// <summary>
    /// Single correct answer to be used in the puzzle, chosen from the AnswerBank
    /// </summary>
    public int CorrectAnswerIndex { get; private set; }

    public bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Clue revealed when victim is saved. Clue given should correspond with the correct answer index.
    /// E.g. ClueBank[0] should be the clue for AnswerBank[0]
    /// </summary>
    public string[] ClueBank = new string[AnswerBankSize];

   /// <summary>
   /// Set the correct answer index depending on the question type.
   /// Called once at the start of the game when questions are first chosen.
   /// Called again just before the question is asked to the player to account for
   /// questions with answers that are based on dynamic data.
   /// </summary>
    public void InitializeQuestion()
    {
        switch (Type)
        {
            case QuestionType.Normal:
                if (!IsInitialized)
                    CorrectAnswerIndex = UnityEngine.Random.Range(0, AnswerBank.Length);
                break;
            case QuestionType.WhoDied:
                InitializeWhoDiedQuestion();
                break;
            case QuestionType.WhoSaved:
                InitializeWhoSavedQuestion();
                break;
            case QuestionType.InputAtSpecificTime:
                InitializeInputAtSpecificTimeQuestion();
                break;
            case QuestionType.HowManyDied:
                InitializeHowManyDiedQuestion();
                break;
            case QuestionType.HowManySaved:
                InitializeHowManySavedQuestion();
                break;
        }

        // Indent all of the answers
        // TODO: I can't figure out the right formatting to make long text look good
        //for (int i = 0; i < AnswerBank.Length; i++)
        //{
        //    var originalText = AnswerBank[i];
        //    AnswerBank[i] = $"<line-indent=15%>{originalText}</line-indent>";
        //}

        IsInitialized = true;
        Debug.Log($"Question initialized: {QuestionText} with answer index {CorrectAnswerIndex}: {AnswerBank[CorrectAnswerIndex]}");
        Debug.Log($"Clue: {ClueBank[CorrectAnswerIndex]}");
    }

    private void InitializeHowManySavedQuestion()
    {
        // AnswerBank[0] will be reserved for 0
        AnswerBank[0] = "0";
        // other answers will be randomized, with some lower and some higher
        for (int i = 1; i < AnswerBank.Length; i++)
        {
            if (i > 3)
                AnswerBank[i] = UnityEngine.Random.Range(1, 10).ToString();
            else
                AnswerBank[i] = UnityEngine.Random.Range(11, 50).ToString();
        }

        if (GameManager.Instance.SaveCount > 0)
        {
            CorrectAnswerIndex = UnityEngine.Random.Range(1, AnswerBank.Length);
            AnswerBank[CorrectAnswerIndex] = GameManager.Instance.SaveCount.ToString();
        }
        else
        {
            CorrectAnswerIndex = 0;
        }
    }

    private void InitializeHowManyDiedQuestion()
    {
        // AnswerBank[0] will be reserved for 0
        AnswerBank[0] = "0";
        // other answers will be randomized, with some lower and some higher
        for (int i = 1; i < AnswerBank.Length; i++)
        {
            if (i > 3)
                AnswerBank[i] = UnityEngine.Random.Range(1, 20).ToString();
            else
                AnswerBank[i] = UnityEngine.Random.Range(21, 80).ToString();
        }

        if (GameManager.Instance.KillCount > 0) 
        { 
            CorrectAnswerIndex = UnityEngine.Random.Range(1, AnswerBank.Length);
            AnswerBank[CorrectAnswerIndex] = GameManager.Instance.KillCount.ToString();
        }
        else
        {
            CorrectAnswerIndex = 0;
        }
    }

    /// <summary>
    /// Randomly choose 0 or 1 to decide it answer must be input on even or odd minute.
    /// We will actually ignore the correct answer index for purposes of knowing 
    /// if the player input is correct. Instead we only check if they input on even or odd minute.
    /// We still use this index to determine the clue to give
    /// </summary>
    private void InitializeInputAtSpecificTimeQuestion()
    {
        if (!IsInitialized)
        {
            CorrectAnswerIndex = UnityEngine.Random.Range(0, 2);
        }
    }

    private void InitializeWhoSavedQuestion()
    {
        FillAnswerBankWithAvailableVictimNames();
        // we will reserve index 0 for "no one has been rescued"
        AnswerBank[0] = "No one has been rescued";
        // if there are rescued victims, set a random saved victim's name as the correct answer
        var rescuedVictims = SaveSystem.CurrentGameData.VictimHistory.Where((v) => v.State == VictimState.Rescued).ToList();
        if (rescuedVictims.Count > 0)
        {
            CorrectAnswerIndex = UnityEngine.Random.Range(1, AnswerBank.Length);
            AnswerBank[CorrectAnswerIndex] = rescuedVictims[UnityEngine.Random.Range(0, rescuedVictims.Count)].Name;
        }
        else
        {
            CorrectAnswerIndex = 0;
        }
    }

    private void FillAnswerBankWithAvailableVictimNames()
    {
        // fill the answer bank items index 1 - 7 with names from the available names lists
        // Reserving index 0 for "no one" answer
        List<string> wrongAnswerNames = new List<string>();
        wrongAnswerNames.AddRange(Victim.AvailableFemaleNames);
        wrongAnswerNames.AddRange(Victim.AvailableMaleNames);

        for (int i = 1; i < AnswerBank.Length; i++)
        {
            AnswerBank[i] = wrongAnswerNames[UnityEngine.Random.Range(0, wrongAnswerNames.Count)];
            wrongAnswerNames.Remove(wrongAnswerNames[i]);
        }  
    }

    private void InitializeWhoDiedQuestion()
    {
        FillAnswerBankWithAvailableVictimNames();
        // we will reserve index 0 for "no one has died"
        AnswerBank[0] = "No one has died";
        // if there are dead victims, set a random dead victim's name as the correct answer
        var deadVictims = SaveSystem.CurrentGameData.VictimHistory.Where((v) => v.State == VictimState.Dead).ToList();
        if (deadVictims.Count > 0)
        {
            CorrectAnswerIndex = UnityEngine.Random.Range(1, AnswerBank.Length);
            AnswerBank[CorrectAnswerIndex] = deadVictims[UnityEngine.Random.Range(0, deadVictims.Count)].Name;
        }
        // if there are no dead victims, index 0 is the correct answer
        else
        {
            CorrectAnswerIndex = 0;
        }
    }
}

