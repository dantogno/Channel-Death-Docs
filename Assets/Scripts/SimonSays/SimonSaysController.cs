using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimonSaysController : MonoBehaviour
{
    public int sequenceLengthToWin;
    public float inputWindowDuration;
    public Vector2 speedRange;

    public GameObject[] displayScreens;

    public GameObject[] objectHolders;

    private float activeSpeed;
    private List<int> sequence = new List<int>();
    private int playerSequenceCount = 0;
    private bool waitingForInput = false;
    private float inputTimer = 0f;
    private bool infail = false;
    private bool waitingToRun = false;
    private bool won = false;

    public AudioSource[] inputClips;

    public AudioSource source;

    private void Awake()
    {
        GameManager.NewVictimSpawned += NewVictomSpawn;
    }

    private void OnEnable()
    {
        if (won) {
            foreach (GameObject go in displayScreens) {
                go.SetActive(false);
            }
            displayScreens[3].SetActive(true);
            foreach (GameObject go in objectHolders) {
                go.SetActive(true);
            }
            return;
        }
        InputManager.InputActions.Gameplay.Input2.started += InputPressed;
        InputManager.InputActions.Gameplay.Input4.started += InputPressed;
        InputManager.InputActions.Gameplay.Input6.started += InputPressed;
        InputManager.InputActions.Gameplay.Input8.started += InputPressed;
        InputManager.InputActions.Gameplay.Input2.canceled += InputReleased;
        InputManager.InputActions.Gameplay.Input4.canceled += InputReleased;
        InputManager.InputActions.Gameplay.Input6.canceled += InputReleased;
        InputManager.InputActions.Gameplay.Input8.canceled += InputReleased;
        foreach (GameObject go in objectHolders) {
            go.SetActive(false);
        }
        StartGame();
    }

    private void Update()
    {
        if (waitingForInput) {
            if (inputTimer > 0) {
                inputTimer -= Time.deltaTime;
            }
            else {
                if (!infail) {
                   StartCoroutine(FailGame());
                }
            }
        }
    }

    void InputPressed(InputAction.CallbackContext context)
    {
        if (!waitingForInput) return; 
        int sequenceInput = 0;
        if (context.action == InputManager.InputActions.Gameplay.Input2) {
            objectHolders[0].SetActive(true);
        }
        if (context.action == InputManager.InputActions.Gameplay.Input4) {
            objectHolders[1].SetActive(true);
            sequenceInput = 1;
        }
        if (context.action == InputManager.InputActions.Gameplay.Input6) {
            objectHolders[2].SetActive(true);
            sequenceInput = 2;
        }
        if (context.action == InputManager.InputActions.Gameplay.Input8) {
            objectHolders[3].SetActive(true);
            sequenceInput = 3;
        }
        inputClips[sequenceInput].Play();
        if (sequence[playerSequenceCount] == sequenceInput) {
            playerSequenceCount++;
            inputTimer = inputWindowDuration;
            if (playerSequenceCount >= sequence.Count) {
                waitingForInput = false;
                waitingToRun = true;
                playerSequenceCount = 0;
            }
        }
        else {
            StartCoroutine(FailGame());
        }
    }

    void InputReleased(InputAction.CallbackContext context)
    {
        Debug.Log("released");
        if (context.action == InputManager.InputActions.Gameplay.Input2) {
            objectHolders[0].SetActive(false);
        }
        if (context.action == InputManager.InputActions.Gameplay.Input4) {
            objectHolders[1].SetActive(false);
        }
        if (context.action == InputManager.InputActions.Gameplay.Input6) {
            objectHolders[2].SetActive(false);
        }
        if (context.action == InputManager.InputActions.Gameplay.Input8) {
            objectHolders[3].SetActive(false);
        }
        if (waitingToRun) {
            waitingToRun = false;
            if (sequence.Count >= sequenceLengthToWin) {
                WinGame();
            }
            else {
                StartCoroutine(RunSequence());
            }
        }
    }


    void StartGame()
    {
        Debug.Log("StartGame");
        sequence.Clear();
        StartCoroutine(RunSequence());
    }

    IEnumerator RunSequence()
    {

        displayScreens[0].SetActive(true);
        displayScreens[1].SetActive(false);
        displayScreens[2].SetActive(false);
        displayScreens[3].SetActive(false);
        sequence.Add(Random.Range(0, 4));
        int sequenceCount = 0;
        activeSpeed = Mathf.Lerp(speedRange.x, speedRange.y, (float)sequence.Count / (float)sequenceLengthToWin);
        yield return new WaitForSeconds(.5f);
        while (sequenceCount < sequence.Count) {
            objectHolders[sequence[sequenceCount]].SetActive(true);
            inputClips[sequence[sequenceCount]].Play();
            float elapsedTime = 0f;
            while (elapsedTime < activeSpeed) {
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            objectHolders[sequence[sequenceCount]].SetActive(false);
            yield return new WaitForSeconds(.1f);
            sequenceCount++;
        }
        StartInput();
    }

    void StartInput()
    {
        waitingForInput = true;
        displayScreens[0].SetActive(false);
        displayScreens[1].SetActive(true);
        inputTimer = inputWindowDuration;
    }

    IEnumerator FailGame()
    {
        infail = true;
        waitingForInput = false;
        displayScreens[2].SetActive(true);
        displayScreens[0].SetActive(false);
        displayScreens[1].SetActive(false);
        playerSequenceCount = 0;
        inputClips[4].Play();
        yield return new WaitForSeconds(3f);
        foreach (GameObject go in objectHolders) {
            go.SetActive(false);
        }
        StartGame();
        infail = false;
    }

    void WinGame()
    {
        foreach (GameObject go in displayScreens) {
            go.SetActive(false);
        }
        foreach (GameObject go in objectHolders) {
            go.SetActive(true);
        }
        displayScreens[3].GetComponent<TMPro.TMP_Text>().text = PasscodeManager.Instance.HeartsNumber;
        displayScreens[3].SetActive(true);
        won = true;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        foreach (GameObject go in objectHolders) {
            go.SetActive(false);
        }
        displayScreens[0].SetActive(true);
        displayScreens[1].SetActive(false);
        displayScreens[2].SetActive(false);
        displayScreens[3].SetActive(false);
        playerSequenceCount = 0;
        infail = false;
        waitingForInput = false;
        waitingToRun = false;
        InputManager.InputActions.Gameplay.Input2.performed -= InputPressed;
        InputManager.InputActions.Gameplay.Input4.performed -= InputPressed;
        InputManager.InputActions.Gameplay.Input6.performed -= InputPressed;
        InputManager.InputActions.Gameplay.Input8.performed -= InputPressed;
        InputManager.InputActions.Gameplay.Input2.canceled -= InputReleased;
        InputManager.InputActions.Gameplay.Input4.canceled -= InputReleased;
        InputManager.InputActions.Gameplay.Input6.canceled -= InputReleased;
        InputManager.InputActions.Gameplay.Input8.canceled -= InputReleased;
    }

    void NewVictomSpawn()
    {
        won = false;
    }

    private void OnDestroy()
    {
        GameManager.NewVictimSpawned -= NewVictomSpawn;
    }
}
