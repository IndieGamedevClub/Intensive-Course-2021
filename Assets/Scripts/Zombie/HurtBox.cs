using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public GameObject otherHurtBox;
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().playerHealth -= damage;
            
            gameObject.SetActive(false);
            otherHurtBox.SetActive(false);
        }
    }
}
