using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(0);
    }

    public static void EndGame(int difference)
    {
        if (difference > 0)
            GameObject.FindGameObjectWithTag("WinPanel").GetComponent<Animator>().SetTrigger("lose");
        else if(difference < 0 )
            GameObject.FindGameObjectWithTag("WinPanel").GetComponent<Animator>().SetTrigger("win");
        else
            GameObject.FindGameObjectWithTag("WinPanel").GetComponent<Animator>().SetTrigger("tie");
    }
}
