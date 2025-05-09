using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("SnakeScene");
    }

    public void StopGame()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
