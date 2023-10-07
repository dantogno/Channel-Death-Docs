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

    public float TimeUntilNextKill { get; private set; } = 0;

    private Channel[] channels;

    public GameObject victimSpawnPoint;

    public GameObject killPosition;

    public int ChannelIndex { get; private set; } = 0;

    private float conveyorBeltSpeed;

    private void Start()
    {
        InitializeChannelArray();
        ChannelIndex = 0;
        channels[ChannelIndex].ChannelEntered?.Invoke();
        channelNumberText.gameObject.SetActive(false);
        UpdateChannelText();

        // the conveyorBeltSpeed should move victim position to killPosition in killTimeInSeconds
        conveyorBeltSpeed = Vector3.Distance(victimSpawnPoint.transform.position, killPosition.transform.position) / killTimeInSeconds;
        SetUpNextVictim();
    }

    private void Update()
    {
        UpdateKillTimer();
        // move victim along conveyor belt
        //victimSpawnPoint.transform.position = 
          //  Vector3.MoveTowards(victimSpawnPoint.transform.position, killPosition.transform.position, conveyorBeltSpeed * Time.deltaTime);

        // move the victim along the conveyor belt at a constant speed
        victimSpawnPoint.transform.position += victimSpawnPoint.transform.forward * conveyorBeltSpeed * Time.deltaTime;
    }

    private void UpdateKillTimer()
    {
        TimeUntilNextKill -= Time.deltaTime;
    }

    private void SetUpNextVictim()
    {
        TimeUntilNextKill = killTimeInSeconds;
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
