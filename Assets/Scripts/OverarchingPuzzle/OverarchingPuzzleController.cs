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

    [SerializeField]
    private int lockOutTimeInMinutes = 5;

    [SerializeField]
    [Tooltip("Order must match desired number labels")]
    private TMP_Text[] answerBankUiTexts;

    [SerializeField]
    private TMP_Text questionText, timerText;

    [SerializeField]
    private GameObject monitorScreen, questionAndAnswerPanel;

    [SerializeField]
    private GameObject correctAnswerLabel, wrongAnswerLabel, lockedPrompt, mainContainer;

    [SerializeField]
    private AudioClip correctAnswerAudioClip, wrongAnwerAudioClip;

    [SerializeField]
    [Tooltip("Order is relevant, numbered left to right")]
    private Image[] progressDots;

    [SerializeField]
    private GameObject initialPrompt;

    [SerializeField]
    private Color progressDotActiveColor;

    private Color selectedColor = Color.white;
    private Color normalColor;
    private Color progressDotInactiveColor = Color.white;
    private int currentQuestionIndex = 0;
    private AudioSource audioSource;
    private bool blockInput = false;
    private bool isChannelActive = false;
    private bool isLockedOut_useProperty = false;
    private bool isTurnedOn = false;
    private bool quizIsStarted = false;
    private float lockOutTimerInSeconds;
    private int minutes, seconds;
    private LTDescr progressDotTween;

    private bool IsLockedOut
    {
        get => isLockedOut_useProperty;
        set
        {
            isLockedOut_useProperty = value;
            lockedPrompt.SetActive(value);
            timerText.color = value ? Color.red : normalColor;
            if (value == false)
            {
                LeanTween.scale(initialPrompt, Vector3.one, 0.25f).setEaseInQuart();
                TurnOffMonitor();
            }
        }
    }
    private Image activeProgressDot => progressDots[currentQuestionIndex];

    private void ResetQuiz()
    {
        quizIsStarted = false;
        currentQuestionIndex = 0;
        lockOutTimerInSeconds = 10;//TODO: remove test code! lockOutTimeInMinutes * 60;
    }

    public void OnChannelExit()
    {
        isChannelActive = false;
        mainContainer.SetActive(false);
        if (quizIsStarted)
        {
            // set Y scale question and answer panel to 0
            LeanTween.scaleY(questionAndAnswerPanel, 0, 0.1f).setEaseInQuart();
            HandleWrongAnswer();
        }  
    }

    public void OnChannelEnter()
    {
        isChannelActive = true;
        mainContainer.SetActive(true);
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        normalColor = answerBankUiTexts[0].color;
        IsLockedOut = false;
        // move the lockedText up and down looping with LeanTween
        var lockedTextGameObject = lockedPrompt.GetComponentInChildren<TMP_Text>().gameObject;
        LeanTween.moveLocalY(lockedTextGameObject, lockedTextGameObject.transform.localPosition.y + 1300, 10f).setEaseInOutSine().setLoopPingPong();
        TurnOffMonitor();
    }
    private void Update()
    {
        UpdateTimers();
        // if turned off...
        if (!isTurnedOn)
        {
            if (isChannelActive && InputManager.InputActions.Gameplay.Power.WasPressedThisFrame())
            {
                isTurnedOn = true;
                // scale the y from 0 to 1 with Leantween
                LeanTween.scaleY(monitorScreen, 1, 0.25f).setEaseInQuart();
            }
        }
        // if turned on...
        else 
        {
            if (!blockInput && !IsLockedOut && isChannelActive)
            {
                // turn the monitor off if they press power.
                if (InputManager.InputActions.Gameplay.Power.WasPressedThisFrame())
                {
                    TurnOffMonitor();
                }
                else if (quizIsStarted)
                {
                    HandlePuzzleQuestionInput();
                }
                // if puzzle isn't started, check if they want to start it
                else if (InputManager.InputActions.Gameplay.Enter.WasPressedThisFrame())
                {
                    StartQuiz();
                }
            }     
        }
    }

    private void StartQuiz()
    {
        quizIsStarted = true;
        PrepareNextQuestion();
        // use leantween to scale out the initial prompt
        LeanTween.scale(initialPrompt, Vector3.zero, 0.25f).setEaseOutQuart()
            .setOnComplete(() =>
            {
                questionAndAnswerPanel.transform.localScale = Vector3.zero;
                questionAndAnswerPanel.SetActive(true);
                LeanTween.scale(questionAndAnswerPanel, Vector3.one, 0.25f).setEaseInQuart();
            });
    }

    private void TurnOffMonitor()
    {
        isTurnedOn = false;
        // scale the y from 0 to 1 with Leantween
        LeanTween.scaleY(monitorScreen, 0, 0.25f).setEaseOutQuart();
    }

    private void HandlePuzzleQuestionInput()
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

    private void TurnOffAllProgressDots()
    {
        // if quiz is not started, all progress dots should be set to the inactive color
        for (int i = 0; i < NumberOfQuestions; i++)
        {
            progressDots[i].color = progressDotInactiveColor;
        }
        if (progressDotTween != null)
            progressDotTween.reset();
    }

    private void UpdateCurrentProgressDot()
    {
        if (progressDotTween != null)
            progressDotTween.reset();

        if (quizIsStarted && !IsLockedOut && isTurnedOn)
        {
            // the progress dot that matches the current question index should use leantween to pingpong between active and inactive colors
            progressDotTween = LeanTween.value(activeProgressDot.gameObject, progressDotInactiveColor, progressDotActiveColor, 0.25f)
              .setOnUpdate((Color val) => activeProgressDot.color = val).setEaseInOutSine().setLoopPingPong();
            // the progress dots before the current question index should be set to the active color
            for (int i = 0; i < currentQuestionIndex; i++)
            {
                progressDots[i].color = progressDotActiveColor;
            }
        }
    }

    private void UpdateTimers()
    {
        if (IsLockedOut)
        {
            lockOutTimerInSeconds -= Time.deltaTime;
            minutes = Mathf.FloorToInt(lockOutTimerInSeconds / 60);
            seconds = Mathf.FloorToInt(lockOutTimerInSeconds % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
            if (lockOutTimerInSeconds <= 0)
            {
                // Property update will reset the timer and text color
                IsLockedOut = false;
            }
        }
        else 
        {
            if (isTurnedOn)
            {
                minutes = Mathf.FloorToInt(SaveSystem.CurrentGameData.TimeRemainingInSeconds / 60);
                seconds = Mathf.FloorToInt(SaveSystem.CurrentGameData.TimeRemainingInSeconds % 60);

                // update the timer text to show minutes and seconds remaining
                timerText.text = $"{minutes:00}:{seconds:00}";
            }
            else
            // If the monitor is off, set the timer text to the turn on message.
            {
                timerText.text = "Power On?";
            }
        }
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
        // Show the feedback message
        LeanTween.scale(feedbackMessage, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);

        // wait for transition to finish and for player to process feedback
        yield return new WaitForSeconds(2f);

        // Hide the feedback message
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
                LeanTween.scaleY(questionAndAnswerPanel.gameObject, 1, 0.5f)
                    .setEase(LeanTweenType.easeOutBack);
            }
            else
            {
                // todo: big finish
            }
  
        }
        else
        {
            // incorrect answer
            HandleWrongAnswer();
        }


        // wait for transition to finish
        yield return new WaitForSeconds(0.5f);

        blockInput = false;
    }

    private void HandleWrongAnswer()
    {
        // Lock out behavior handled in the IsLockedOut property
        ResetQuiz();
        IsLockedOut = true;
        TurnOffAllProgressDots();
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
