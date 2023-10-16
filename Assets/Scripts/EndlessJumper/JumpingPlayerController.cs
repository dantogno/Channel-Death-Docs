using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpingPlayerController : MonoBehaviour
{
    public float jumpStrength;
    private Rigidbody rigid;
    private bool inJump;
    private bool waitForJump;
    private EndlessJumperController ejc;
    private float jumpInputDelay = 0f;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        ejc = GetComponentInParent<EndlessJumperController>();
    }

    private void OnEnable()
    {
        InputManager.InputActions.Gameplay.Jump.performed += Jump;
        InputManager.InputActions.Gameplay.Jump.canceled += ReleaseJump;
    }

    private void Update()
    {
        if (jumpInputDelay > 0f) {
            jumpInputDelay -= Time.deltaTime;
        }
    }

    void Jump(InputAction.CallbackContext context)
    {
        waitForJump = true;
        if (!inJump) {
            rigid.AddForce(Vector3.up * jumpStrength);
            inJump = true;
            jumpInputDelay = .5f;
        }
    }

    void ReleaseJump(InputAction.CallbackContext context)
    {
        waitForJump = false;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("JumpObstacle")) {
            Die();
        }
        else {
            if (jumpInputDelay <= 0) {
                if (waitForJump) {
                    Jump(new InputAction.CallbackContext());
                }
                inJump = false;
            }
        }

    }

    void Die()
    {
        ejc.Die(this.gameObject.transform.position);
    }

    private void OnDisable()
    {
        InputManager.InputActions.Gameplay.Jump.performed -= Jump;
        InputManager.InputActions.Gameplay.Jump.canceled -= ReleaseJump;
        waitForJump = false;
        inJump = false;
    }
}
