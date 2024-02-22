using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureScript : MonoBehaviour
{

    SkinnedMeshRenderer meshRenderer;
    Material creatureMat;

    AudioSource creatureAudio;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        if(meshRenderer != null)
        {
            creatureMat = meshRenderer.material;
        }
        creatureAudio = GetComponent<AudioSource>();
        float timeToPlay = Random.Range(0, creatureAudio.clip.length);
        creatureAudio.time = timeToPlay;
        creatureAudio.Play();
    }

    Vector3 lookTarget;
    // Update is called once per frame
    void Update()
    {
        lookTarget = PlayerCarController.Instance.transform.position;
        transform.LookAt(new Vector3(lookTarget.x, transform.position.y, lookTarget.z));       
    }

    public void OnCreatureHit()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        creatureMat.color = Color.red;
    }
}
