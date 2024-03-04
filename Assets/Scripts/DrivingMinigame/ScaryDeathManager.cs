using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScaryDeathManager : MonoBehaviour
{
    public static ScaryDeathManager instance;

    [SerializeField]
    private GameObject gathering;

    [SerializeField]
    private GameObject slendyScare;

    [SerializeField]
    private GameObject BlackScreen;

    [SerializeField]
    float gathringShowTime;

    [SerializeField]
    float darkTime;

    [SerializeField]
    float ShowSlendyTime;

    [SerializeField] private float randomSuspenseTime;

    [SerializeField]
    float preSequenceTime;

    [SerializeField]
    AudioClip GatheringAudio, SlendyScareAudio;

    AudioSource scareAS;

    public List<TMP_Text> displayTexts;
    private Channel parentChannel;

    private void Awake()
    {
        instance = this;
        parentChannel = GetComponentInParent<Channel>();
    }
    // Start is called before the first frame update
    void Start()
    {
        scareAS = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDeathSequence()
    {
        StartCoroutine(DeathSequence());
    }
    
    IEnumerator DeathSequence()
    {
        yield return new WaitForSeconds(preSequenceTime);
        BlackScreen.SetActive(true);
        EnemySpawner.Instance.DestroyEnemies();
        yield return new WaitForSeconds(darkTime);
        scareAS.PlayOneShot(GatheringAudio);
        BlackScreen.SetActive(false); 
        gathering.SetActive(true);
        yield return new WaitForSeconds(gathringShowTime);
        BlackScreen.SetActive(true);
        gathering.SetActive(false);
        yield return new WaitForSeconds(darkTime + Random.Range(0, randomSuspenseTime));
        scareAS.PlayOneShot(SlendyScareAudio);
        BlackScreen.SetActive(false);
        slendyScare.SetActive(true);
        yield return new WaitForSeconds(ShowSlendyTime);
        ResetChannel();
        PlayerCarController.Instance.StartMessage();
    }

    public void ActivateNumberDisplay()
    {
        string number = PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString();
        foreach (TMP_Text text in displayTexts) {
            text.text = number;
        }
        foreach (TMP_Text text in displayTexts)
        {
            text.enabled = true;
        }
    }

    public void ResetChannel()
    {
        if(PlayerCarController.Instance == null || PlayerCarController.Instance.CompletedMiniGame)
        {
            return;
        }
        StopAllCoroutines();
        EnemySpawner.Instance.DestroyEnemies();
        PlayerCarController.Instance.ResetPlayerCar();
        slendyScare.SetActive(false);
        BlackScreen.SetActive(false);
        gathering.SetActive(false);
        EnemySpawner.Instance.SetupEnemies();
        foreach (TMP_Text text in displayTexts) {
            text.enabled = false;
        }
    }
}
