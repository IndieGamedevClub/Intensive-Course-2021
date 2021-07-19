using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public float lifeTime = 10f;
    private float timer = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
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
        GameObject[] tmp = GameObject.FindGameObjectsWithTag("Player");
        tmp[0].GetComponent<Score>().AddScore();
        ///////
        
        Destroy(this.gameObject);
    }
}
