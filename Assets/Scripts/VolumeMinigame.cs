using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class VolumeMinigame : MonoBehaviour
{
    public TMP_Text volumeSymbolText;
    public TMP_Text muteText;
    public AudioMixer audioMixer;
    private string volumeSymbolString = string.Empty;
    private const int maxVolume = 20;
    private const char noVolumeSymbol = '.';
    private const char volumeSymbol = '|';
    private bool volumeUpPressed, volumeDownPressed= false;
    private float repeatInputDelay = 0.1f;
    private float repeatInputTimer = 0;

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
        InputManager.InputActions.Gameplay.VolumeUp.performed -= OnVolumeUpPressed;
        InputManager.InputActions.Gameplay.VolumeDown.performed -= OnVolumeDownPressed;
        InputManager.InputActions.Gameplay.VolumeUp.canceled -= OnVolumeUpReleased;
        InputManager.InputActions.Gameplay.VolumeDown.canceled -= OnVolumeDownReleased;
    }
}
