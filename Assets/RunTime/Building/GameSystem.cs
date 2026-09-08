using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    [Header("메뉴 UI")]
    [SerializeField] private GameObject _gameTitleUI;

    [Header("게임 오버 UI")]
    [SerializeField] private GameObject _gameOverUI;

    private void Start()
    {
        if (_gameOverUI == null)
        {
            CPrint.Warn("GameOverUI 연결 필요 확인 요망");
            return;
        }

        if( _gameOverUI != null)
        {
            _gameOverUI.SetActive(false);
        }
    }

    // 게임 오버시 호출
    public void GameOver()
    {
        if (_gameOverUI != null)
        {
            _gameOverUI.SetActive(true);
        }

        // 시간 정지
        Time.timeScale = 0f;
    }

    // 다시 하기 버튼
    public void Restart()
    {
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    // 메인 메뉴 버튼
    public void Menu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }

    // 게임 종료
    private void GameEnd()
    {
        Time.timeScale = 1f;

        Application.Quit();
    }
}
