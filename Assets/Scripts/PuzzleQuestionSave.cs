using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleQuestionSave
{
    private const int AnswerBankSize = 8;

    /// <summary>
    /// Used to determine if special logic is needed for the question
    /// </summary>
    public PuzzleQuestion.QuestionType Type;

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
    public int CorrectAnswerIndex { get; set; }

    public bool IsInitialized { get; set; } = false;

    /// <summary>
    /// Clue revealed when victim is saved. Clue given should correspond with the correct answer index.
    /// E.g. ClueBank[0] should be the clue for AnswerBank[0]
    /// </summary>
    public string[] ClueBank = new string[AnswerBankSize];

    public bool HasClueBeenGiven = false;
}
