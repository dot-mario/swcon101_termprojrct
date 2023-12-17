using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    // 게임을 다시 시작하는 메서드
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // 게임을 종료하는 메서드
    public void QuitGame()
    {
        Application.Quit();
    }
}
