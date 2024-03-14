using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Victim;

[Serializable]
public class VictimData
{
    public string Name;
    public VictimState State { get; set; } = VictimState.None;

}
