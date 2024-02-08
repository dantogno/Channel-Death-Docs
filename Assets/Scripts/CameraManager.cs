using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public GameObject MainChannelCamera;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ToggleCamera()
    {
        if(MainChannelCamera == null)
        {
            Debug.LogError("No GameObject Assigned to MainChannelCamera in Camera Manager");
            return;
        }
        if(MainChannelCamera.activeSelf)
        {
            MainChannelCamera.SetActive(false);
        }
        else
        {
            MainChannelCamera.SetActive(true);
        }
    }
}
