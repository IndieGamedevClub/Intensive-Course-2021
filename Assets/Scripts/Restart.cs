using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public void RestartLevel()
    {
        Debug.Log("PRESSED");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); //Загружаем текущую сцену
    }
}
