using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    private EndlessJumperController ejc;

    private void Awake()
    {
        ejc = GetComponentInParent<EndlessJumperController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        ejc.ReturnGround(other.GetComponentInParent<GroundId>().gameObject);
    }

}
