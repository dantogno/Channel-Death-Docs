using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class JumpScare : MonoBehaviour
{
    [SerializeField]
    GameObject jumpScareObj;

    [SerializeField]
    AudioClip JumpScareSFX;

    [SerializeField]
    GameObject canvasObj;

    AudioSource audioSource;

    bool JumpScareTriggered;

    [SerializeField]
    float scareTime;
    // Start is called before the first frame update
    void Start()
    {
        canvasObj.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        jumpScareObj.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!JumpScareTriggered)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 6))
            {
                if (hit.collider.gameObject.tag == "Goal")
                {
                    JumpScareTriggered = true;
                    canvasObj.SetActive(true);
                    MusicPlayer.instance.DuckMusicVolume();
                    audioSource.PlayOneShot(JumpScareSFX);
                    StartCoroutine(JumpAndScare());
                }
            }
        }
    }

    IEnumerator JumpAndScare()
    {
        jumpScareObj.SetActive(true);
        yield return new WaitForSeconds(scareTime);
        jumpScareObj.SetActive(false);
    }
}
