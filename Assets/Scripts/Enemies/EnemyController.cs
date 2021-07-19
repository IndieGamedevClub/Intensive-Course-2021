using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class EnemyController : MonoBehaviour
{
    public float timeToSpawn = 5f;
    public bool spawn = true;
    
    public float minXRange = 0f;
    public float maxXRange = 10f;
    public float minYRange = 0f;
    public float maxYRange = 10f;
    public float minZRange = 0f;
    public float maxZRange = 10f;
    
    private float timer = 0f;
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= timeToSpawn && spawn)
        {
            Vector3 randomPos = new Vector3(Random.Range(minXRange, maxXRange), Random.Range(minYRange, maxYRange),
                Random.Range(minZRange, maxZRange));
            Instantiate(enemyPrefab, randomPos, Quaternion.identity);
            timer = 0f;
        }
    }
    
    
}
