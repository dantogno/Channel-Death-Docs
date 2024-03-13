using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameSequence : MonoBehaviour
{
    public AudioSource offSound;
    public AudioSource tone;
    public GameObject video;
    public GameObject black;
    public float delaytillOffVideo;
    public float delaytillReload;

    private void Start()
    {
        StartCoroutine(RestartDelay());
    }



    IEnumerator RestartDelay()
    {
        yield return new WaitForSeconds(delaytillOffVideo);
        offSound.Play();
        black.SetActive(true);
        tone.Stop();
        video.SetActive(false);
        yield return new WaitForSeconds(delaytillReload);
        SceneManager.LoadScene("TitleScene");
    }
}
