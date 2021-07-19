using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float lifeTime = 10f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= lifeTime)
        {
            timer = 0f;
            Destroy(this.gameObject);
        }
    }

    public void DieFromPlayer()
    {
        //добавить очко в счетчик
        GameObject[] player = GameObject.FindGameObjectsWithTag("Player");
        player[0].GetComponent<Score>().AddScore();
        
        Destroy(this.gameObject);
    }
}
