using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MazePlayerController : MonoBehaviour
{
    public static MazePlayerController Instance { get; private set; }
    enum PlayerState { idle, walking, turning}

    PlayerState playerState;

    public Camera playerCamera;

    public float Speed;

    float currentFov;
    public float baseFov;
    public float walkFov;
    public float zoomSpeed;

    ArmVisuals armVisuals;
    public GameObject arms;

    [SerializeField]
    private float turnSpeed;
    Quaternion targetQuaternion;
    float targetRotation;

    Vector2 currentCell;
    Vector2 targetCell;

    Vector3 startPosition;
    Vector3 targetPosition;
    private float elapsedTime;

    bool HoldingWalkInput;

    private void Awake()
    {
        Instance = this;
        playerCamera.backgroundColor = Color.black;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentFov = baseFov;
        armVisuals = arms.GetComponent<ArmVisuals>();
    }

    public void InitializePlayerLocation()
    {
        Vector3 startPos = MazeGenerator.Instance.MazeGrid[0, 0].transform.position;
        transform.position.Set(startPos.x, transform.position.y, startPos.z);
        currentCell = new Vector2(0, 0);
    }

    private Vector2 getNextCell()
    {
        if(transform.forward == Vector3.forward)
        {
            return new Vector2(currentCell.x, currentCell.y + 1);
        }
        if(transform.forward == Vector3.back)
        {
            return new Vector2(currentCell.x, currentCell.y - 1);
        }
        if(transform.forward == Vector3.right)
        {
            return new Vector2(currentCell.x + 1, currentCell.y);
        }
        if(transform.forward == Vector3.left)
        {
            return new Vector2(currentCell.x - 1, currentCell.y);
        }
        return currentCell;
    }

    private bool cellIsValid(Vector2 target)
    {
        if(target == currentCell) return false;
        if (target.y >= MazeGenerator.Instance.mazeDepth){ return false; }
        if(target.y < 0){ return false; }
        if(target.x >= MazeGenerator.Instance.mazeWidth){ return false; }
        if(target.x < 0) { return false; }
        return true;
    }

    private void OnEnable()
    {
        currentFov = baseFov;
        InputManager.InputActions.Gameplay.Input2.performed += Walk;
        InputManager.InputActions.Gameplay.Input2.canceled += StopWalking;
        //InputManager.InputActions.Gameplay.Input8.canceled += StopWalking;
        InputManager.InputActions.Gameplay.Input4.performed += TurnLeft;
        InputManager.InputActions.Gameplay.Input6.performed += TurnRight;
        InputManager.InputActions.Gameplay.Input8.performed += WalkBackwards;
        //change the active camera to the first person camera
        if(CameraManager.instance != null)
        {
            CameraManager.instance.ToggleCamera();
        }
    }

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.Input2.canceled -= StopWalking;
        //InputManager.InputActions.Gameplay.Input8.canceled -= StopWalking;
        InputManager.InputActions.Gameplay.Input2.performed -= Walk;
        InputManager.InputActions.Gameplay.Input4.performed -= TurnLeft;
        InputManager.InputActions.Gameplay.Input6.performed -= TurnRight;
        InputManager.InputActions.Gameplay.Input8.performed -= WalkBackwards;
        //change the active camera back to the main channel camera
        if (CameraManager.instance != null)
        {
            CameraManager.instance.ToggleCamera();
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerState)
        {
            case PlayerState.idle:
                break;
            case PlayerState.walking:
                elapsedTime += Time.deltaTime;
                float percentComplete = elapsedTime / Speed;
                transform.position = Vector3.Lerp(startPosition, targetPosition, percentComplete);
                if(currentFov < walkFov)
                {
                    currentFov += zoomSpeed;
                }
                if(currentFov > walkFov)
                {
                    currentFov = walkFov;
                }
                armVisuals.bobArms();
                if (Vector3.Distance(transform.position, targetPosition) < 0.0099f)
                {
                    transform.position = targetPosition;
                    currentCell = targetCell;
                    if (HoldingWalkInput)
                    {
                        Move();
                    }
                    else
                    {
                        playerState = PlayerState.idle;
                    }
                }
                break;
            case PlayerState.turning:
                transform.rotation = Quaternion.Lerp(transform.rotation, targetQuaternion, turnSpeed * Time.deltaTime);
                if (Quaternion.Dot(transform.rotation, targetQuaternion) > 0.9999f)
                {
                    playerState = PlayerState.idle;
                    transform.rotation = targetQuaternion;
                }
                break;
            default:
                break;
        }
        if(playerState != PlayerState.walking)
        {
            if (currentFov > baseFov)
            {
                currentFov -= zoomSpeed;
            }
            if (currentFov < baseFov)
            {
                currentFov = baseFov;
            }
        }
        playerCamera.fieldOfView = currentFov;
    }

    void Walk(InputAction.CallbackContext context)
    {
        HoldingWalkInput = true;
        if(playerState == PlayerState.idle)
        {
            Move();
        }
    }

    private void Move()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 2))
        {
            playerState = PlayerState.idle;
            return;
        }
        else
        {
            targetCell = getNextCell();
            if (cellIsValid(targetCell))
            {
                elapsedTime = 0;
                startPosition = transform.position;
                Vector3 nextTilePos = MazeGenerator.Instance.MazeGrid[(int)targetCell.x, (int)targetCell.y].transform.position;
                targetPosition = new Vector3(nextTilePos.x, transform.position.y, nextTilePos.z);
                playerState = PlayerState.walking;
            }
            else
            {
                playerState = PlayerState.idle;
            }
        }
    }

    void WalkBackwards(InputAction.CallbackContext context)
    {
      
    }

    void StopWalking(InputAction.CallbackContext context)
    {
        HoldingWalkInput = false;
    }

    
    void TurnRight(InputAction.CallbackContext context)
    {
        if(playerState != PlayerState.idle)
        {
            return;
        }
        targetRotation += 90;
        targetQuaternion = Quaternion.AngleAxis(targetRotation, transform.up);
        playerState = PlayerState.turning;
    }

    void TurnLeft(InputAction.CallbackContext context)
    {
        if(playerState != PlayerState.idle) { return; }
        targetRotation -= 90;
        targetQuaternion = Quaternion.AngleAxis(targetRotation, transform.up);
        playerState = PlayerState.turning;
    }

}