using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This is the news channel.
/// It will start by showing the scenario overview, then integrate the interrogation as victims are rescued.
/// 
/// What logic should determine when to show the scenario overview and when to show the interrogation?
/// Maybe if a new victim is just rescued, the interrogation is shown for sure the next time they enter the channel.
/// 
/// Otherwise, we show the scenario overview for 30 second, then switch to the interrogation, repeat?
/// Would be cool to have other news stuff.
/// 
/// This channel will not stop when the player leaves the channel (though it should disable it's audio and renderers).
/// </summary>
public class NewsChannelController : MonoBehaviour
{
    [SerializeField]
    private float scenarioOverviewDuration = 30;

    [SerializeField]
    private GameObject scenarioOverview;

    [SerializeField]
    private InterrogationSceneControler interrogation;

    private bool shouldShowInterrogation = false;
    private int rescueQueue = 0;
    private bool isScenarioOverviewPlaying = false;

    public string mostRecentVictim = null;
    

    /// <summary>
    /// Hook this up to the channel Unity event in the editor.
    /// </summary>
    public void OnChannelEntered()
    {
        if (shouldShowInterrogation || rescueQueue > 0)
        {
            interrogation.ShowInterrogationScene(mostRecentVictim);
            mostRecentVictim = null;
        }
        else
        {
            scenarioOverview.SetActive(true);
            if (!isScenarioOverviewPlaying)
            {
                StartCoroutine(StartScenarioOverviewSequence());
            }
        }
    }

    /// <summary>
    /// Hook this up to the channel Unity event in the editor.
    /// </summary>
    public void OnChannelExited()
    {
        interrogation.OnChannelExited();
        scenarioOverview.SetActive(false);
    }

    private IEnumerator StartScenarioOverviewSequence()
    {
        isScenarioOverviewPlaying = true;
        shouldShowInterrogation = false;
        yield return new WaitForSeconds(scenarioOverviewDuration);
        shouldShowInterrogation = GameManager.Instance.RescuedCount > 0;
        isScenarioOverviewPlaying = false;
    }

    private void OnVictimRescued(string victimName)
    {
        if (victimName == GameManager.Instance.KillerName) { return; }
        rescueQueue++;
        mostRecentVictim = victimName;
        StartCoroutine(SwitchToInterrogationAfterDelay());
    }

    private IEnumerator SwitchToInterrogationAfterDelay()
    {
        yield return new WaitForSeconds(10);
        interrogation.CancelSequence();
        GameManager.Instance.GotoChannel(GameManager.Instance.NewsChannelIndex);
    }

    private void OnInterrogationSequenceFnished()
    {
        rescueQueue--;
        if (rescueQueue < 0) 
        { 
            rescueQueue = 0;
        }
        shouldShowInterrogation = false;
    }

    private void OnEnable()
    {
        GameManager.VictimRescued += OnVictimRescued;
        interrogation.InterrogationSequenceFinished += OnInterrogationSequenceFnished;
    }

    private void OnDisable()
    {
        GameManager.VictimRescued -= OnVictimRescued;
        interrogation.InterrogationSequenceFinished -= OnInterrogationSequenceFnished;
    }
}
