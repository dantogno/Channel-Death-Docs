using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

[System.Serializable]
public class ButtonConnection
{
    [HideInInspector]
    public string name;
    public InputActionReference input;
    public string remoteCommand;

}

[CreateAssetMenu( menuName = "RemoteSetting")]
public class RemoteSettings : ScriptableObject
{
    public List<ButtonConnection> buttonConnections;

    private void OnValidate()
    {
        foreach(ButtonConnection bCon in buttonConnections) {
            if (bCon.input != null) {
                bCon.name = bCon.input.name;
            }
            else {
                bCon.name = "Missing Input";
            }
        }
    }
}
