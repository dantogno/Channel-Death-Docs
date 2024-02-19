using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PuzzleQuestion
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
    /// Seven wrong answers to be used in the puzzle
    /// </summary>
    public string[] WrongAnswerBank;

    /// <summary>
    /// Single correct answer to be used in the puzzle
    /// </summary>
    public string CorrectAnswer;

    /// <summary>
    /// Clue revealed when victim is saved.
    /// </summary>
    public string ClueText;

    /// <summary>
    /// Use to decide which clue to show when a victim is saved.
    /// We can loop them once they have all been shown.
    /// </summary>
    public bool HasClueBeenGiven = false;
}

