using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class VolumeMinigame : MonoBehaviour
{
    public float testSpeedScale = 1;
    public float commercialViewTimeThreshold = 8;
    public float musicMutedTimeThreshold = 5;
    public Canvas volumeCanvas;
    public GameObject jumpScare;
    public TMP_Text volumeSymbolText;
    public TMP_Text muteText;
    public AudioMixer audioMixer;
    public VideoPlayer commercialVideo;
    public GameObject phonographImage, commercialRenderTexture;
    public AudioSource music, instructions, instructions2, instructions3, clueAudioSource;
    public AudioClip[] clueClips;
    private string volumeSymbolString = string.Empty;
    private const int maxVolume = 20;
    private const char noVolumeSymbol = '.';
    private const char volumeSymbol = '|';
    private bool volumeUpPressed, volumeDownPressed= false;
    private float repeatInputDelay = 0.3f;
    private float repeatInputTimer = 0;
    private bool isCommercialPlaying = false;
    private float commercialViewTimer = 0;
    private float musicMutedTimer = 0;
    private bool playingJumpScare = false;
    private bool isFirstTime = true;
    private bool shouldSkipToClue = false;

    private int clue = -1;
    private int oldClue = -1;
    private Channel parentChannel;


    [SerializeField]
    TextMeshProUGUI captionBox;

    [SerializeField]
    List<string> phonographCaptions;

    [SerializeField]
    List<string> clueCaptions;

    [SerializeField]
    Volume postProcess;
    /// <summary>
    /// The number of volume symbols in the volume symbol string divided by the max volume
    /// </summary>
    public float VolumePercentage => muteText.gameObject.activeSelf ? 0 : VolumeSymbolString.Count(c => c == volumeSymbol) / (float)maxVolume;
    public string VolumeSymbolString
    {
        get => volumeSymbolString;
        set
        {
            volumeSymbolString = value;
            volumeSymbolText.text = volumeSymbolString;
            ModifyMixerVolume();
        }
    }

    private AudioClip GetClueClip() => clueClips[clue];

    private void SkipToClue()
    {
        // resume feature with clue
        PlayMainFeature();
        clueAudioSource.clip = GetClueClip();
        clueAudioSource.Play();
    }

    private void SetInstructionCaption(int instruction)
    {
        if(instruction <= phonographCaptions.Count-1)
        {
            string s = phonographCaptions[instruction];

            captionBox.text = s;
        }
    }

    private void HideCaption()
    {
        captionBox.text = "";
    }

    private void SetClueCaption()
    {
        captionBox.text = clueCaptions[clue];
    }

    private IEnumerator PlayMinigameSequence()
    {
        // start feature
        instructions.Play();
        SetInstructionCaption(0);
        PlayMainFeature();
        yield return new WaitForSeconds(10);
        PauseMainFeature();
        HideCaption();

        // play first commercial break
        StartCommercial();
        yield return new WaitForSeconds(16);
        PauseCommercial();

        //resume feature
        PlayMainFeature();
        yield return new WaitForSeconds(1);
        instructions2.Play();
        SetInstructionCaption(1);
        yield return new WaitForSeconds(15);
        PauseMainFeature();
        HideCaption();

        // play second commercial break
        StartCommercial();
        yield return new WaitForSeconds(29.1f);
        PauseCommercial();

        // resume feature
        PlayMainFeature();
        yield return new WaitForSeconds(1);
        instructions3.Play();
        SetInstructionCaption(2);
        yield return new WaitForSeconds(10);
        PauseMainFeature();
        HideCaption();

        // play third commercial break
        StartCommercial();
        yield return new WaitForSeconds(24.2f);
        PauseCommercial();

        // resume feature with clue
        PlayMainFeature();
        clueAudioSource.clip = GetClueClip();
        clueAudioSource.Play();
        SetClueCaption();
        shouldSkipToClue = true;
        currentHealth = 0;
    }


    private void PauseCommercial()
    {
        isCommercialPlaying = false;
        commercialVideo.Pause();
        commercialRenderTexture.SetActive(false);
    }

    private void StartCommercial()
    {
        isCommercialPlaying = true;
        commercialRenderTexture.SetActive(true);
        commercialVideo.Play();
    }
    private void PlayMainFeature()
    {
        music.Play();
        phonographImage.SetActive(true);
    }
    private void PauseMainFeature()
    {
        music.Pause();
        phonographImage.SetActive(false);
    }

    private void SetHalfVolume()
    {
        // set the volume symbol string to maxVolume / 2 volume symbols followed by maxVolume / 2 no volume symbols
        VolumeSymbolString =
            new string(volumeSymbol, maxVolume / 2) + new string(noVolumeSymbol, maxVolume / 2);
    }
    private void IncreaseVolume()
    {
        // find the first no volume symbol and replace it with a volume symbol
        int index = VolumeSymbolString.IndexOf(noVolumeSymbol);
        if (index >= 0)
        {
            VolumeSymbolString = VolumeSymbolString.Remove(index, 1).Insert(index, volumeSymbol.ToString());
        }
    }

    private void ModifyMixerVolume()
    {
        Debug.Log($"PercentageProperty: {VolumePercentage}");
        // changed to -50 from -80 to make the volume louder
        var adjustedPercentage = (VolumePercentage * 100) - 50;
        adjustedPercentage = adjustedPercentage == -50 ? -80 : adjustedPercentage;
        audioMixer.SetFloat("Volume", adjustedPercentage);

        // mute ends if volume is changed
        muteText.gameObject.SetActive(false);
    }

    private void DecreaseVolume()
    { 
        // find the last volume symbol and replace it with a no volume symbol, as long as there is at least one volume symbol
        int index = VolumeSymbolString.LastIndexOf(volumeSymbol);
        if (index >= 0)
        {
            VolumeSymbolString = VolumeSymbolString.Remove(index, 1).Insert(index, noVolumeSymbol.ToString());
        }
    }

    private void Awake()
    {
        parentChannel = GetComponentInParent<Channel>();
    }

    private void Start()
    {
#if UNITY_EDITOR
        Time.timeScale = testSpeedScale;
        commercialVideo.playbackSpeed = testSpeedScale;
#endif
        if (isFirstTime)
        {
            isFirstTime = false;
            StartCoroutine(SkipFistTime());
        }
    }

    // music doesn't work, I think it's a loading issue.
    private IEnumerator SkipFistTime()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.GoToRandomChannel();
    }

    private void setPostProcessWeight(float weight, float maxTime)
    {
        postProcess.weight = MapValueLog(weight, 0, maxTime, 0, 1);
    }
    public float MapValue(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;
        return mappedValue;
    }

    public static float MapValueLog(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);
        normalizedValue = Mathf.Log10(normalizedValue * 9 + 1);
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;
        return mappedValue;
    }

    float currentHealth;
    private void Update()
    {

        //use commercial view timer to adjust post process weight during commercials
        //use music view timer to adjust post process weight during phonograph

        repeatInputTimer += Time.deltaTime;

        if (repeatInputTimer < repeatInputDelay) return;
        if (volumeUpPressed) 
        { 
            repeatInputTimer = 0;
            IncreaseVolume(); 
        }
        else if (volumeDownPressed) 
        { 
            repeatInputTimer = 0;
            DecreaseVolume(); 
        }
        if (!shouldSkipToClue) {
            if (isCommercialPlaying && VolumePercentage > 0.15f) {
                //commercialViewTimer += Time.deltaTime;
                currentHealth += Time.deltaTime;
                //if (commercialViewTimer > commercialViewTimeThreshold && !playingJumpScare)
                if (currentHealth > commercialViewTimeThreshold && !playingJumpScare) {
                    StopAllCoroutines();
                    StartCoroutine(PlayJumpScareFailureCoroutine());
                }

            } else {
                if (!isCommercialPlaying && VolumePercentage < 0.2f) {
                    //musicMutedTimer += Time.deltaTime;
                    currentHealth += Time.deltaTime;
                    //if (musicMutedTimer > musicMutedTimeThreshold && !playingJumpScare)
                    if (currentHealth > musicMutedTimeThreshold && !playingJumpScare) {
                        StopAllCoroutines();
                        StartCoroutine(PlayJumpScareFailureCoroutine());
                    }
                } else {
                    currentHealth -= Time.deltaTime;
                    if (currentHealth < 0) {
                        currentHealth = 0;
                    }
                }
            }
        }
        setPostProcessWeight(currentHealth, commercialViewTimeThreshold);
        //Debug.Log($"commercialViewTimer: {commercialViewTimer}");
    }

    private IEnumerator PlayJumpScareFailureCoroutine()
    {
        playingJumpScare = true;
        audioMixer.SetFloat("Volume", -80);
        muteText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.6f);
        Debug.Log("Jump scare.");
        jumpScare.SetActive(true);
        yield return new WaitForSeconds(2.1f);
        GameManager.Instance.GoToRandomChannel();
    }

    private void OnVolumeUpPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        volumeUpPressed = true;
    }
    private void OnVolumeUpReleased(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        volumeUpPressed = false;
    }
    private void OnVolumeDownPressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        volumeDownPressed = true;
    }

    private void OnVolumeDownReleased(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        volumeDownPressed = false;
    }

    private void OnEnable()
    {
        clue = int.Parse(PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString());
        bool clueHasChanged = clue != oldClue;
        oldClue = clue;
        if (clueHasChanged)
            shouldSkipToClue = false;

        volumeCanvas.gameObject.SetActive(true);
        SetHalfVolume();
        muteText.gameObject.SetActive(false);
        musicMutedTimer = 0;
        commercialViewTimer = 0;
        playingJumpScare = false;
        muteText.gameObject.SetActive(false);
        jumpScare.SetActive(false);
        commercialRenderTexture.SetActive(false);
        commercialVideo.Stop();
        StopAllCoroutines();

        if (shouldSkipToClue)
            SkipToClue();
        else
            StartCoroutine(PlayMinigameSequence());

        GameManager.WillInterruptBroadcastToChangeToKillingChannel += OnWillChangeToKillingFloor;
        InputManager.InputActions.Gameplay.VolumeUp.performed += OnVolumeUpPressed;
        InputManager.InputActions.Gameplay.VolumeDown.performed += OnVolumeDownPressed;
        InputManager.InputActions.Gameplay.VolumeUp.canceled += OnVolumeUpReleased;
        InputManager.InputActions.Gameplay.VolumeDown.canceled += OnVolumeDownReleased;
        InputManager.InputActions.Gameplay.Mute.performed += OnMutePressed;
        }


    private void OnWillChangeToKillingFloor()
    {
        volumeCanvas.gameObject.SetActive(false );
    }

    private void OnMutePressed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        muteText.gameObject.SetActive(!muteText.gameObject.activeSelf);
        if (muteText.gameObject.activeSelf) 
        {
            audioMixer.SetFloat("Volume", -80);
        }
        else
        {
            ModifyMixerVolume();           
        }
    }

    private void OnDisable()
    {
        volumeCanvas.gameObject.SetActive(false);
        musicMutedTimer = 0;
        currentHealth = 0;
        commercialViewTimer = 0;
        playingJumpScare = false;
        jumpScare.SetActive(false );
        clueAudioSource.Stop();
        music.Stop();
        StopAllCoroutines();
        audioMixer.SetFloat("Volume", 0);
        InputManager.InputActions.Gameplay.VolumeUp.performed -= OnVolumeUpPressed;
        InputManager.InputActions.Gameplay.VolumeDown.performed -= OnVolumeDownPressed;
        InputManager.InputActions.Gameplay.VolumeUp.canceled -= OnVolumeUpReleased;
        InputManager.InputActions.Gameplay.VolumeDown.canceled -= OnVolumeDownReleased;
        GameManager.WillInterruptBroadcastToChangeToKillingChannel -= OnWillChangeToKillingFloor;
    }
}
