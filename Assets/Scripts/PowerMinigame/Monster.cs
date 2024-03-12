using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Vector3 initialPosition;

    public enum MonsterState { approaching, retracting}
    public MonsterState monsterState;

    public float ApproachSpeed;
    public float RetractSpeed;
    public float ApproachSpeedIncrease;

    Vector3 direction;

    [HideInInspector]
    public float startDistance;

    float currentApproachSpeed;

    [SerializeField]
    GameObject monsterMesh;

    [SerializeField]
    AudioSource monsterRunSource;

    [SerializeField]
    float footstepVolume;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    public void SetupMonster()
    {
        monsterMesh.SetActive(true);
        currentApproachSpeed = ApproachSpeed;
        transform.position = initialPosition;
        startDistance = Vector3.Distance(PowerPlayerController.Instance.transform.position, transform.position);
        direction = PowerPlayerController.Instance.transform.position - transform.position;
        direction.y = initialPosition.y;
    }

    public void IncreaseApproach()
    {
        currentApproachSpeed += ApproachSpeedIncrease;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceFromPlayer = Vector3.Distance(PowerPlayerController.Instance.transform.position, transform.position);
        switch (monsterState)
        {
            case MonsterState.approaching:
                monsterRunSource.volume = 0.0f;
                transform.position += direction * currentApproachSpeed * Time.deltaTime;
                break; 
            case MonsterState.retracting:
                monsterRunSource.volume = footstepVolume;
                if(distanceFromPlayer < startDistance)
                {
                    transform.position -= direction * RetractSpeed * Time.deltaTime;
                }
                break;
        }
    }

    public void SetMonsterState(MonsterState state)
    {
        switch(state)
        {
            case MonsterState.approaching:
                break;
            case MonsterState.retracting:
                break;
        }
        this.monsterState = state;
    }

    public void HideMonster()
    {
        monsterMesh.SetActive(false);
    }

}
