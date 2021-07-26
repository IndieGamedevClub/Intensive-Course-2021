using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float lifeTime = 10f; //Время жизни врага
    private float timer = 0f;
    void Start()
    {
        
    }

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

        ///////
        
        Destroy(this.gameObject);
    }
}
