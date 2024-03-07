using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongRotation : MonoBehaviour
{
    [SerializeField] private float rotationTime = 1.0f;
    [SerializeField] private Vector3 rotationAmount = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        // use leantween to pingpong from the current rotation to the rotation amount using speed
        LeanTween.rotate(gameObject, rotationAmount, rotationTime)
            .setEase(LeanTweenType.easeInOutSine).setLoopPingPong();
    }

}
