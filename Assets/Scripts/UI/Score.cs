using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class Score : MonoBehaviour
{
    // Start is called before the first frame update
    public Text scoreNumber;
    public Canvas winUI;
    public GameObject GameManager;
    public int playerScore = 0;
    public int winScore = 10;

    // Update is called once per frame
    void Update()
    {
        scoreNumber.text = playerScore.ToString();

        if (playerScore >= winScore)
        {
            winUI.gameObject.SetActive(true);
            GameManager.GetComponent<EnemyController>().spawn = false;
        }
    }

    public void AddScore()
    {
        playerScore += 1;
    }
}
