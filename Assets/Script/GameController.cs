using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // ������ �ٽ� �����ϴ� �޼���
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // ������ �����ϴ� �޼���
    public void QuitGame()
    {
        Application.Quit();
    }
}
