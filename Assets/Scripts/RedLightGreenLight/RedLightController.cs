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
    public GameObject[] babyHeads;
    private List<Material> instanceHeadMats = new List<Material>();
    private List<Material> instanceBabyEyes = new List<Material>();
    private bool inGame = false;
    public TMP_Text[] eyeTexts;
    public AudioSource redSource;
    public AudioSource greedSource;
    public AudioSource laugh;
    private string numberRequired;
    private Channel parentChannel;


    private void Awake()
    {
        PasscodeManager.NewPasscodeSet += NewPasscodeSet;
        parentChannel = GetComponentInParent<Channel>();
        foreach (GameObject go in babyHeads) {
            instanceHeadMats.Add(go.GetComponent<Renderer>().materials[0]);
            instanceBabyEyes.Add(go.GetComponent<Renderer>().materials[2]);
        }
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.Sleep.started += InputPressed;
        InputManager.InputActions.Gameplay.Sleep.canceled += InputRelease;
        foreach (GameObject go in graphics) {
            go.SetActive(true);
        }
        SetHeads(3.05f, won == true ? .3f : 0f);
        //babyMats[0].SetFloat("_Intensity", 3.05f);
        //babyMats[0].SetFloat("_Amibet", won == true ? .3f : 0f);
        //babyMats[1].SetFloat("_Blend", 4);
        //babyMats[1].SetFloat("_EyeSpeed", 0);
        SetEyes(4, 0);
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

    void SetHeads(float intensity, float amibet)
    {
        foreach (Material mat in instanceHeadMats) {
            if (intensity != -1) {
                mat.SetFloat("_Intensity", intensity);
            }
            if (amibet != -1) {
                mat.SetFloat("_Amibet", amibet);
            }
        }
    }

    void SetEyes(float blend, float eyespeed)
    {
        foreach (Material mat in instanceBabyEyes) {
            if (blend != -1) {
                mat.SetFloat("_Blend", blend);
            }
            if (eyespeed != -1) {
                mat.SetFloat("_EyeSpeed", eyespeed);
            }
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
            //currentCharge -= Time.deltaTime * .2f;
            if (currentCharge < 0) currentCharge = 0f;
        }
        if (lightState == LightState.red) {
            SetHeads(Mathf.Lerp(.2f, .4f, currentCharge / chargeLength), -1);
            //babyMats[0].SetFloat("_Intensity", Mathf.Lerp(.2f,.4f, currentCharge/chargeLength));
            //babyMats[1].SetFloat("_EyeSpeed", Mathf.Lerp(.3f, 1f, currentCharge/ chargeLength));
            SetEyes(-1, Mathf.Lerp(.3f, 1f, currentCharge / chargeLength));
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
            SetHeads(3.05f, 0f);
            SetEyes(4, 0);
            //babyMats[0].SetFloat("_Intensity", 3.05f);
            //babyMats[1].SetFloat("_Blend", 4);
            //babyMats[1].SetFloat("_EyeSpeed", 0);
            //babyMats[0].SetFloat("_Amibet", 0f);
            foreach (Image fillbar in fillBars) {
                fillbar.color = Color.green;
            }
            redSource.Pause();
        } else {
            lightState = LightState.red;
            SetHeads(0f, .3f);
            SetEyes(0, .3f);
            //babyMats[0].SetFloat("_Intensity", 0);
            //babyMats[1].SetFloat("_Blend", 0);
            //babyMats[1].SetFloat("_EyeSpeed", .3f);
            //babyMats[0].SetFloat("_Amibet", 0.3f);
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
            text.text = PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString(); ;
            text.enabled = true;
        }
        SetHeads(3.05f, .3f);
        SetEyes(4, 0);
        //babyMats[0].SetFloat("_Intensity", 3.05f);
        //babyMats[1].SetFloat("_Blend", 4);
        //babyMats[1].SetFloat("_EyeSpeed", 0);
        //babyMats[0].SetFloat("_Amibet", 0.3f);
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
