using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PuzzleQuestion", menuName = "Scriptable Objects/Create PuzzleQuestion")]
public class PuzzleQuestion: ScriptableObject
{
    public enum QuestionType
    {
        Normal,
        WhoDied,
        WhoSaved,
        InputAtSpecificTime
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
    public string[] AnswerBank = new string[8];

    /// <summary>
    /// Single correct answer to be used in the puzzle, chosen from the AnswerBank
    /// </summary>
    public int CorrectAnswerIndex { get; private set; }

    public bool IsInitialized { get; private set; } = false;

    /// <summary>
    /// Clue revealed when victim is saved. Clue index should correspond with the correct answer index.
    /// E.g. ClueBank[0] should be the clue for AnswerBank[0]
    /// </summary>
    public string[] ClueBank =new string[8];

    /// <summary>
    /// Use to decide which clue to show when a victim is saved.
    /// We can loop them once they have all been shown.
    /// </summary>
    public bool HasClueBeenGiven = false;

   /// <summary>
   /// Select a correct answer from the answer bank at random
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

                break;
            case QuestionType.WhoSaved:
                break;
            case QuestionType.InputAtSpecificTime:
                break;
        }
        IsInitialized = true;
    }
}

