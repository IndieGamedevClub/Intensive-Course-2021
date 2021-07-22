using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombieType;
    public float timeForNextZombie;
    public bool canSummon = true;

    void Start()
    {
        StartCoroutine(Summon());
    }

    IEnumerator Summon()
    {
        if (!canSummon)
            yield return 0;

        int i = Mathf.RoundToInt(Random.Range(0, zombieType.Length-1));
        Instantiate(zombieType[i], this.transform.position, Quaternion.identity);

        yield return new WaitForSeconds(timeForNextZombie);
        StartCoroutine(Summon());
    }
}
