using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : Singleton<GameManager>
{
    [SerializeField]
    private TMP_Text channelNumberText;

    [Tooltip("These should be in the scene, we don't instantiate them in code.")]
    [SerializeField]
    private GameObject[] channelPrefabs;

    [Tooltip("Victim dies after this many seconds.")]
    public float killTimeInSeconds = 180;

    public float timePenaltyMultiplier = 5;
    public float timePenaltyDuration = 2f;

    [Tooltip("How long to wait before spawning a new victim after the previous one dies or is rescued.")]
    public float DelayInSecondsBetweenVictims = 5;

    [SerializeField]
    private GameObject conveyorBelt;

    [SerializeField]
    private GameObject breakingNewsUI;

    public UVOffsetYAnim beltUVAnimation;
    public GameObject victimSpawnPoint;
    public GameObject killPosition;
    public GameObject[] victimPrefabs;
    public Vector3 victimLocalRotation = new Vector3(0, 279.761871f, 0);
    public static event Action<string> VictimDied;
    public static event Action NewVictimSpawned;

   
    public int KillingFloorChannelIndex { get; private set; }
    public float TimeUntilNextKill { get; private set; } = 0;
    public int ChannelIndex { get; private set; } = 0;  

    public bool BlockChangingChannels { get; private set; } = false;
    public Victim CurrentVictim { get; private set; }

    private float conveyorBeltDefaultSpeed;
    private bool currentVictimIsDead;
    private Channel[] channels;
    private Vector3 originalSpawnPosition;
    private int victimIndex = 0;
    private Vector3 outOfViewPosition;
    private Coroutine channelTextCoroutine;
    private float timePenaltyTimer = 0;
    private bool victimIsBeingRescued = false;
    private bool waitingToChangeToKillingFloor = false;
   

    private void Start()
    {
        ChannelIndex = 0;
        KillingFloorChannelIndex = 0;
        InitializeChannelArray();
        channels[ChannelIndex].ChannelEntered?.Invoke();
        channelNumberText.gameObject.SetActive(false);
        UpdateChannelText();
        InitializeVictimsOutOfViewPosition();
        ResetKillTimer();
        SetNewVictimName();
        MoveVictimToPosition();
        conveyorBeltDefaultSpeed = Vector3.Distance(victimSpawnPoint.transform.position, killPosition.transform.position) / killTimeInSeconds;
    }

    private void InitializeVictimsOutOfViewPosition()
    {
        outOfViewPosition = killPosition.transform.position + Vector3.down * 100;
        foreach (var victim in victimPrefabs)
        {
            victim.transform.position = outOfViewPosition;
        }
        originalSpawnPosition = victimSpawnPoint.transform.position;
    }

    private void Update()
    {   
        float timeMultiplier = GetMultiplierBasedOnPenaltyStatus();
        UpdateKillTimer(timeMultiplier);
        UpdateBeltSpeed(timeMultiplier);
        UpdateVictimPosition(timeMultiplier);

        if (!currentVictimIsDead && TimeUntilNextKill <= 0)
        {
            BlockChangingChannels = true;
            if (ChannelIndex != KillingFloorChannelIndex && !waitingToChangeToKillingFloor)
            {
                waitingToChangeToKillingFloor = true;
                breakingNewsUI.SetActive(true);
                StartCoroutine(SwitchToKillingFloorAfterDelay());
            }
            else if (ChannelIndex == KillingFloorChannelIndex)
            {
                KillVictim();
            }            
        }
    }

    private IEnumerator SwitchToKillingFloorAfterDelay()
    {
        yield return new WaitForSeconds(2);
        ChangeChannel(KillingFloorChannelIndex);
        waitingToChangeToKillingFloor = false;
        // delay before kill to let viewer get their bearings.
        yield return new WaitForSeconds(1.5f);
        KillVictim();

    }

    private IEnumerator SpawnNewVictimAfterDelay()
    {
        yield return new WaitForSeconds(DelayInSecondsBetweenVictims);
        SetUpNextVictim();
        BlockChangingChannels = false;
    }

    private float GetMultiplierBasedOnPenaltyStatus()
    {
        float timeMultiplier = 1;
        if (timePenaltyTimer >= 0)
        {
            timePenaltyTimer -= Time.deltaTime;
            timeMultiplier = timePenaltyTimer > 0 ? timePenaltyMultiplier : 1;
        }
        else 
            timePenaltyTimer = 0;

        //Debug.Log($"timePenaltyTimer: {timePenaltyTimer}");

        return timeMultiplier;
    }

    private void UpdateBeltSpeed(float multiplier)
    {
        var beltSpeed = victimIsBeingRescued ? 0 : conveyorBeltDefaultSpeed * multiplier;
        // the conveyorBeltSpeed should move victim position to killPosition in killTimeInSeconds
        beltUVAnimation.speed = beltSpeed / 2;
    }
    private void UpdateVictimPosition(float multiplier)
    {
        // move the victim along the conveyor belt at a constant speed
        victimSpawnPoint.transform.position += victimSpawnPoint.transform.forward * conveyorBeltDefaultSpeed * Time.deltaTime * multiplier;
    }

    public void IncurPenalty()
    {
        timePenaltyTimer += timePenaltyDuration;
    }

    private void UpdateKillTimer(float multiplier)
    {
        TimeUntilNextKill -= Time.deltaTime * multiplier;
    }

    public void SetUpNextVictim()
    {
        // Move old victim out of view
        victimPrefabs[victimIndex].transform.SetParent(null);
        victimPrefabs[victimIndex].transform.position = outOfViewPosition;
        UpdateVictimIndex();
        SetNewVictimName();
        MoveVictimToPosition();
        ResetKillTimer();
        currentVictimIsDead = false;
        victimIsBeingRescued = false;
        NewVictimSpawned?.Invoke();
    }
    
    private void SetNewVictimName()
    {
        var victim = victimPrefabs[victimIndex].GetComponent<Victim>();
        CurrentVictim = victim;
        victim.SetRandomName();
    }

    private void ResetKillTimer()
    {
        TimeUntilNextKill = killTimeInSeconds;
        timePenaltyTimer = 0;
    }

    /// <summary>
    /// Update victim index so we get a different victim next time
    /// </summary>
    private void UpdateVictimIndex()
    {
        victimIndex = victimIndex == victimPrefabs.Length - 1 ? 0 : victimIndex + 1;
    }

    /// <summary>
    /// move new victim to spawn position and set its parent as the spawn point 
    /// (the spawn point is what moves along belt)
    /// </summary>
    private void MoveVictimToPosition()
    {
        victimSpawnPoint.transform.position = originalSpawnPosition;
        victimPrefabs[victimIndex].transform.SetParent(victimSpawnPoint.transform, false);
        victimPrefabs[victimIndex].transform.localPosition = Vector3.zero;
        victimPrefabs[victimIndex].transform.rotation = Quaternion.Euler(victimLocalRotation);
    }

    private void KillVictim()
    {
        currentVictimIsDead = true;
        var animController = victimSpawnPoint.GetComponentInChildren<Animator>();
        animController.SetTrigger("Die");
        VictimDied?.Invoke(CurrentVictim.Name);
        StartCoroutine(SpawnNewVictimAfterDelay());
    }

    private void InitializeChannelArray()
    {
        channels = new Channel[channelPrefabs.Length];
        for (int i = 0; i < channelPrefabs.Length; i++)
        {
            channels[i] = channelPrefabs[i].GetComponent<Channel>();
        }
        // Not sure this is a good idea. Need to preserve index 0, which is channel 13.
        // almost seemed to not work anyway.
        // channels.OrderBy(c => c.ChannelNumber);
    }

    public void ChannelUp()
    {
        if (ChannelIndex == channels.Length - 1)
        {
            ChangeChannel(0);
        }
        else
        {
            ChangeChannel(ChannelIndex + 1);
        }
    }

    public void ChannelDown()
    { 
        if (ChannelIndex == 0)
        {
            ChangeChannel(channels.Length - 1);
        }
        else
        {
            ChangeChannel(ChannelIndex - 1);
        }
    }

    private void ChangeChannel(int newIndex)
    {
        if (!BlockChangingChannels)
        {
            channels[ChannelIndex].ChannelExited?.Invoke();
            ChannelIndex = newIndex;
            channels[newIndex].ChannelEntered?.Invoke();
            UpdateChannelText();        
        }
    }

    public void GoToRandomChannel()
    {
        var randomIndex = UnityEngine.Random.Range(0, channels.Length);
        ChangeChannel(randomIndex);
    }

    private void UpdateChannelText()
    {
        if (channelTextCoroutine != null)
        {
            StopCoroutine(channelTextCoroutine);
        }
        channelTextCoroutine = StartCoroutine(ShowTextForFixedDuration());
    }

    private IEnumerator ShowTextForFixedDuration()
    {
        var duration = 3.0f;
        channelNumberText.text = channels[ChannelIndex].ChannelNumber.ToString();
        channelNumberText.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        channelNumberText.gameObject.SetActive(false);
    }

    private void OnChannelUpPressed(CallbackContext context)
    {
        ChannelUp();
    }

    private void OnChannelDownPressed(CallbackContext context)
    {
        ChannelDown();
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.ChannelUp.performed += OnChannelUpPressed;
        InputManager.InputActions.Gameplay.ChannelDown.performed += OnChannelDownPressed;
        PasscodeManager.ValidationCompleted += OnPasscodeValidationCompleted;
    }

    private void OnPasscodeValidationCompleted(bool isSuccessful)
    {
        if (isSuccessful)
        {
            victimIsBeingRescued = true;
            victimSpawnPoint.transform.position = outOfViewPosition;
        }        
    }
}
