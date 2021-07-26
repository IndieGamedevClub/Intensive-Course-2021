using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    public GameObject otherHurtBox; //другая рука зомби

    public int damage; //к-во урона

    private void OnTriggerEnter(Collider other)
    {
        //Если мы столкнулись с чем-то и тег объекта Player
        if(other.gameObject.CompareTag("Player"))
        {
            //Говорим игроку, что он получил урон
            other.gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().playerHealth -= damage;

            //выключаем руки, чтобы избежать мнгновенной смерти игрока
            gameObject.SetActive(false); 
            otherHurtBox.SetActive(false);
        }
    }
}
