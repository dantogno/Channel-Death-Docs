using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Playables;

public class JumpScare : MonoBehaviour
{
    [SerializeField]
    GameObject jumpScareObj;

    [SerializeField]
    GameObject Crawl;

    [SerializeField]
    float crawlSpeed;

    [SerializeField]
    AudioClip JumpScareSFX;

    [SerializeField]
    GameObject canvasObj;

    [SerializeField]
    int triggerDistance;

    AudioSource jumpScareSource;

    bool JumpScareTriggered;

    [SerializeField]
    float scareTime, crawlScareDelay, jumpScareVol;

    // Start is called before the first frame update
    void Start()
    {
        if(Crawl != null)
        {
            initialCrawlPos = Crawl.transform.position;
        }
        canvasObj.SetActive(false);
        GameObject g = new GameObject();
        g.name = "scareSFX";
        g.transform.position = transform.position;
        g.transform.parent = transform;
        jumpScareSource = g.AddComponent<AudioSource>();
        jumpScareSource.playOnAwake = false;
        jumpScareSource.loop = false;
        jumpScareSource.clip = JumpScareSFX;
        jumpScareSource.volume = jumpScareVol;
        jumpScareObj.SetActive(false);
    }

    Vector3 initialCrawlPos;
    Vector3 crawlDirection;
    bool crawling;
    // Update is called once per frame
    void Update()
    {
        if (!JumpScareTriggered)
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, triggerDistance))
            {
                if (hit.collider.gameObject.tag == "Goal")
                {
                    JumpScareTriggered = true;
                    StartCoroutine(JumpAndScare());
                }
                
            }
        }
        if (crawling)
        {
            Crawl.SetActive(true);
            crawlDirection = Vector3.right;
            crawlDirection.Normalize();
            crawlDirection.y = 0;
            Crawl.transform.position += crawlDirection * crawlSpeed * Time.deltaTime;
        }
    }

    IEnumerator JumpAndScare()
    {
        crawling = true;
        Crawl.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(crawlScareDelay);
        MusicPlayer.instance.DuckMusicVolume();
        Crawl.GetComponent<AudioSource>().Stop();
        //audioSource.PlayOneShot(JumpScareSFX, 1);
        jumpScareSource.Play();
        jumpScareObj.SetActive(true);
        yield return new WaitForSeconds(scareTime);
        jumpScareObj.SetActive(false);
        canvasObj.SetActive(true);
    }
}
