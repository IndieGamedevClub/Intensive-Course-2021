using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject[] zombieType; //какие типы зомби доступны

    public float timeForNextZombie; //время до следующего зомби

    void Start()
    {
        StartCoroutine(Summon()); //Запускаем процесс создания зомби
    }

    //Рекурсивное создание зомби
    IEnumerator Summon()
    {
        int i = Mathf.RoundToInt(Random.Range(0, zombieType.Length)); //выбираем случайного зомби

        Instantiate(zombieType[i], this.transform.position, Quaternion.identity); //создаем его

        yield return new WaitForSeconds(timeForNextZombie); //ждем таймера

        StartCoroutine(Summon()); //снова вызываем зомби
    }
}
