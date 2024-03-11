using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClueFollowUpLines", menuName = "Scriptable Objects/Create ClueFollowUpLines")]
public class ClueFollowUpLines : ScriptableObject
{
    [Tooltip("Sequence starts with detective speaking one of these lines.")]
    public string[] DetectiveIntroLines;

    [Tooltip("Lines that come after the rescued victim describes the clue. Need to make sense with any clue.")]
    public string[] VictimFollowUpLines;

}
