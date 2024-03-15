using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCarController : MonoBehaviour
{
    public static PlayerCarController Instance;

    [SerializeField]
    GameObject Message;
    [SerializeField]
    float messageDisplayLength;

    [HideInInspector]
    public bool CanMove;

    private float currentSpeed;
    [SerializeField]
    private float MaxSpeed;
    [SerializeField]
    private float Acceleration;
    [SerializeField]
    private float MaxTurnSpeed;
    private float turnSpeed;
    [SerializeField]
    private float turnAcceleration;
    [SerializeField]
    private float wheelSpeed;
    [SerializeField]
    private GameObject SteeringWheel;

    [SerializeField]
    float xBound;

    [SerializeField]
    private AudioSource EngineSource;

    [SerializeField]
    private float AudioFadeSpeed;

    Vector3 direction;

    enum TurnState { idle, right, left}
    TurnState turningState;

    Quaternion TargetAngleRight = Quaternion.Euler(0,90,-90);
    Quaternion TargetAngleLeft = Quaternion.Euler(-180, 90, -90);
    Quaternion TargetAngleIdle = Quaternion.Euler(-90, 90, -90);
    Quaternion currentAngle;

    private void Awake()
    {
        Instance = this;
    }

    Vector3 StartPosition;

    [SerializeField]
    AudioClip CarStopSound;

    public bool CompletedMiniGame;
    void Start()
    {
        StartPosition = transform.position;
        direction = transform.forward;
        StartCoroutine(WaitForMessage());
        CanMove = true;
    }

    IEnumerator WaitForMessage()
    {
        Message.SetActive(true);
        yield return new WaitForSeconds(messageDisplayLength);
        Message.SetActive(false);
        //CanMove = true;
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.Input4.performed += SteerLeft;
        InputManager.InputActions.Gameplay.Input4.canceled += StopSteerLeft;
        InputManager.InputActions.Gameplay.Input6.performed += SteerRight;
        InputManager.InputActions.Gameplay.Input6.canceled += StopSteerRight;
        InputManager.InputActions.Gameplay.UpPlusR.performed += SteerRight;
        InputManager.InputActions.Gameplay.UpPlusR.canceled += StopSteerRight;
        InputManager.InputActions.Gameplay.DownMinusL.performed += SteerLeft;
        InputManager.InputActions.Gameplay.DownMinusL.canceled += StopSteerLeft;
        StartMessage();
    }

    public void StartMessage()
    {
        StartCoroutine(WaitForMessage());
    }

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.Input4.performed -= SteerLeft;
        InputManager.InputActions.Gameplay.Input4.canceled -= StopSteerLeft;
        InputManager.InputActions.Gameplay.Input6.performed -= SteerRight;
        InputManager.InputActions.Gameplay.Input6.canceled -= StopSteerRight;
        InputManager.InputActions.Gameplay.UpPlusR.performed -= SteerRight;
        InputManager.InputActions.Gameplay.UpPlusR.canceled -= StopSteerRight;
        InputManager.InputActions.Gameplay.DownMinusL.performed -= SteerLeft;
        InputManager.InputActions.Gameplay.DownMinusL.canceled -= StopSteerLeft;
    }

    void SteerLeft(InputAction.CallbackContext context)
    {
        turningState = TurnState.left;
        
    }

    void StopSteerLeft(InputAction.CallbackContext context)
    {
        if(turningState == TurnState.left)
        {
            turningState = TurnState.idle;
        }
    }

    void SteerRight(InputAction.CallbackContext context)
    {
        turningState = TurnState.right;
        
    }

    void StopSteerRight(InputAction.CallbackContext context)
    {
        if(turningState == TurnState.right)
        {
            turningState = TurnState.idle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            FadeInEngineAudio();
            MoveForward();
            TurnUpdate();
        }
        else
        {
            FadeOutEngineAudio();
        }
        updateAudio();
    }

    void MoveForward()
    {
       if(currentSpeed < MaxSpeed)
       {
            Accelerate();
       }
       
       transform.position += direction * currentSpeed * Time.deltaTime;
        if (transform.position.x < -xBound)
        {
            transform.position = new Vector3(-xBound, transform.position.y, transform.position.z);
        }
        if (transform.position.x > xBound)
        {
            transform.position = new Vector3(xBound, transform.position.y, transform.position.z);
        }
    }

    void Accelerate()
    {
        if (currentSpeed < MaxSpeed)
        {
            currentSpeed += Acceleration;
        }
        else
        {
            currentSpeed = MaxSpeed;
        }
    }

    void TurnUpdate()
    {
        switch (turningState)
        {
            case TurnState.idle:
                currentAngle = TargetAngleIdle;
                direction.x = 0;
                turnSpeed = 0;
                break;
            case TurnState.right:
                currentAngle = TargetAngleRight;
                if (direction.x < MaxTurnSpeed)
                {
                    turnSpeed += turnAcceleration;
                    direction += (Vector3.right * turnSpeed);
                }
                break;
            case TurnState.left:
                currentAngle = TargetAngleLeft;
                if(direction.x > -MaxTurnSpeed)
                {
                    turnSpeed += turnAcceleration;
                    direction += (Vector3.left * turnSpeed);
                }
                break;

        }
        SteeringWheel.transform.rotation = Quaternion.Slerp(SteeringWheel.transform.rotation, currentAngle, wheelSpeed);
    }

    void updateAudio()
    {
        EngineSource.pitch = Remap(currentSpeed, 0, MaxSpeed);
    }
    float Remap(float value, float min, float max)
    {
        return 0.5f + (value - min) * (0.5f / (max - min));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Creature")
        {
            CanMove = false;
            if(other.gameObject.TryGetComponent<CreatureScript>(out CreatureScript creature))
            {
                creature.OnCreatureHit();
                ScaryDeathManager.instance.StartDeathSequence();
                EngineSource.PlayOneShot(CarStopSound);
            }
            Debug.Log("Hit a creature!");
        }
        if(other.gameObject.tag == "Goal")
        {
            CanMove = false;
            EngineSource.PlayOneShot(CarStopSound);
            CompletedMiniGame = true;
            ScaryDeathManager.instance.ActivateNumberDisplay();
        }
    }

    void FadeOutEngineAudio()
    {
        if(EngineSource.volume > 0)
        {
            EngineSource.volume -= AudioFadeSpeed;
        }
        else if(EngineSource.volume < 0)
        {
            EngineSource.volume = 0;
        }
    }

    void FadeInEngineAudio()
    {
        if (EngineSource.volume < 1)
        {
            EngineSource.volume += AudioFadeSpeed;
        }
        else if (EngineSource.volume > 1)
        {
            EngineSource.volume = 1;
        }
    }

    public void ResetPlayerCar()
    {
        transform.position = StartPosition;
        direction = transform.forward;
        currentSpeed = 0;
        CanMove = true;
    }
}
