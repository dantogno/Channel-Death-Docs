using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    public float delayBeforeResetting = 1f;
    public float delayBeforeScrolling = 3f;
    public float speed = 2;
    /// <summary>
    /// I just got this by dividing the size of the text by where the position it needed to go to
    /// </summary>
    private float sizeFactor = 0.66f;
    private RectTransform rectTransform;
    private float resetXPosition;
    private float delayBeforeResettingTimer, delayBeforeScrollingTimer = 0f;
    private bool isWaitingToReset = false;
    private bool isWaitingToScroll = true;
    private TMP_Text text;
    private int resetCounter = 0;

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
    }
    private void Start()
    {
        ShowRescueInstructions(GameManager.Instance.CurrentVictim.Name);
    }
    private void Update()
    {
        //Debug.Log($"transform.positon.x: {transform.position.x}");
        //Debug.Log($"transform.localPosition.x: {transform.localPosition.x}");
        //Debug.Log($"rectTransform.position.x: {rectTransform.position.x}");
        //Debug.Log($"rectTransform.localPosition.x: {rectTransform.localPosition.x}");
        resetXPosition = rectTransform.rect.width / sizeFactor * -1;

        if (isWaitingToReset)
        {
            delayBeforeResettingTimer += Time.deltaTime;
            if (delayBeforeResettingTimer >= delayBeforeResetting)
            {
                RefreshTextNow();
            }
        }
        if (isWaitingToScroll)
        {
            delayBeforeScrollingTimer += Time.deltaTime;
            if (delayBeforeScrollingTimer >= delayBeforeScrolling)
            {
                isWaitingToScroll = false;
            }
        }
        if (!isWaitingToReset && !isWaitingToScroll)
        {
            rectTransform.anchoredPosition3D += new Vector3(-speed * Time.deltaTime, 0, 0);
            if (rectTransform.anchoredPosition3D.x <= resetXPosition)
            {
                isWaitingToReset = true;
            }
        }
    }
    private void OnVictimDied(Victim obj)
    {
        resetCounter = 0;

        if (GameManager.Instance.CurrentVictimIsKiller)
        {
            text.text = "I see you have chosen death. So be it... AAAAAAAAARRRGHHHHRRGH!";
        }
        else
        {
            text.text = $"Oh no! {GameManager.Instance.CurrentVictim.Name} has died! More blood on your hands...";
        }
        RefreshTextNow();
    }

    private void OnValidationComplete(bool isSuccessful)
    {
        resetCounter = 0;

        if (isSuccessful)
        {
            if (GameManager.Instance.CurrentVictimIsKiller)
            {
                text.text = $"You have done well. I am free. I will not forget your kindness.";
            }
            else
                text.text = $"Correct! {GameManager.Instance.CurrentVictim.Name} has been set free. But another head will be on the chopping block soon...";
        }
        else 
        {
            if (GameManager.Instance.CurrentVictimIsKiller)
            {
                text.text = "Is it blood you're after? Or perhaps you're simply a fool...";
            }
            else
                text.text = $"Wrong! Poor {GameManager.Instance.CurrentVictim.Name} will pay for your carelessness!";
        }
        RefreshTextNow();
    }

    private void ShowRescueInstructions(string name)
    {
        resetCounter = 0;
        if (GameManager.Instance.CurrentVictimIsKiller)
        {
            text.text = $"It all comes down to this. I will put my own life in your hands. Do what you will.";
        }
        else
        {
            text.text = $"Enter the passcode to save {name}. Beware, there are consequences for wrong answers.";
        }
        RefreshTextNow();
    }

    private void RefreshTextNow()
    {
        resetCounter++;
        isWaitingToReset = false;
        delayBeforeResettingTimer = 0;
        delayBeforeScrollingTimer = 0;
        isWaitingToScroll = true;
        rectTransform.anchoredPosition3D = 
            new Vector3(0, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);

        if (resetCounter > 2)
        {
            ShowRescueInstructions(GameManager.Instance.CurrentVictim.Name);
        }
    }


    private void OnEnable()
    {
        Victim.VictimNameUpdated += ShowRescueInstructions;
        GameManager.VictimDied += OnVictimDied;
        PasscodeManager.ValidationCompleted += OnValidationComplete;
        
    }
    private void OnDisable()
    {
        Victim.VictimNameUpdated -= ShowRescueInstructions;
        GameManager.VictimDied -= OnVictimDied;
        PasscodeManager.ValidationCompleted -= OnValidationComplete;
    }
}
