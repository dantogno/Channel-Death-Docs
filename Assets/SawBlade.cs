using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawBlade : MonoBehaviour
{
    [SerializeField]
    Material bloodySaw;

    MeshRenderer sawRenderer;

    [SerializeField]
    List<GameObject> splatters;
    bool BloodActive;
    // Start is called before the first frame update
    void Start()
    {
        sawRenderer = GetComponent<MeshRenderer>();
        GameManager.VictimDied += SetSawBladeBloody;
    }
    private void OnDestroy()
    {
        GameManager.VictimDied -= SetSawBladeBloody;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SetSawBladeBloody(Victim v)
    {
        if (!BloodActive)
        {
            BloodActive = true;
            sawRenderer.material = bloodySaw;
            foreach (GameObject go in splatters)
            {
                go.SetActive(true);
            }
        }
    }

}
