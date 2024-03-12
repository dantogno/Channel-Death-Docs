using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Rendering;
public class PowerPlayerController : MonoBehaviour
{
    public static PowerPlayerController Instance;
    
    [SerializeField]
    Image[] fillBars;
    enum PowerState { idle, consuming}
    PowerState powerState;
    [SerializeField]
    float MaxPower, PowerDepletionRate;
    float powerRemaining;

    public Monster monster;
    public float LoseThreshHold;
    float currentDistance;

    [SerializeField]
    TextMeshProUGUI clockText;
    [SerializeField]
    string[] hours;
    [SerializeField]
    float hourLength;
    int currentHour;

    [SerializeField]
    Light flashlight;

    [SerializeField]
    AudioClip lightOn, lightOff;

    [SerializeField]
    AudioSource flashLightSource;

    [SerializeField]
    AudioSource whisperSource;

    [SerializeField]
    Volume postProcess;


    bool won;
    bool lost;

    [SerializeField]
    GameObject jumpscareMonster;
    [SerializeField]
    float JumpscareTime;

    [SerializeField]
    TextMeshProUGUI numText;

    [SerializeField]
    Channel parentChannel;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Setup();
        started = true;
        PasscodeManager.NewPasscodeSet += NewPasscodeSet;
    }

    private void OnDestroy()
    {
        PasscodeManager.NewPasscodeSet -= NewPasscodeSet;
    }

    private void NewPasscodeSet(string str)
    {
        Setup();
        numText.text = PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString();
    }

    private void Setup()
    {
        StopAllCoroutines();
        jumpscareMonster.SetActive(false);
        flashlight.enabled = false;        
        currentHour = 0;
        clockText.text = hours[0];
        powerState = PowerState.idle;
        powerRemaining = MaxPower;
        monster.SetupMonster();
        waitingForNextHour = false;
        lost = false;
        numText.enabled = false;
    }

    bool waitingForNextHour;
    IEnumerator IncreaseTime()
    {
        waitingForNextHour = true;
        yield return new WaitForSeconds(hourLength);
        currentHour += 1;
        clockText.text = hours[currentHour];
        if(currentHour >= hours.Length - 1 && !lost)
        {
            Debug.Log("You win! Display Number");
            won = true;
            numText.text = PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString();
            numText.enabled = true;
        }
        else
        {
            waitingForNextHour=false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(lost)
        {
            return;
        }
        if(!won)
        {
            switch (powerState)
            {
                case PowerState.idle:
                    monster.monsterState = Monster.MonsterState.approaching;
                    break;
                case PowerState.consuming:
                    monster.monsterState = Monster.MonsterState.retracting;
                    powerRemaining -= PowerDepletionRate * Time.deltaTime;
                    if (powerRemaining <= 0)
                    {
                        turnLightOff(false);
                    }
                    break;
            }
        }
        else
        {
            monster.monsterState = Monster.MonsterState.retracting;
            flashlight.enabled = true;
        }
        currentDistance = Vector3.Distance(transform.position, monster.transform.position);
        updateProgressBar();
        if(currentDistance < LoseThreshHold && !won)
        {
            JumpScare();
        }
        if(!waitingForNextHour)
        {
            StartCoroutine(IncreaseTime());
        }
        UpdatePostProcess();
        whisperSource.volume = MapValueReverseExponential(currentDistance, monster.startDistance, LoseThreshHold, 0, 0.8f);
    }
    private void JumpScare()
    {
        lost = true;
        StartCoroutine(LoseJumpscare());
    }

    IEnumerator LoseJumpscare()
    {
        turnLightOff(false);
        monster.HideMonster();
        jumpscareMonster.SetActive(true);
        yield return new WaitForSeconds(JumpscareTime);
        Setup();
    }

    private void UpdatePostProcess()
    {
        postProcess.weight = MapValue(currentDistance, monster.startDistance, LoseThreshHold, 0, 1);
    }
    public float MapValue(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;
        return mappedValue;
    }

    public static float MapValueReverseExponential(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);
        normalizedValue = Mathf.Pow(normalizedValue, 3f);
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;
        return mappedValue;
    }
    private void updateProgressBar()
    {
        
        foreach (Image fillBar in fillBars)
        {
            fillBar.fillAmount = powerRemaining;
        }
    }

    bool started;
    private void OnEnable()
    {
        if (started)
        {
            Setup();
        }
        RenderSettings.fogDensity = 3f;
        InputManager.InputActions.Gameplay.Power.performed += PowerOn;
        InputManager.InputActions.Gameplay.Power.canceled += PowerOff;
    }

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.Power.performed -= PowerOn;
        InputManager.InputActions.Gameplay.Power.canceled -= PowerOff;
        RenderSettings.fogDensity = 0.01f;
        StopAllCoroutines();
    }

    private void PowerOn(InputAction.CallbackContext c)
    {
        switch (powerState)
        {
            case PowerState.idle:
                turnLightOn();
                break;
            case PowerState.consuming:
                turnLightOff(true);
                break;
        }
       
    }

    private void turnLightOn()
    {
        if (powerRemaining > 0 && !lost)
        {
            powerState = PowerState.consuming;
            flashlight.enabled = true;
            RenderSettings.fogDensity = 0.2f;
            flashLightSource.PlayOneShot(lightOn, 0.5f);
            monster.IncreaseApproach();
        }
    }

    private void PowerOff(InputAction.CallbackContext c)
    {
        //turnLightOff(true);    
    }
    void turnLightOff(bool playSound)
    {
        powerState = PowerState.idle;
        flashlight.enabled = false;
        RenderSettings.fogDensity = 3;
        if(playSound )
        {
            flashLightSource.PlayOneShot(lightOff, 0.5f);
        }
    }
}
