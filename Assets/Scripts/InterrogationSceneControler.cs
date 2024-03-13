using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static Victim;

public class InterrogationSceneControler : MonoBehaviour
{
    [SerializeField]
    private float readingTimePerCharacter = 0.15f;

    [SerializeField]
    private float delayBetweenLines = 0.5f;

    [SerializeField]
    private GameObject[] victims;

    [SerializeField]
    private GameObject censorFollowTargetEmptyGameObject;

    [SerializeField]private TMP_Text closedCaptionText;
    [SerializeField] private GameObject closedCaptionBG;

    [SerializeField]
    private List<GameObject> objectsToTurnOffOnChannelExit;

    private const string victimPrefix = ">>VICTIM: ";
    private const string detectivePrefix = ">>DETECTIVE: ";

    private static ClueFollowUpLines clueFollowUpLines;
    private bool isPlaying = false;
    private List<Renderer> renderers;
    private List<Camera> cameras;
    private List<AudioListener> audioListeners;
    private GameObject currentVictim = null;
    
    public event Action InterrogationSequenceFinished;

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

        // not sure if I should disable the renderers or just deactivate the game objects
        // renderers leaves out the camera.
        renderers = new List<Renderer>();
        // add all of the renderers from the objects to turn off on channel exit to the renderers list
        foreach (var obj in objectsToTurnOffOnChannelExit)
        {
            renderers.AddRange(obj.GetComponentsInChildren<Renderer>());
        }

        cameras = new List<Camera>();
        audioListeners = new List<AudioListener>();
        // add all of the cameras from the objects to turn off on channel exit to the cameras list
        foreach (var obj in objectsToTurnOffOnChannelExit)
        {
            cameras.AddRange(obj.GetComponentsInChildren<Camera>());
        }
        foreach (var cam in cameras)
        {
            audioListeners.AddRange(cam.GetComponentsInChildren<AudioListener>());
        }
        OnChannelExited();

        //RunTest(3);
    }

    private IEnumerator InterrogationSequenceCoroutine()
    {
        isPlaying = true;
        // Detective: confirm your name:
        closedCaptionText.text = detectivePrefix + "Can please you state your name for the recording?";
        closedCaptionText.gameObject.SetActive(true);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(readingTimePerCharacter * closedCaptionText.text.Length);

        // clear
        closedCaptionText.gameObject.SetActive(false);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(delayBetweenLines);

        // Victim: I'm [name]
        var rescuedVictims = SaveSystem.CurrentGameData.VictimHistory.Where(v => v.State == VictimState.Rescued);
        var randomVictim = rescuedVictims.ElementAt(UnityEngine.Random.Range(0, rescuedVictims.Count()));
        closedCaptionText.text = victimPrefix + "I'm " + randomVictim.Name;
        closedCaptionText.gameObject.SetActive(true);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(readingTimePerCharacter * closedCaptionText.text.Length);

        // clear
        closedCaptionText.gameObject.SetActive(false);
        closedCaptionBG.SetActive(closedCaptionText.gameObject.activeSelf);
        yield return new WaitForSeconds(delayBetweenLines);

        // show detective intro line
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
        isPlaying = false;
        currentVictim = null;
        InterrogationSequenceFinished?.Invoke();
    }

    private string GetVictimFollowUpLine()
    {
        return victimPrefix + clueFollowUpLines.VictimFollowUpLines[UnityEngine.Random.Range(0, clueFollowUpLines.VictimFollowUpLines.Length)];
    }

    private string GetClueText()
    {
        if (GameManager.Instance.RescuedCount == 0)
        {
            // TODO: make sure we don't get here if we haven't saved anyone
            Debug.LogWarning("shouldn't be here if we haven't saved anyone");
            return detectivePrefix + "We have to keep looking!";
        }

        // choose the max index to be the smaller of the save count and the total number of questions/clues
        int maxIndex = GameManager.Instance.RescuedCount > SaveSystem.CurrentGameData.QuestionList.Count 
            ? SaveSystem.CurrentGameData.QuestionList.Count : GameManager.Instance.RescuedCount;

        // I guess we'll reveal the clues in order? no sense getting a clue for a question they can't see yet, right?
        for (int i = 1; i < maxIndex; i++)
        {
            var question = SaveSystem.CurrentGameData.QuestionList[i];
            if (!question.HasClueBeenGiven)
            {
                question.HasClueBeenGiven = true;
                return victimPrefix + question.ClueBank[question.CorrectAnswerIndex];
            }
        }

        // if we get here, we've given all the clues within this index.
        // repeat a random clue (don't show clues they haven't seen yet)
        var randomClueIndex = UnityEngine.Random.Range(0, maxIndex);
        var randomQuestion = SaveSystem.CurrentGameData.QuestionList[randomClueIndex];
        return victimPrefix + randomQuestion.ClueBank[randomQuestion.CorrectAnswerIndex];
    }

    private string GetDetectiveIntroLine()
    {
        // get a random line from the detective intro lines
        return detectivePrefix + clueFollowUpLines.DetectiveIntroLines[UnityEngine.Random.Range(0, clueFollowUpLines.DetectiveIntroLines.Length)];
    }

    public void ShowInterrogationScene()
    {
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }

        foreach (var camera in cameras)
        {
            camera.enabled = true;
        }


        if (!isPlaying)
        {
            SelectVictimModel();
            closedCaptionText.gameObject.SetActive(false);
            StartCoroutine(InterrogationSequenceCoroutine());
        }
        else
        {
            // resume
            DisableRenderersInVictims();
            var victimRenderers = currentVictim.GetComponentsInChildren<Renderer>();
            foreach (var renderer in victimRenderers)
            {
                renderer.enabled = true;
            }
        }
    }

    public void OnChannelExited()
    {
        DisableRenderersInVictims();
        // turn off all the renderers in the list
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                renderer.enabled = false;
            }
        }
        foreach (var camera in cameras)
        {
            if (camera != null)
                camera.enabled = false;
        }
        foreach (var listener in audioListeners)
        {
            if (listener != null)
                listener.enabled = false;
        }
    }

    public void RunTest(int numRescuedVictims)
    {
        for (int i = 0; i < numRescuedVictims; i++)
        {
            // initialize a dummy victim and add it to the save data
            // create an empty gameobject and add the victim component to it
            var victim = new GameObject().AddComponent<Victim>();
            victim.isFemale = UnityEngine.Random.Range(0, 2) == 0;
            victim.Name = "Victim " + i;
            victim.State = VictimState.Rescued;
            SaveSystem.CurrentGameData.VictimHistory.Add(victim);
        }
        ShowInterrogationScene();
        StartCoroutine(TestChannelChange());
    }

    private IEnumerator TestChannelChange()
    {
        yield return new WaitForSeconds(3);
        OnChannelExited();
        yield return new WaitForSeconds(1f);
        ShowInterrogationScene();
    }

    /// <summary>
    /// Pick a random victim model to use
    /// </summary>
    private void SelectVictimModel()
    {
        // select a random victim model
        currentVictim = victims[UnityEngine.Random.Range(0, victims.Length)];
        
        // enable the renderers on the selected victim
        var renderers = currentVictim.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = true;
        }

        // find the child gameobject named "head" and assign it as the follow target
        var head = currentVictim.GetComponentInChildren<CensorFollowTarget>();
        if (head != null)
        {
            censorFollowTargetEmptyGameObject.transform.SetParent(head.transform, false);
            censorFollowTargetEmptyGameObject.transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.LogWarning("Can't find head censor follow target on victim model");
        }
    }
    private void DisableRenderersInVictims()
    {
      // iterate through all the victims and disable their renderers
        foreach (var victim in victims)
        {
            var renderers = victim.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.enabled = false;
            }
        }
    }
}
