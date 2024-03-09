using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HingedDoor : Door
{
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        DoorUpdate();
    }

    public override void DoorUpdate()
    {
        base.DoorUpdate();
    }

    public override void OpenDoor()
    {
        base.OpenDoor();
    }
}
