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
    private float timeSinceLast = 0f;
    private bool waitingRelease = false;
    public float waitTimeSinceLast = .15f;

    protected override string controlPathInternal {
        get => m_ControlPath;
        set => m_ControlPath = value;
    }

    private void Update()
    {
        if (waitingRelease) {
            timeSinceLast += Time.deltaTime;
            if (timeSinceLast > waitTimeSinceLast) {
                ReleaseInput();
            }
        }
    }

    public void OnMessageArrived(string msg)
    {

        if (waitingRelease && timeSinceLast < waitTimeSinceLast) {
            timeSinceLast = 0f;
            return;
        } 

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
        ReleaseInput();
        foreach (ButtonConnection bCon in currentRemoteSettings.buttonConnections) {
            if (bCon.remoteCommand == message) {
                this.gameObject.SetActive(false);
                controlPath = bCon.input.action.bindings[0].path;
                this.gameObject.SetActive(true);
                SendValueToControl(1.0f);
                waitingRelease = true;
                timeSinceLast = 0f;
                return;
            }
        }

    }

    void ReleaseInput()
    {
        if (controlPath == null) return;
        Debug.Log("release");
        waitingRelease = false;
        timeSinceLast = 0f;
        SendValueToControl(0.0f);
    }

    public void OnConnectionEvent(bool connection)
    {
        if (connection) {
            Debug.Log("Connected " + connection);
            controlPath = currentRemoteSettings.buttonConnections[0].input.action.bindings[0].path;
        }
        
    }

}
