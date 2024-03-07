using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CensorFollow : MonoBehaviour
{
    [SerializeField] private GameObject followTarget;

     private Vector3 offset;

    private void Start()
    {
        // initialize offset based on distance from the target
        offset = transform.position - followTarget.transform.position;
    }

    private void Update()
    {
        // follow the target with the offset
        transform.position = followTarget.transform.position + offset;
    }


}
