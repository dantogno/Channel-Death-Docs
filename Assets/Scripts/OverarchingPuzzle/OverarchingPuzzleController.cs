using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OverarchingPuzzleController : MonoBehaviour
{
    public const int NumberOfQuestions = 10;
    
    [Tooltip("Lines that come after the rescued victim describes the clue. Need to make sense with any clue.")]
    public string[] clueFollowUpLines;

    [SerializeField]
    [Tooltip("Order must match desired number labels")]
    private TMP_Text[] answerBankUiTexts;

    [SerializeField]
    private TMP_Text questionText;

    private int currentQuestionIndex = 0;

    private void Start()
    {
        InitializeQuestionText();
        InitializeAnswerBank();
    }

    private void InitializeQuestionText()
    {
        questionText.text = SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex].QuestionText;
    }


    private void InitializeAnswerBank()
    {
        for (int i = 0; i < answerBankUiTexts.Length; i++)
        {
            answerBankUiTexts[i].text = $"{i+1}. {SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex].AnswerBank[i]}";
        }
    }


}
