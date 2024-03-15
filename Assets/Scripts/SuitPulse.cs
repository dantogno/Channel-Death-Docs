using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitPulse : MonoBehaviour
{
    float tweenTime = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Pulse()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one * 2, tweenTime).setEasePunch();
    }
}
