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
        if (offSound != null) {
            offSound.Play();
        }
        if (black != null) {
            black.SetActive(true);
        }
        if (tone != null) {
            tone.Stop();
        }
        if (video != null) {
            video.SetActive(false);
        }
        yield return new WaitForSeconds(delaytillReload);
        SaveSystem.CreateNewGame();
        SceneManager.LoadScene("TitleScene");
    }
}
