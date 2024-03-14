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
    AudioSource monsterLoopSource, hissSource;

    [SerializeField]
    float footstepVolume, hissVolume;

    float initialLoopVOl;

    [SerializeField]
    float distanceTowardsPlayerAtStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InitializeMonster()
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

    MonsterState prevState;
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
                if(prevState == MonsterState.idle || prevState == MonsterState.approaching)
                {
                    hissSource.volume = MapValueReverseExponential(distanceFromPlayer, startDistance, PowerPlayerController.Instance.LoseThreshHold, 0, hissVolume);
                    hissSource.Play();
                }
                monsterLoopSource.volume = initialLoopVOl;
                monsterRunSource.volume = footstepVolume;
                if(distanceFromPlayer < startDistance)
                {
                    transform.position -= direction * RetractSpeed * Time.deltaTime;
                }
                break;
        }
        prevState = monsterState;

    }

    public void HideMonster()
    {
        monsterMesh.SetActive(false);
    }
    public static float MapValueReverseExponential(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        float normalizedValue = (value - fromMin) / (fromMax - fromMin);
        normalizedValue = Mathf.Pow(normalizedValue, 3f);
        float mappedValue = normalizedValue * (toMax - toMin) + toMin;
        return mappedValue;
    }
}
