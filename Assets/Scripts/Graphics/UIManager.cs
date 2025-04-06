using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene(1);
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
