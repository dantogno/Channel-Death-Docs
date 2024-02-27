using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Put channel contents inside container child object. Do not disable the root channel obj. 
/// Disabling container is fine.
/// </summary>
public class Channel : MonoBehaviour
{
    [Tooltip("Channel number displayed to user and used to sort channels.")]
    public int ChannelNumber;

    [Tooltip("Image object in channel that display suit associated with minigame")]
    public Image suitImage;

    [Tooltip("Invoked when channel is entered.")]
    public UnityEvent ChannelEntered;

    [Tooltip("Invoked when channel is entered.")]
    public UnityEvent ChannelExited;

    private List<Renderer> renderers;
    //[HideInInspector]
    public SuitEnum currentSuit;

    private void Start()
    {
        renderers = new List<Renderer>(GetComponentsInChildren<Renderer>(true));
    }

    public void EnableAllRenderers(bool shouldEnable)
    {
        if (renderers == null) { return; }  
        // disable or enable all renderers based on shouldEnable
        foreach (var renderer in renderers)
        {
            renderer.enabled = shouldEnable;
        }
    }

    public void SetSuit(Sprite sprite, int suit)
    {
        currentSuit = (SuitEnum)suit;
        if (suitImage != null) suitImage.sprite = sprite;
    }
}
