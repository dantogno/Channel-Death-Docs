using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Door : MonoBehaviour
{

    [SerializeField]
    protected float doorSpeed = 0.5f;

    protected bool openDoor;
    protected bool solved;

    protected float initalZ;

    float progress;

    AudioSource source;
    [SerializeField]
    AudioClip doorOpenSound;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        source = this.gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = doorOpenSound;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void DoorUpdate()
    {
        if (openDoor && progress <= 90)
        {
            Vector3 euler = transform.rotation.eulerAngles;
            euler.z -= Time.deltaTime * doorSpeed;
            progress += Time.deltaTime * doorSpeed;
            transform.eulerAngles = euler;
        }
    }

    public virtual void OpenDoor()
    {
        if (!solved)
        {
            source.Play();
            initalZ = transform.eulerAngles.z;
            solved = true;
            openDoor = true;
        }
    }
}
