using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Monster : MonoBehaviour
{
    Vector3 initialPosition;

    public enum MonsterState { idle, approaching, retracting}
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
    AudioSource monsterLoopSource;

    [SerializeField]
    float footstepVolume;

    float initialLoopVOl;

    [SerializeField]
    float distanceTowardsPlayerAtStart;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
        initialLoopVOl = monsterLoopSource.volume;
    }

    public void SetupMonster()
    {
        monsterState = MonsterState.idle;
        monsterMesh.SetActive(true);
        currentApproachSpeed = ApproachSpeed;
        transform.position = initialPosition;
        startDistance = Vector3.Distance(PowerPlayerController.Instance.transform.position, transform.position);
        direction = PowerPlayerController.Instance.transform.position - transform.position;
        direction.y = initialPosition.y;
        transform.position += direction * distanceTowardsPlayerAtStart; 
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
            case MonsterState.idle:
                monsterLoopSource.volume = 0.0f;
                monsterRunSource.volume = 0.0f;
                break;
            case MonsterState.approaching:
                monsterLoopSource.volume = initialLoopVOl;
                monsterRunSource.volume = 0.0f;
                transform.position += direction * currentApproachSpeed * Time.deltaTime;
                break; 
            case MonsterState.retracting:
                monsterLoopSource.volume = initialLoopVOl;
                monsterRunSource.volume = footstepVolume;
                if(distanceFromPlayer < startDistance)
                {
                    transform.position -= direction * RetractSpeed * Time.deltaTime;
                }
                break;
        }
    }

    public void HideMonster()
    {
        monsterMesh.SetActive(false);
    }

}
