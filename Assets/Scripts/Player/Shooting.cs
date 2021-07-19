using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject bulletPrefab;
    public Transform bulletSocket;
    public float timeToNextBullet = 0.5f;

    private float timer = 0f;
    void Start()
    {
        
    }

    // Update is called once per frame
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
