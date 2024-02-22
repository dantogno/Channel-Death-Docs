using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [SerializeField]
    GameObject EnemyPrefab;

    [SerializeField]
    int numberOfEnemies;


    float currentDistance;
    [SerializeField]
    float enemyFrequency;
    [SerializeField]
    float enemyFrequencyScale;
    [SerializeField]
    float enemyFrequencyStartScale;
    [SerializeField]
    float enemyFrequencyVariation;
    [SerializeField]
    float firstEnemyDistance;
    [SerializeField]
    float XSpawnWidth;
    [SerializeField]
    float spawnHeight;
    // Start is called before the first frame update
    float currentFreq;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        SetupEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    {
        GameObject e = Instantiate(EnemyPrefab, this.transform);
        e.transform.position = GetSpawnLocation();
        
    }

    Vector3 GetSpawnLocation()
    {
        float z = PlayerCarController.Instance.transform.position.z + currentDistance; 
        return new Vector3(Random.Range(-XSpawnWidth, XSpawnWidth), spawnHeight, z);
    }

    public void DestroyEnemies()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
        {
            Object.Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void SetupEnemies()
    {
        currentFreq = enemyFrequency * enemyFrequencyStartScale;
        currentDistance = firstEnemyDistance;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
            currentDistance += (currentFreq + Random.Range(-enemyFrequencyVariation, enemyFrequencyVariation));
            if (currentFreq > enemyFrequency)
            {
                currentFreq *= enemyFrequencyScale;
            }
        }
    }
}
