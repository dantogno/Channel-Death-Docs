using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField]
    private GameObject QuestionAndAnswerMask;

    private int currentQuestionIndex = 0;
    private Button[] answerButtons;
    private bool blockInput = false;

    private void Start()
    {
        InitializeQuestionText();
        InitializeAnswerBank();
        InitializeAnswerButtons();
        answerButtons[0].onClick.AddListener(() => StartCoroutine(ProcessAnswerCoroutine()));
    }

    private void Update()
    {
        if (!blockInput)
        {
            if (InputManager.InputActions.Gameplay.Input1.WasPressedThisFrame())
            {
                answerButtons[0].Select();
                answerButtons[0].onClick.Invoke();
            }
            if (InputManager.InputActions.Gameplay.Input2.WasPressedThisFrame())
            {
                answerButtons[1].Select();
            }
            if (InputManager.InputActions.Gameplay.Input3.WasPressedThisFrame())
            {
                answerButtons[2].Select();
            }
            if (InputManager.InputActions.Gameplay.Input4.WasPressedThisFrame())
            {
                answerButtons[3].Select();
            }
            if (InputManager.InputActions.Gameplay.Input5.WasPressedThisFrame())
            {
                answerButtons[4].Select();
            }
            if (InputManager.InputActions.Gameplay.Input6.WasPressedThisFrame())
            {
                answerButtons[5].Select();
            }
            if (InputManager.InputActions.Gameplay.Input7.WasPressedThisFrame())
            {
                answerButtons[6].Select();
            }
            if (InputManager.InputActions.Gameplay.Input8.WasPressedThisFrame())
            {
                answerButtons[7].Select();
            }
        }
    }

    private IEnumerator ProcessAnswerCoroutine()
    {
        blockInput = true;
        yield return new WaitForSeconds(0.5f);
        // use leantween to scale down the height of the question and answer mask
        LeanTween.scaleY(QuestionAndAnswerMask.gameObject, 0, 0.5f).setEase(LeanTweenType.easeInBack)
            .setOnComplete(()=>
            {
                 blockInput = false;
            });
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

    /// <summary>
    /// initialize the answer button array based to map to the answer bank ui texts array
    /// </summary>
    private void InitializeAnswerButtons()
    {
        answerButtons = new Button[answerBankUiTexts.Length];
        for (int i = 0; i < answerBankUiTexts.Length; i++)
        {
            answerButtons[i] = answerBankUiTexts[i].GetComponent<Button>();
        }
    }
}
