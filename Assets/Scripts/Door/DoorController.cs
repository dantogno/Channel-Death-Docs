using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class DoorController : MonoBehaviour
{
    [SerializeField]
    List<Door> doors;

    int currentDoor;

    [SerializeField]
    float moveSpeed;

    [SerializeField]
    float moveDistance;

    float initialZ;

    [SerializeField]
    GameObject bloodMovement;

    bool waitingForPlay;

    [SerializeField]
    float footstepPlayRate;

    [SerializeField]
    List<AudioClip> stepSounds, creakSounds;

    [SerializeField]
    Channel parentChannel;

    [SerializeField]
    TextMeshProUGUI numText;

    AudioSource footstepSource;
    AudioSource creakSource;
    enum ControllerState { idle, walking}
    ControllerState state;
    // Start is called before the first frame update
    void Start()
    {
        initialZ = transform.position.z;
        footstepSource = this.gameObject.AddComponent<AudioSource>();
        GameObject g = new GameObject();
        g.transform.position = transform.position;
        g.transform.parent = transform;
        creakSource = g.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.Enter.performed += EnterPressed;
    }

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.Enter.performed -= EnterPressed;
    }

    private void EnterPressed(InputAction.CallbackContext context)
    {
        if(currentDoor < doors.Count)
        {
            doors[currentDoor].OpenDoor();
            if(currentDoor <= doors.Count - 2)
            {
                state = ControllerState.walking;
            }
            if(currentDoor == doors.Count - 2)
            {
                bloodMovement.SetActive(true);
                numText.text = PasscodeManager.Instance.Passcode[(int)parentChannel.currentSuit].ToString();
            }
        }
    }
   
    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            case ControllerState.idle:
                break;
            case ControllerState.walking:
                if (!waitingForPlay)
                {
                    PlayFootstepSound(footstepSource, stepSounds, 10, 0.8f);
                    PlayFootstepSound(creakSource, creakSounds, 10, 0.17f);
                    StartCoroutine(PlayFootstep());
                }
                transform.position += Vector3.forward * moveSpeed * Time.deltaTime;
                if (transform.position.z >= initialZ + moveDistance)
                {
                    destinationReached();
                }
                break;
        }
    }

    private void PlayFootstepSound(AudioSource s, List<AudioClip> c, int pitchVar, float volume)
    {
        s.volume = volume;
        s.clip = getRandomSound(c);
        s.pitch = 1 + (Random.Range(-pitchVar, pitchVar) * 0.01f);
        s.Play();
    }

    private void destinationReached()
    {
        state = ControllerState.idle;
        initialZ = transform.position.z;
        currentDoor++;
    }

    IEnumerator PlayFootstep()
    {
        waitingForPlay = true;
        yield return new WaitForSeconds(footstepPlayRate);
        waitingForPlay = false;
    }

    AudioClip getRandomSound(List<AudioClip> clips)
    {
        return clips[Random.Range(0, clips.Count - 1)];
    }
}
