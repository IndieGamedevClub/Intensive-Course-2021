using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int score = 0; //сколько сейчас очков
    public int neededScore = 1; //как много нужно набрать очков
    public Text scoreText;

    void Update()
    {
        scoreText.text = score.ToString(); //переводим число в строку и отображаем её

        if(score >= neededScore && this.gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>()) //проверяем - активен ли игрок
        {
            this.gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().win = true;
        }
    }
}
