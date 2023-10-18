using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsChannel : MonoBehaviour
{
    public float initialDelay = 4; 
    public float scrollSpeed = 2;
    public float restartDelay = 30;

    private float timer = 0;
    void Update()
    {
        if (timer > initialDelay)
        {
            // move up based on speed
            transform.position += Vector3.up * scrollSpeed * Time.deltaTime;
        }
        else if (timer > restartDelay)
        {
            // restart the game
            SceneManager.LoadScene(0);
        }

         timer += Time.deltaTime;
    }
}
