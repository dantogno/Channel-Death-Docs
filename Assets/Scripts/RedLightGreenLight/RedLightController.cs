using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public enum LightState
{ 
    waiting,
    red,
    green,
}

public class RedLightController : MonoBehaviour
{
    public float chargeLength;
    public Vector2 lightLength;
    private float currentChargeLengthTimer = 0f;
    private bool won = false;
    public GameObject[] graphics;
    private bool charging = false;
    public LightState lightState = LightState.waiting;
    private float currentCharge;
    public Image[] fillBars;
    public Material[] babyMats;
    private bool inGame = false;
    public TMP_Text[] eyeTexts;
    public AudioSource redSource;
    public AudioSource greedSource;
    public AudioSource laugh;
    private string numberRequired;


    private void Awake()
    {
        PasscodeManager.NewPasscodeSet += NewPasscodeSet;
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.Jump.started += InputPressed;
        InputManager.InputActions.Gameplay.Jump.canceled += InputRelease;
        foreach (GameObject go in graphics) {
            go.SetActive(true);
        }
        babyMats[0].SetFloat("_Intensity", 3.05f);
        babyMats[0].SetFloat("_Amibet", won == true ? .3f : 0f);
        babyMats[1].SetFloat("_Blend", 4);
        babyMats[1].SetFloat("_EyeSpeed", 0);
        if (won) laugh.Play();
        foreach (Image fillbar in fillBars) {
            fillbar.color = Color.green;
        }
        currentCharge = won == true ? chargeLength: 0;
        lightState = LightState.green;
        foreach (TMP_Text text in eyeTexts) {
            text.enabled = won;
        }
    }

    private void Update()
    {
        if (won) return;
        if (charging) {
            if (!inGame) {
                inGame = true;
                currentChargeLengthTimer = lightLength.x;
            }
            if (lightState == LightState.green) {
                currentCharge += Time.deltaTime;
                if (currentCharge >= chargeLength) {
                    WinGame();
                }
            }
            if (lightState == LightState.red) {
                currentCharge -= Time.deltaTime;
                if (currentCharge < 0) currentCharge = 0;
            }
        } else {
            currentCharge -= Time.deltaTime * .2f;
            if (currentCharge < 0) currentCharge = 0f;
        }
        if (lightState == LightState.red) {
            babyMats[0].SetFloat("_Intensity", Mathf.Lerp(.2f,.4f, currentCharge/chargeLength));
            babyMats[1].SetFloat("_EyeSpeed", Mathf.Lerp(.3f, 1f, currentCharge/ chargeLength));
        }
        if (inGame) {
            currentChargeLengthTimer -= Time.deltaTime;
            if (currentChargeLengthTimer <= 0) {
                ChangeLightState();
                SetChargeLength();
            }
        }
        foreach (Image fillBar in fillBars) {
            fillBar.fillAmount = currentCharge / chargeLength;
        }
    }

    void ChangeLightState()
    {
        if (lightState == LightState.red) {
            lightState = LightState.green;
            babyMats[0].SetFloat("_Intensity", 3.05f);
            babyMats[1].SetFloat("_Blend", 4);
            babyMats[1].SetFloat("_EyeSpeed", 0);
            babyMats[0].SetFloat("_Amibet", 0f);
            foreach (Image fillbar in fillBars) {
                fillbar.color = Color.green;
            }
            redSource.Pause();
        } else {
            lightState = LightState.red;
            babyMats[0].SetFloat("_Intensity", 0);
            babyMats[1].SetFloat("_Blend", 0);
            babyMats[1].SetFloat("_EyeSpeed", .3f);
            babyMats[0].SetFloat("_Amibet", 0.3f);
            foreach (Image fillbar in fillBars) {
                fillbar.color = Color.red;
            }
            redSource.Play();
        }
    }

    void SetChargeLength()
    {
        currentChargeLengthTimer = Random.Range(lightLength.x, lightLength.y);
    }

    public void WinGame()
    {
        lightState = LightState.waiting;
        won = true;
        inGame = false;
        foreach (TMP_Text text in eyeTexts) {
            text.text = 4.ToString();
            text.enabled = true;
        }
        babyMats[0].SetFloat("_Intensity", 3.05f);
        babyMats[1].SetFloat("_Blend", 4);
        babyMats[1].SetFloat("_EyeSpeed", 0);
        babyMats[0].SetFloat("_Amibet", 0.3f);
        laugh.Play();

    }

    private void InputPressed(InputAction.CallbackContext context)
    {
        charging = true;
    }

    private void InputRelease(InputAction.CallbackContext context)
    {
        charging = false;
    }

    void NewPasscodeSet(string str)
    {
        won = false;
    }

    private void OnDestroy()
    {
        PasscodeManager.NewPasscodeSet -= NewPasscodeSet;
    }

    private void OnDisable()
    {
        redSource.Stop();
        greedSource.Stop();
    }
}
