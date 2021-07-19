﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float lifeTime = 1.5f;
    public float bulletSpeed = 10f;
    private float timer = 0f;
    void Start()
    {
        this.gameObject.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward*bulletSpeed, ForceMode.Impulse);
    }
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>=lifeTime)
            Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().DieFromPlayer();
            Destroy(this.gameObject);
        }
    }
    
}
