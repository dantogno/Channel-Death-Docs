using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public UVOffsetYAnim beltUVAnimation;
    public GameObject victimSpawnPoint;
    public GameObject killPosition;
    public GameObject[] victimPrefabs;
    public Vector3 victimLocalRotation = new Vector3(0, 279.761871f, 0);

    public float TimeUntilNextKill { get; private set; } = 0;
    public int ChannelIndex { get; private set; } = 0;

    private float conveyorBeltSpeed;
    private bool currentVictimIsDead;
    private Channel[] channels;
    private Vector3 originalSpawnPosition;
    private int victimIndex = 0;
    private Vector3 outOfViewPosition;

    private void Start()
    {
        outOfViewPosition = killPosition.transform.position + Vector3.down * 100;
        foreach (var victim in victimPrefabs)
        {
            victim.transform.position = outOfViewPosition;
        }
        originalSpawnPosition = victimSpawnPoint.transform.position;
        InitializeChannelArray();
        ChannelIndex = 0;
        channels[ChannelIndex].ChannelEntered?.Invoke();
        channelNumberText.gameObject.SetActive(false);
        UpdateChannelText();

        // the conveyorBeltSpeed should move victim position to killPosition in killTimeInSeconds
        conveyorBeltSpeed = Vector3.Distance(victimSpawnPoint.transform.position, killPosition.transform.position) / killTimeInSeconds;
        beltUVAnimation.speed = conveyorBeltSpeed/2;
        ResetKillTimer();
        MoveVictimToPosition();
    }

    private void Update()
    {
        UpdateKillTimer(); 

        // move the victim along the conveyor belt at a constant speed
        victimSpawnPoint.transform.position += victimSpawnPoint.transform.forward * conveyorBeltSpeed * Time.deltaTime;

        if (!currentVictimIsDead && TimeUntilNextKill <=0 )
        {
            KillVictim();
        }
    }

    private void UpdateKillTimer()
    {
        TimeUntilNextKill -= Time.deltaTime;
    }

    private void SetUpNextVictim()
    {
        // Move old victim out of view
        victimPrefabs[victimIndex].transform.SetParent(null);
        victimPrefabs[victimIndex].transform.position = outOfViewPosition;
        UpdateVictimIndex();
        MoveVictimToPosition();
        ResetKillTimer();
        currentVictimIsDead = false;
    }

    private void ResetKillTimer()
    {
        TimeUntilNextKill = killTimeInSeconds;
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
    }

    private void InitializeChannelArray()
    {
        channels = new Channel[channelPrefabs.Length];
        for (int i = 0; i < channelPrefabs.Length; i++)
        {
            channels[i] = channelPrefabs[i].GetComponent<Channel>();
        }
        channels.OrderBy(c => c.ChannelNumber);
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
        channels[ChannelIndex].ChannelExited?.Invoke();
        ChannelIndex = newIndex;
        channels[newIndex].ChannelEntered?.Invoke();
        UpdateChannelText();
    }

    private void UpdateChannelText()
    {
        StopAllCoroutines();
        StartCoroutine(ShowTextForFixedDuration());
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
        SetUpNextVictim();
    }

    private void OnChannelDownPressed(CallbackContext context)
    {
        ChannelDown();
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.ChannelUp.performed += OnChannelUpPressed;
        InputManager.InputActions.Gameplay.ChannelDown.performed += OnChannelDownPressed;
    }
}
