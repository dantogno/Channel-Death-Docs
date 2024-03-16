using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.InputSystem.InputAction;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("Game ends after this many minutes.")]
    [SerializeField]
    private int totalTimeLimitInMinutes = 180;

    [Tooltip("Victim dies after this many seconds.")]
    public float KillTimeInSeconds = 180;

    public string KillerName = "The Broadcast Killer";

    public AudioSource channelChangeAudio;
    public int numberOVictimsToSpawnBoss = 3;
    public float timePenaltyMultiplier = 5;
    public float timePenaltyDuration = 2f;
    public float distanceToClearBelt = 3f;
    public float deathBeltSpeedMultiplier = 5;
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

    public float TotalTimeLimitInSeconds => totalTimeLimitInMinutes * 60;

    /// <summary>
    /// Models used for victims. They have a Victim component to mark if they are male or female, 
    /// which is used to determine the name.
    /// </summary>
    [Tooltip("Models used for victims. Should be in the scene. They have a Victim component to mark if they are male or female for name gen")]
    public GameObject[] victimModels;
    public Vector3 victimLocalRotation = new Vector3(0, 279.761871f, 0);
    public static event Action<Victim> VictimDied;
    public static event Action NewVictimSpawned;
    public static event Action WillInterruptBroadcastToChangeToKillingChannel;
    public static event Action ChangedToKillingChannel;
    public static event Action VictimRescued;

    public static float bonusSpeed = 1f;

    public bool CurrentVictimIsKiller => CurrentVictim == killer;
    //public bool BeginGameplay { get;  set; } = false;
  
    public int KillingFloorChannelIndex { get; private set; }
    public float TimeUntilNextKill { get; private set; } = 0;
    public int ChannelIndex { get; private set; } = 0;  

    public bool BlockChannelInput { get; private set; } = false;
    public bool BlockPasscodeInput { get; private set; } = false;
    public Victim CurrentVictim
    {
        get
        {
            if (currentVictim_useProprety == null)
            {
                SetUpNextVictim();
            }   
            return currentVictim_useProprety;
        }
        private set => currentVictim_useProprety = value;
    }

    public int KillCount => SaveSystem.CurrentGameData.VictimHistory.Where(v => v.State == Victim.VictimState.Dead).Count();
    public int RescuedCount => SaveSystem.CurrentGameData.VictimHistory.Where(v => v.State == Victim.VictimState.Rescued).Count();

    private float conveyorBeltDefaultSpeed;
    private bool currentVictimIsDead;
    private Channel[] channels;
    private Vector3 originalSpawnPosition;
    /// <summary>
    /// index for victimModels array
    /// </summary>
    private int victimModelIndex = 0;
    private Vector3 outOfViewPosition;
    private Coroutine channelTextCoroutine;
    private float timePenaltyTimer = 0;
    private bool victimIsBeingRescued = false;
    private bool waitingToChangeToKillingFloor = false;
    private bool waitingToKillVictim = false;
    private bool isKilling = false;
    private Victim currentVictim_useProprety = null;
    private ChannelChangeEffects channelChangeEffects;
    private MinigameSelector minigameSelector;
    private bool neverChangedChannel = true;
    public bool IsReadyForEnding => overarchingPuzzleController.IsQuizComplete; //RescuedCount + KillCount >= numberOVictimsToSpawnBoss;
    private OverarchingPuzzleController overarchingPuzzleController;
    private bool inlossLoad = false;

    //protected override void Awake()
    //{
    //    //base.Awake();

    //}
    public Vector3 LastVictimRescuePos;

    private void Start()
    {
        // hide the mouse cursor
        Cursor.visible = false;
        Victim.InitializeNameListsFromCSV();
        if(bloodVideo != null)
        {
            bloodVideo.SetActive(false);
        }
        overarchingPuzzleController = FindAnyObjectByType<OverarchingPuzzleController>();
        channelChangeEffects = GetComponent<ChannelChangeEffects>();
        ChannelIndex = 0;
        InitializeChannelArray();
        channelNumberText.gameObject.SetActive(false);
        UpdateChannelText();
        minigameSelector = GetComponent<MinigameSelector>();
        // randomize the victimPrefabs
        victimModels = victimModels.OrderBy(x => Guid.NewGuid()).ToArray();
        InitializeVictimsOutOfViewPosition();
        SetNewVictimName();
        ResetKillTimer();
        MoveVictimToPosition();
        channels[ChannelIndex].ChannelEntered?.Invoke();
        conveyorBeltDefaultSpeed = Vector3.Distance(victimSpawnPoint.transform.position, killPosition.transform.position) / KillTimeInSeconds;
    }

    private void InitializeVictimsOutOfViewPosition()
    {
        outOfViewPosition = killPosition.transform.position + Vector3.down * 100;
        foreach (var victim in victimModels)
        {
            victim.transform.position = outOfViewPosition;
        }
        originalSpawnPosition = victimSpawnPoint.transform.position;
    }

    private void Update()
    {
        //if (!BeginGameplay) { return; }

        // if the victim is dead we speed up the belt to make it look cooler.
        if (!waitingToChangeToKillingFloor && !currentVictimIsDead)
        {
            float timeMultiplier =  GetMultiplierBasedOnPenaltyStatus() * bonusSpeed;
            UpdateKillTimer(timeMultiplier);
            UpdateBeltSpeed(timeMultiplier);
            UpdateVictimPosition(timeMultiplier);
        }
        else
        {    
            float timeMultiplier = isKilling ? deathBeltSpeedMultiplier : 0;
            Debug.Log(timeMultiplier);
            UpdateKillTimer(timeMultiplier);
            UpdateBeltSpeed(timeMultiplier);
            UpdateVictimPosition(timeMultiplier);
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
        // Update the time remaining
        SaveSystem.CurrentGameData.TimeRemainingInSeconds -= Time.deltaTime;

        //Trigger Loss
        if (SaveSystem.CurrentGameData.TimeRemainingInSeconds < 0 && !inlossLoad) {
            inlossLoad = false;

            UnityEngine.SceneManagement.SceneManager.LoadScene("GameLose");
        }
    }


    private IEnumerator SwitchToKillingFloorAfterDelay()
    {
        yield return new WaitForSeconds(2);
        StartCoroutine(ChangeChannel(KillingFloorChannelIndex));
        waitingToChangeToKillingFloor = false;
        breakingNewsUI.SetActive(false);
    }

    public void SpawnNewVictimAfterDelay()
    {
        StartCoroutine(SetUpNewVictimAfterDelayCoroutine());
    }

    private IEnumerator SetUpNewVictimAfterDelayCoroutine()
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
        // Save the previous victim to history, if there is one
        if (currentVictim_useProprety != null)
        {
            SaveSystem.CurrentGameData.VictimHistory.Add(CurrentVictim.VictimData);
            SaveSystem.SaveGameToFile();
        }
        isKilling = false;
        // Move old victim out of view
        victimModels[victimModelIndex].transform.SetParent(null);
        victimModels[victimModelIndex].transform.position = outOfViewPosition;
        
        UpdateVictimIndex();
        SetNewVictimName();
        MoveVictimToPosition();
        ResetKillTimer();
        currentVictimIsDead = false;
        victimIsBeingRescued = false;
        BlockChannelInput = false;
        BlockPasscodeInput = false;
        NewVictimSpawned?.Invoke();
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
            if(victimModelIndex <= victimModels.Count() - 1)
            {
                var victim = victimModels[victimModelIndex].GetComponent<Victim>();
                CurrentVictim = victim;
                victim.SetRandomName();
            }
        }
    }

    private void ResetKillTimer()
    {
        TimeUntilNextKill = KillTimeInSeconds;
        timePenaltyTimer = 0;
    }

    /// <summary>
    /// Update victim index so we get a different victim next time
    /// </summary>
    private void UpdateVictimIndex()
    {
        victimModelIndex = victimModelIndex == victimModels.Length - 1 ? 0 : victimModelIndex + 1;
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
            if(victimModelIndex <= victimModels.Count() - 1)
            {
                victimModels[victimModelIndex].transform.SetParent(victimSpawnPoint.transform, false);
                victimModels[victimModelIndex].transform.localPosition = Vector3.zero;
                victimModels[victimModelIndex].transform.rotation = Quaternion.Euler(victimLocalRotation);
            }
        }
    }

    private void KillVictim()
    {
        var shouldGoToCredits = CurrentVictimIsKiller;
        currentVictimIsDead = true;
        CurrentVictim.State = Victim.VictimState.Dead;
 
        var animController = victimSpawnPoint.GetComponentInChildren<Animator>();
        animController.SetTrigger("Die");
        VictimDied?.Invoke(CurrentVictim);
        bloodBurst.Play();
        bloodStream.Play();
        bloodVideo.SetActive(true);
        isKilling = true;
        StartCoroutine(DisableBloodAfterDelay());
        if (shouldGoToCredits)
        {
            StartCoroutine(ChangeToCreditsChannel());
        }
        else
        {
            SpawnNewVictimAfterDelay();
        }
    }

    private IEnumerator ChangeToCreditsChannel()
    {
        yield return new WaitForSeconds(DelayInSecondsBetweenVictims);
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
        if (neverChangedChannel)
        {
            neverChangedChannel = false;
            newIndex = KillingFloorChannelIndex;
        }

        if (newIndex == ChannelIndex)
            yield break;
        BlockChannelInput = true;
        channelChangeEffects.RampUpChannelChangeEffect();
        channelChangeAudio.Play();
        yield return new WaitForSeconds(channelChangeEffects.rampUpTime);
        channels[ChannelIndex].ChannelExited?.Invoke();
        ChannelIndex = newIndex;
        UpdateChannelText();
        if (minigameSelector.ActiveChannel(channels[newIndex])){
            channels[newIndex].ChannelEntered?.Invoke();
        }
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

    public void GoToKillingFloorChannel()
    {
        StartCoroutine(ChangeChannel(KillingFloorChannelIndex));
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

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.ChannelUp.performed -= OnChannelUpPressed;
        InputManager.InputActions.Gameplay.ChannelDown.performed -= OnChannelDownPressed;
        PasscodeManager.ValidationCompleted -= OnPasscodeValidationCompleted;
    }

    private void OnPasscodeValidationCompleted(bool isSuccessful)
    {
        if (isSuccessful)
        {
            RescueVictim();
            victimSpawnPoint.transform.position = outOfViewPosition;
            if (CurrentVictimIsKiller)
                ChangeToCreditsChannel();
        }
    }
    
    private void RescueVictim()
    {
        // Unneeded since we use victim history list now... Right?
        //SaveCount++;
        LastVictimRescuePos = CurrentVictim.transform.position;
        victimIsBeingRescued = true;
        CurrentVictim.State = Victim.VictimState.Rescued;
        VictimRescued?.Invoke();
    }
}
