using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Victim;

public class InterrogationSceneControler : MonoBehaviour
{
    [SerializeField]
    private float readingTimePerCharacter = 0.15f;

    [SerializeField]
    private float delayBetweenLines = 0.5f;


    [SerializeField]private TMP_Text closedCaptionText;
    [SerializeField] private GameObject closedCaptionBG;

    private const string victimPrefix = ">>VICTIM: ";
    private const string detectivePrefix = ">>DETECTIVE: ";

    private static ClueFollowUpLines clueFollowUpLines;
    // Start is called before the first frame update
    void Start()
    {
        // load clue follow up lines from resources if not already loaded
        if (clueFollowUpLines == null)
        {
            clueFollowUpLines = Resources.Load<ClueFollowUpLines>("ClueFollowUpLines");
        }

        closedCaptionText.gameObject.SetActive(false);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);

        RunTest(3);
    }

    private IEnumerator PlaySequence()
    {
        // show detective line
        closedCaptionText.text = GetDetectiveIntroLine();
        closedCaptionText.gameObject.SetActive(true);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(readingTimePerCharacter * closedCaptionText.text.Length);

        // clear
        closedCaptionText.gameObject.SetActive(false);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(delayBetweenLines);

        // show victim line 1
        closedCaptionText.text = GetClueText();
        closedCaptionText.gameObject.SetActive(true);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(readingTimePerCharacter * closedCaptionText.text.Length);

        // clear
        closedCaptionText.gameObject.SetActive(false);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(delayBetweenLines);

        // show victim line 2
        closedCaptionText.text = GetVictimFollowUpLine();
        closedCaptionText.gameObject.SetActive(true);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        //yield return new WaitForSeconds(readingTimePerCharacter * closedCaptionText.text.Length);
        // we can probably just leave the last line up? or should we clear it?
    }

    private string GetVictimFollowUpLine()
    {
        return victimPrefix + clueFollowUpLines.VictimFollowUpLines[Random.Range(0, clueFollowUpLines.VictimFollowUpLines.Length)];
    }

    private string GetClueText()
    {
        if (GameManager.Instance.SaveCount == 0)
        {
            // TODO: make sure we don't get here if we haven't saved anyone
            Debug.LogWarning("shouldn't be here if we haven't saved anyone");
            return detectivePrefix + "We have to keep looking!";
        }

        int maxIndex = GameManager.Instance.SaveCount > SaveSystem.CurrentGameData.QuestionList.Count 
            ? SaveSystem.CurrentGameData.QuestionList.Count : GameManager.Instance.SaveCount;

        // 
        for (int i = 1; i < maxIndex; i++)
        {
            var question = SaveSystem.CurrentGameData.QuestionList[i];
            if (!question.HasClueBeenGiven)
            {
                question.HasClueBeenGiven = true;
                return victimPrefix + question.ClueBank[question.CorrectAnswerIndex];
            }
        }

        // if we get here, we've given all the clues
        // give a random clue
        var randomClueIndex = Random.Range(0, maxIndex);
        var randomQuestion = SaveSystem.CurrentGameData.QuestionList[randomClueIndex];
        return victimPrefix + randomQuestion.ClueBank[randomQuestion.CorrectAnswerIndex];
    }

    private string GetDetectiveIntroLine()
    {
        // get a random line from the detective intro lines
        return detectivePrefix + clueFollowUpLines.DetectiveIntroLines[Random.Range(0, clueFollowUpLines.DetectiveIntroLines.Length)];
    }

    public void OnChannelEntered()
    {
        // if we have anyone rescued, play the sequence
        StopAllCoroutines();
        closedCaptionText.gameObject.SetActive(false);
        StartCoroutine(PlaySequence());
    }

    public void OnChannelExited()
    {
        StopAllCoroutines();
        closedCaptionText.gameObject.SetActive(false);
    }

    public void RunTest(int numRescuedVictims)
    {
        for (int i = 0; i < numRescuedVictims; i++)
        {
            // initialize a dummy victim and add it to the save data
            // create an empty gameobject and add the victim component to it
            var victim = new GameObject().AddComponent<Victim>();
            victim.isFemale = Random.Range(0, 2) == 0;
            victim.name = "Victim " + i;
            victim.State = VictimState.Rescued;
            SaveSystem.CurrentGameData.VictimHistory.Add(victim);
        }
        OnChannelEntered();
    }
}
