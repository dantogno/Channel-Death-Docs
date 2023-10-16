using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public GameObject player;

    private void Update()
    {
        this.transform.LookAt(player.transform);
    }
}
