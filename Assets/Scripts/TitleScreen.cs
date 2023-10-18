using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public bool fullScreen = true;
   // public float minimumTimeOnTitleScreen = 3f;
    //float timer;
    
    // Start is called before the first frame update
    void Start()
    {
  
        // set a full screen 4:3 resolution
        Screen.SetResolution(1024, 768, fullScreen);
        
        StartCoroutine(LoadMainScene());
    }
    private void Update()
    {
        //timer += Time.deltaTime;
    }
    private IEnumerator LoadMainScene()
    {
        // load main scene async
        // wait until it's done loading
        yield return new WaitForSeconds(3.25f);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
