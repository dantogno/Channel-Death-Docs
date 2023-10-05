using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Put channel contents inside container child object. Do not disable the root channel obj. 
/// Disabling container is fine.
/// </summary>
public class Channel : MonoBehaviour
{
    [Tooltip("Channel number displayed to user and used to sort channels.")]
    public int ChannelNumber;

    [Tooltip("Invoked when channel is entered.")]
    public UnityEvent ChannelEntered;

    [Tooltip("Invoked when channel is entered.")]
    public UnityEvent ChannelExited;

}
