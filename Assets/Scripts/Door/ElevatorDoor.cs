using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorDoor : Door
{
    [SerializeField]
    GameObject elevatorLeft, elevatorRight;

    Vector3 initialLeftPos, initialRightPos;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        initialLeftPos = elevatorLeft.transform.position;
        initialRightPos = elevatorRight.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        DoorUpdate();
    }

    public override void DoorUpdate()
    {
        if (openDoor)
        {
            if (elevatorLeft.transform.position.x > -100)
            {
                elevatorLeft.transform.position += Vector3.left * doorSpeed * Time.deltaTime;
            }
            if (elevatorRight.transform.position.x < 100)
            {
                elevatorRight.transform.position += Vector3.right * doorSpeed * Time.deltaTime;
            }
        }
    }

    public override void ResetDoor()
    {
        base.ResetDoor();
        elevatorLeft.transform.position = initialLeftPos;
        elevatorRight.transform.position= initialRightPos;
    }
}
