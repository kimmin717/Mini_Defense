using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("인 게임 화면")]
    [SerializeField] private string _inGameScene = "Mini_Defense";

    // 게임 시작시 호출
    public void GameStart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(_inGameScene);
    }

    public void GameEnd()
    {
        CPrint.Log("게임을 종료합니다.");

        Application.Quit();
    }
}
