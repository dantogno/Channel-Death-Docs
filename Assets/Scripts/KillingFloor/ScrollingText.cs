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

    // Start is called before the first frame update
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponent<TMP_Text>();
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
    private void OnVictimDied(string obj)
    {
        text.text = $"Oh no! {GameManager.Instance.CurrentVictim.Name} has died! More blood on your hands...";
        RefreshTextNow();
    }

    private void OnValidationComplete(bool isSuccessful)
    {
        if (isSuccessful)
        {
            text.text = $"Correct! {GameManager.Instance.CurrentVictim.Name} has been set free. But another head will be on the chopping block soon...";
        }
        else 
        {
            text.text = $"Wrong! Poor {GameManager.Instance.CurrentVictim.Name} will pay for your carelessness!";
        }
        RefreshTextNow();
    }

    private void ShowRescueInstructions(string name)
    {
        text.text = $"Enter the passcode to save {name}. Beware, there are consequences for wrong answers.";
        RefreshTextNow();
    }

    private void RefreshTextNow()
    {
        isWaitingToReset = false;
        delayBeforeResettingTimer = 0;
        delayBeforeScrollingTimer = 0;
        isWaitingToScroll = true;
        rectTransform.anchoredPosition3D = 
            new Vector3(0, rectTransform.anchoredPosition3D.y, rectTransform.anchoredPosition3D.z);
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
