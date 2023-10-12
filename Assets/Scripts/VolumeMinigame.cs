using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Video;
using static UnityEngine.Rendering.DebugUI;

public class VolumeMinigame : MonoBehaviour
{
    public float testSpeedScale = 1;
    public float commercialViewTimeThreshold = 8;
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
    private float repeatInputDelay = 0.1f;
    private float repeatInputTimer = 0;
    private bool isCommercialPlaying = false;
    private float commercialViewTimer = 0;
    private bool playingJumpScare = false;

    /// <summary>
    /// Must set this for each new victim!
    /// </summary>
    private int Clue { get; set; } = 3;


    /// <summary>
    /// The number of volume symbols in the volume symbol string divided by the max volume
    /// </summary>
    public float VolumePercentage => VolumeSymbolString.Count(c => c == volumeSymbol) / (float)maxVolume;
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

    private AudioClip GetClueClip() => clueClips[Clue];

    private IEnumerator PlayMinigameSequence()
    {
        // start feature
        instructions.Play();
        PlayMainFeature();
        yield return new WaitForSeconds(10);
        PauseMainFeature();

        // play first commercial break
        StartCommercial();
        yield return new WaitForSeconds(16);
        PauseCommercial();

        //resume feature
        PlayMainFeature();
        yield return new WaitForSeconds(1);
        instructions2.Play();
        yield return new WaitForSeconds(15);
        PauseMainFeature();

        // play second commercial break
        StartCommercial();
        yield return new WaitForSeconds(29.1f);
        PauseCommercial();

        // resume feature
        PlayMainFeature();
        yield return new WaitForSeconds(1);
        instructions3.Play();
        yield return new WaitForSeconds(10);
        PauseMainFeature();

        // play third commercial break
        StartCommercial();
        yield return new WaitForSeconds(24.2f);
        PauseCommercial();

        // resume feature with clue
        PlayMainFeature();
        clueAudioSource.clip = GetClueClip();
        clueAudioSource.Play();
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

    private void Start()
    {
#if UNITY_EDITOR
        Time.timeScale = testSpeedScale;
        commercialVideo.playbackSpeed = testSpeedScale;
#endif
        SetHalfVolume();
        muteText.gameObject.SetActive(false);
    }

    private void Update()
    {
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

        if (isCommercialPlaying && VolumePercentage > 0.1f)
        {
            commercialViewTimer += Time.deltaTime;
            if (commercialViewTimer > commercialViewTimeThreshold && !playingJumpScare)
            {
                StopAllCoroutines();
                StartCoroutine(PlayJumpScareFailureCoroutine());
            }
        }
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
        commercialViewTimer = 0;
        playingJumpScare = false;
        muteText.gameObject.SetActive(false);
        jumpScare.SetActive(false);
        commercialRenderTexture.SetActive(false);
        commercialVideo.Stop();
        StopAllCoroutines();
        StartCoroutine(PlayMinigameSequence());
        InputManager.InputActions.Gameplay.VolumeUp.performed += OnVolumeUpPressed;
        InputManager.InputActions.Gameplay.VolumeDown.performed += OnVolumeDownPressed;
        InputManager.InputActions.Gameplay.VolumeUp.canceled += OnVolumeUpReleased;
        InputManager.InputActions.Gameplay.VolumeDown.canceled += OnVolumeDownReleased;
        InputManager.InputActions.Gameplay.Mute.performed += OnMutePressed;
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
    }
}
