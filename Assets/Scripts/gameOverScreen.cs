using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public void StartScreen()
    {
        Debug.Log("yo");
        SceneManager.LoadScene(0);
    }
}
