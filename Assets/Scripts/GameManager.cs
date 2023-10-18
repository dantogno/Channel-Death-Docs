using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("Victim dies after this many seconds.")]
    public float killTimeInSeconds = 180;

    public string KillerName = "The Broadcast Killer";

    public int numberOVictimsToSpawnBoss = 3;
    public float timePenaltyMultiplier = 5;
    public float timePenaltyDuration = 2f;
    public float distanceToClearBelt = 3f;
    public float deathBeltSpeedMultiplier;
    public bool waitForIntroBeforeBeginningGameplay = true;

    [SerializeField]
    private Victim killer;

    [SerializeField]
    private TMP_Text channelNumberText;

    [Tooltip("These should be in the scene, we don't instantiate them in code.")]
    [SerializeField]
    private GameObject[] channelPrefabs;

    

    [Tooltip("How long to wait before spawning a new victim after the previous one dies or is rescued.")]
    public float DelayInSecondsBetweenVictims = 5;

    [SerializeField]
    private GameObject breakingNewsUI;

    [SerializeField]
    private VisualEffect bloodStream, bloodBurst;

    public Channel creditsChannel;
    public GameObject bloodVideo;
    public UVOffsetYAnim beltUVAnimation;
    public GameObject victimSpawnPoint;
    public GameObject killPosition;
    public GameObject[] victimPrefabs;
    public Vector3 victimLocalRotation = new Vector3(0, 279.761871f, 0);
    public static event Action<string> VictimDied;
    public static event Action NewVictimSpawned;
    public static event Action WillInterruptBroadcastToChangeToKillingChannel;
    public static event Action ChangedToKillingChannel;


    public bool CurrentVictimIsKiller => CurrentVictim == killer;
    public bool BeginGameplay { get;  set; } = false;
  
    public int KillingFloorChannelIndex { get; private set; }
    public float TimeUntilNextKill { get; private set; } = 0;
    public int ChannelIndex { get; private set; } = 0;  

    public bool BlockChannelInput { get; private set; } = false;
    public bool BlockPasscodeInput { get; private set; } = false;
    public Victim CurrentVictim { get; private set; }

    public int KillCount { get; private set; } = 0;
    public int SaveCount { get; private set; } = 0;


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
    private bool waitingToKillVictim = false;
    private bool isKilling = false;
    private ChannelChangeEffects channelChangeEffects;
    public bool IsReadyForEnding => SaveCount + KillCount >= numberOVictimsToSpawnBoss;
   

    private void Start()
    {
        bloodVideo.SetActive(false);
        channelChangeEffects = GetComponent<ChannelChangeEffects>();
        ChannelIndex = 0;
        InitializeChannelArray();
        channels[ChannelIndex].ChannelEntered?.Invoke();
        channelNumberText.gameObject.SetActive(false);
        UpdateChannelText();
        // randomize the victimPrefabs
        victimPrefabs = victimPrefabs.OrderBy(x => Guid.NewGuid()).ToArray();
        InitializeVictimsOutOfViewPosition();
        SetNewVictimName();
        ResetKillTimer();
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
        if (!BeginGameplay)
            return;

        // if the victim is dead we speed up the belt to make it look cooler.
        if (!waitingToChangeToKillingFloor && !currentVictimIsDead)
        {
            float timeMultiplier = isKilling ? deathBeltSpeedMultiplier : GetMultiplierBasedOnPenaltyStatus();
            UpdateKillTimer(timeMultiplier);
            UpdateBeltSpeed(timeMultiplier);
            UpdateVictimPosition(timeMultiplier);
        }
        else
        {
            UpdateKillTimer(0);
            UpdateBeltSpeed(0);
            UpdateVictimPosition(0);
        }

        if (!currentVictimIsDead && TimeUntilNextKill <= channelChangeEffects.rampUpTime + channelChangeEffects.rampDownTime && !waitingToChangeToKillingFloor)
        {
            BlockChannelInput = true;
        }

        if (!currentVictimIsDead && TimeUntilNextKill <= 0 && !waitingToChangeToKillingFloor)
        {
            BlockChannelInput = true;
            waitingToKillVictim = true;
            BlockPasscodeInput = true;
            if (ChannelIndex != KillingFloorChannelIndex)
            {
                waitingToChangeToKillingFloor = true;
                breakingNewsUI.SetActive(true);
                WillInterruptBroadcastToChangeToKillingChannel?.Invoke();
                StartCoroutine(SwitchToKillingFloorAfterDelay());
            }
            else if (ChannelIndex == KillingFloorChannelIndex)
            {
                BlockPasscodeInput = true;
                KillVictim();
            }            
        }
    }

    private IEnumerator SwitchToKillingFloorAfterDelay()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(ChangeChannel(KillingFloorChannelIndex));
        waitingToChangeToKillingFloor = false;
        breakingNewsUI.SetActive(false);
    }

    private IEnumerator SpawnNewVictimAfterDelay()
    {
        yield return new WaitForSeconds(DelayInSecondsBetweenVictims);
        SetUpNextVictim();
        waitingToKillVictim = false;
    }

    private IEnumerator DisableBloodAfterDelay()
    {
        yield return new WaitForSeconds(DelayInSecondsBetweenVictims - 1);
        bloodStream.Stop();
        bloodVideo.SetActive(false);
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
        isKilling = false;
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
        BlockChannelInput = false;
        BlockPasscodeInput = false;
    }

    private void SetNewVictimName()
    {
        if (IsReadyForEnding)
        {
            CurrentVictim = killer;
            CurrentVictim.Name = KillerName;
        }
        else
        {
            var victim = victimPrefabs[victimIndex].GetComponent<Victim>();
            CurrentVictim = victim;
            victim.SetRandomName();
        }
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
        if (IsReadyForEnding)
        {
            // Killer is manually positioned.
            killer.gameObject.SetActive(true); 
            //killer.transform.SetParent(victimSpawnPoint.transform, false);
            //killer.transform.localPosition = Vector3.zero;
            //killer.transform.rotation = Quaternion.Euler(victimLocalRotation);
        }
        else 
        { 
            victimPrefabs[victimIndex].transform.SetParent(victimSpawnPoint.transform, false);
            victimPrefabs[victimIndex].transform.localPosition = Vector3.zero;
            victimPrefabs[victimIndex].transform.rotation = Quaternion.Euler(victimLocalRotation);
        }
    }

    private void KillVictim()
    {
        var shouldGoToCredits = CurrentVictimIsKiller;
        currentVictimIsDead = true;
        KillCount++;
        var animController = victimSpawnPoint.GetComponentInChildren<Animator>();
        animController.SetTrigger("Die");
        VictimDied?.Invoke(CurrentVictim.Name);
        bloodBurst.Play();
        bloodStream.Play();
        bloodVideo.SetActive(true);
        isKilling = true;
        StartCoroutine(DisableBloodAfterDelay());
        if (shouldGoToCredits)
        {
            ChangeToCreditsChannel();
        }
        else
        {
            StartCoroutine(SpawnNewVictimAfterDelay());
        }
    }

    private IEnumerator ChangeToCreditsChannel()
    {
        BlockChannelInput = true;
        channelChangeEffects.RampUpChannelChangeEffect();
        yield return new WaitForSeconds(channelChangeEffects.rampUpTime);
        channels[ChannelIndex].ChannelExited?.Invoke();
        StartCoroutine(ShowTextForFixedDuration());
        channelNumberText.text = creditsChannel.ChannelNumber.ToString();
        creditsChannel.ChannelEntered?.Invoke();
        channelChangeEffects.RampDownChannelChangeEffect();
    }

    private void InitializeChannelArray()
    {
        channels = new Channel[channelPrefabs.Length];
        for (int i = 0; i < channelPrefabs.Length; i++)
        {
            channels[i] = channelPrefabs[i].GetComponent<Channel>();
            if (channels[i].name == "KillingFloorChannel")
            {
                KillingFloorChannelIndex = i;
            }
        }
    }

    public void ChannelUp()
    {
        if (BlockChannelInput)
            return;

        if (ChannelIndex == channels.Length - 1)
        {
            StartCoroutine(ChangeChannel(0));
        }
        else
        {
            StartCoroutine(ChangeChannel(ChannelIndex + 1));
        }
    }

    public void ChannelDown()
    { 
        if (BlockChannelInput)
            return;

        if (ChannelIndex == 0)
        {
            StartCoroutine(ChangeChannel(channels.Length - 1));
        }
        else
        {
            StartCoroutine(ChangeChannel(ChannelIndex - 1));
        }
    }

    private IEnumerator ChangeChannel(int newIndex)
    {
        if (newIndex == ChannelIndex)
            yield break;
        BlockChannelInput = true;
        channelChangeEffects.RampUpChannelChangeEffect();
        yield return new WaitForSeconds(channelChangeEffects.rampUpTime);
        channels[ChannelIndex].ChannelExited?.Invoke();
        ChannelIndex = newIndex;
        UpdateChannelText();
        channels[newIndex].ChannelEntered?.Invoke();
        if (ChannelIndex == KillingFloorChannelIndex)
            ChangedToKillingChannel?.Invoke();
        channelChangeEffects.RampDownChannelChangeEffect();
        if (!waitingToKillVictim)
            BlockChannelInput = false;
    }

    public void GoToRandomChannel()
    {
        var randomIndex = UnityEngine.Random.Range(0, channels.Length);
        StartCoroutine(ChangeChannel(randomIndex));
    }

    private void UpdateChannelText()
    {
        if (channelTextCoroutine != null)
        {
            StopCoroutine(channelTextCoroutine);
        }
        channelNumberText.text = channels[ChannelIndex].ChannelNumber.ToString();
        channelTextCoroutine = StartCoroutine(ShowTextForFixedDuration());
    }

    private IEnumerator ShowTextForFixedDuration()
    {
        var duration = 3.0f;
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
            SaveCount++;
            victimIsBeingRescued = true;
            victimSpawnPoint.transform.position = outOfViewPosition;
            if (CurrentVictimIsKiller)
                ChangeToCreditsChannel();
        }        
    }
}
