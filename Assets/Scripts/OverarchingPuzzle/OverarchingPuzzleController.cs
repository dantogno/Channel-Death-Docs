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
    private GameObject questionAndAnswerPanel;

    [SerializeField]
    private GameObject correctAnswerLabel, wrongAnswerLabel, lockedText;

    [SerializeField]
    private AudioClip correctAnswerAudioClip, wrongAnwerAudioClip;


    private Color selectedColor = Color.white;
    private Color normalColor;
    private int currentQuestionIndex = 0;
    private bool blockInput = false;
    private AudioSource audioSource;

    private void Start()
    {
        InitializeQuestionText();
        InitializeAnswerBank();
        audioSource = GetComponent<AudioSource>();
        normalColor = answerBankUiTexts[0].color;
        // move the lockedText up and down looping with LeanTween
        LeanTween.moveLocalY(lockedText, lockedText.transform.localPosition.y + 1300, 10f).setEaseInOutSine().setLoopPingPong();
    }

    private void ChooseAnswerAtIndex(int index)
    {
        if (!blockInput)
        {
            StopAllCoroutines();
            StartCoroutine(ProcessAnswerCoroutine(index));
            blockInput = true;
        }
    }

    private void Update()
    {
        if (!blockInput)
        {
            if (InputManager.InputActions.Gameplay.Input1.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(0);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input2.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(1);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input3.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(2);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input4.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(3);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input5.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(4);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input6.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(5);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input7.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(6);
                return;
            }
            if (InputManager.InputActions.Gameplay.Input8.WasPressedThisFrame())
            {
                ChooseAnswerAtIndex(7);
                return;
            }
        }
    }

    private IEnumerator ProcessAnswerCoroutine(int answerIndex)
    {
        answerBankUiTexts[answerIndex].color = selectedColor;
        // wait for player to process text select state change
        yield return new WaitForSeconds(0.5f);
        LeanTween.scaleY(questionAndAnswerPanel.gameObject, 0, 0.5f).setEase(LeanTweenType.easeInBack);

        // wait for transition to finish
        yield return new WaitForSeconds(0.9f);
        answerBankUiTexts[answerIndex].color = normalColor;

        var isCorrect = SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex].CorrectAnswerIndex == answerIndex;  
        var feedbackMessage = isCorrect ? correctAnswerLabel : wrongAnswerLabel;
        var audioClip = isCorrect ? correctAnswerAudioClip : wrongAnwerAudioClip;
        audioSource.PlayOneShot(audioClip);
        feedbackMessage.transform.localScale = Vector3.zero;
        feedbackMessage.SetActive(true);
        LeanTween.scale(feedbackMessage, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);

        // wait for transition to finish and for player to process feedback
        yield return new WaitForSeconds(2f);

        LeanTween.scale(feedbackMessage, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() => feedbackMessage.SetActive(false));

        // wait for transition to finish
        yield return new WaitForSeconds(0.7f);

        LeanTween.scaleY(questionAndAnswerPanel.gameObject, 1, 0.5f).setEase(LeanTweenType.easeOutBack);

        // wait for transition to finish
        yield return new WaitForSeconds(0.5f);
        blockInput = false;
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
