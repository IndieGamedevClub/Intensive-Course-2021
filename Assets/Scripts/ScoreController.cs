using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    public int score = 0;
    public int neededScore = 1;
    public Text scoreText;

    void Update()
    {
        scoreText.text = score.ToString();

        if(score >= neededScore && this.gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>())
        {
            this.gameObject.GetComponent<FPSControllerLPFP.FpsControllerLPFP>().win = true;
        }
    }
}
