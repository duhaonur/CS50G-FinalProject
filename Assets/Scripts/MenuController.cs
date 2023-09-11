using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void LoadPlayScene()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
