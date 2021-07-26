using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletSocket;
    public float timeToNextBullet = 0.5f;

    private float timer = 0f;
    void Start()
    {
        
    }
    void Update()
    {
        timer += Time.deltaTime;
        if (Input.GetMouseButtonDown(0) && timer >= timeToNextBullet)
        {
            timer = 0f;
            Instantiate(bulletPrefab, bulletSocket.position, bulletSocket.rotation);
        }
    }
}
