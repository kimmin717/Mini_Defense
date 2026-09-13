using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    [Header("ESC 메뉴 UI")]
    [SerializeField] private GameObject _gamePauseUI;

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

        if (_gamePauseUI != null)
        { 
            _gamePauseUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePause();
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

    public void GameExit()
    {
        CPrint.Log("게임을 종료합니다.");

        Time.timeScale = 1f;

        Application.Quit();
    }

    public void GamePause()
    {
        if(Time.timeScale == 0f)
        {
            Time.timeScale = 1f;

            if (_gamePauseUI != null)
            {
                _gamePauseUI.SetActive(false);
            }
        }

        else
        {
            Time.timeScale = 0f;

            if (_gamePauseUI != null)
            {
                _gamePauseUI.SetActive(true);
            }
        }       
    }
}
