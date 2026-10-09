using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel; // 一時停止画面のUI

    // 一時停止
    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    // 再開
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 最初から始める
    public void RestartGame()
    {
        Time.timeScale = 1f; // 忘れがち。止まったまま再読み込みすると動かない
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ゲーム終了
    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();

        // Unityエディタで動作確認用
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
