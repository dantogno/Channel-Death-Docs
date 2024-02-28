using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OverarchingPuzzleController : MonoBehaviour
{
    // TODO: stop conveyor belt during sequence, block out channel changing
    // Set up "power button" initial puzzle
    // Warn player that if they start the quiz and fail they will not be able to try again for some time
    // implement lock out timer
    // TODO: reveal clues. Remember, some question types might need special rules for clues.

    public const int NumberOfQuestions = 10;
    
    [Tooltip("Lines that come after the rescued victim describes the clue. Need to make sense with any clue.")]
    public string[] clueFollowUpLines;

    [SerializeField]
    [Tooltip("Order must match desired number labels")]
    private TMP_Text[] answerBankUiTexts;

    [SerializeField]
    private TMP_Text questionText, timerText;

    [SerializeField]
    private GameObject questionAndAnswerPanel;

    [SerializeField]
    private GameObject correctAnswerLabel, wrongAnswerLabel, lockedText;

    [SerializeField]
    private AudioClip correctAnswerAudioClip, wrongAnwerAudioClip;

    [SerializeField]
    [Tooltip("Order is relevant, numbered left to right")]
    private Image[] progressDots;

    [SerializeField]
    private Color progressDotActiveColor;

    private Color selectedColor = Color.white;
    private Color normalColor;
    private Color progressDotInactiveColor = Color.white;
    private int currentQuestionIndex = 0;
    private bool blockInput = false;
    private AudioSource audioSource;
    private bool isLockedOut = false;
    private int minutes, seconds;

    private Image activeProgressDot => progressDots[currentQuestionIndex];

    private void Start()
    {
        // TODO: where should this be called? Start isn't good enough. Either event or when we "turn on the TV"
        PrepareNextQuestion();
        audioSource = GetComponent<AudioSource>();
        normalColor = answerBankUiTexts[0].color;
        // move the lockedText up and down looping with LeanTween
        LeanTween.moveLocalY(lockedText, lockedText.transform.localPosition.y + 1300, 10f).setEaseInOutSine().setLoopPingPong();
    }

    private void PrepareNextQuestion()
    {
        if (currentQuestionIndex < NumberOfQuestions)
        {
            var question = SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex];
            question.InitializeQuestion();
            UpdateQuestionText();
            UpdateAnswerBankText();
            UpdateCurrentProgressDot();
        }
    }

    private void UpdateCurrentProgressDot()
    {
        // the progress dot that matches the current question index should use leantween to pingpong between active and inactive colors
        LeanTween.value(activeProgressDot.gameObject, progressDotInactiveColor, progressDotActiveColor, 0.25f)
            .setOnUpdate((Color val) => activeProgressDot.color = val).setEaseInOutSine().setLoopPingPong();

        // the progress dots before the current question index should be set to the active color
        for (int i = 0; i < currentQuestionIndex; i++)
        {
            progressDots[i].color = progressDotActiveColor;
        }
    }

    private void UpdateTimerText()
    {
        minutes = Mathf.FloorToInt(SaveSystem.CurrentGameData.TimeRemainingInSeconds / 60);
        seconds = Mathf.FloorToInt(SaveSystem.CurrentGameData.TimeRemainingInSeconds % 60);
        // update the timer text to show minutes and seconds remaining
        timerText.text = $"{minutes:00}:{seconds:00}";
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
        UpdateTimerText();
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

        var isCorrect = DetermineIfAnswerIsCorrect(answerIndex);
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
        if (isCorrect)
        {
            if (currentQuestionIndex < NumberOfQuestions - 1)
            {
                currentQuestionIndex++;
                PrepareNextQuestion();
            }
            else
            {
                // todo: big finish
            }
      
        }
        LeanTween.scaleY(questionAndAnswerPanel.gameObject, 1, 0.5f).setEase(LeanTweenType.easeOutBack);

        // wait for transition to finish
        yield return new WaitForSeconds(0.5f);

        blockInput = false;
    }

    private bool DetermineIfAnswerIsCorrect(int answerIndex)
    {
        var question = SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex];
        bool isCorrect = false;
        switch (question.Type)
        {
            case PuzzleQuestion.QuestionType.InputAtSpecificTime:
                // if time remaining minutes is even, correct answer is at index 0, else correct answer is at index 1
                isCorrect = minutes % 2 == 0 ? question.CorrectAnswerIndex == 0 : question.CorrectAnswerIndex == 1;
                break;
            default:
                isCorrect = question.CorrectAnswerIndex == answerIndex;
                break;
        }
        return isCorrect;
    }

    private void UpdateQuestionText()
    {
        questionText.text = SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex].QuestionText;
    }

    private void UpdateAnswerBankText()
    {
        for (int i = 0; i < answerBankUiTexts.Length; i++)
        {
            // inserting some TMP formatting to try and make the space character take up less room.
            answerBankUiTexts[i].text = $"{i+1}. <cspace=-1em>   </cspace>{SaveSystem.CurrentGameData.QuestionList[currentQuestionIndex].AnswerBank[i]}";
        }
    }
}
