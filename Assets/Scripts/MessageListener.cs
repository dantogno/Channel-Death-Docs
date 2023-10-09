using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.OnScreen;

public class MessageListener : OnScreenControl
{
    public RemoteSettings currentRemoteSettings;
    public InputActionAsset inputs;

    public bool commandDebug = false;

    public string lastCommand;

    private string m_ControlPath;

    protected override string controlPathInternal {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    public void OnMessageArrived(string msg)
    {
        if (msg.Contains("Repeat")) return;

        string[] messages = msg.Split(" ");

        foreach (string message in messages) {
            if (message.Contains("Command=")) {
                string tempMes = message.Substring(8);
                if (commandDebug) Debug.Log(tempMes);
                lastCommand = tempMes;
                TriggerInput(tempMes);
                return;
            }
        } 
    }

    void TriggerInput(string message)
    {

        foreach (ButtonConnection bCon in currentRemoteSettings.buttonConnections) {
            if (bCon.remoteCommand == message) {
                this.gameObject.SetActive(false);
                controlPath = bCon.input.action.bindings[0].path;
                this.gameObject.SetActive(true);
                SendValueToControl(1.0f);
                SendValueToControl(0.0f);
            }
        }

    }

    public void OnConnectionEvent(bool connection)
    {
        Debug.Log("Connected " + connection);
        
    }

}
