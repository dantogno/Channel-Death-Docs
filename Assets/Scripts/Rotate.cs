using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public Vector3 rotationSpeed = Vector3.zero;
    
    // Update is called once per frame
    void Update()
    {
       // rotate based on rotation speed    
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
