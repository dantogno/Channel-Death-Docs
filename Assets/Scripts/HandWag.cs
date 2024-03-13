using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandWag : MonoBehaviour
{
    [SerializeField]
    private float delayBetweenWags = 1.0f;

    [SerializeField]
    private float rotationAmount = 30.0f;

    private float timer = 0;
    private bool reverse = false;

    // Update is called once per frame
    void Update()
    {
        // increment the timer
        timer += Time.deltaTime;
        // if the timer is greater than the delay
        if (timer > delayBetweenWags)
        {
            // reset the timer
            timer = 0;
            // rotate the hand in direction based on reverse
            if (reverse)
            {
                transform.Rotate(Vector3.forward, -rotationAmount);
            }
            else
            {
                transform.Rotate(Vector3.forward, rotationAmount);
            }
            // reverse the direction
            reverse = !reverse;
        }
    }
}
