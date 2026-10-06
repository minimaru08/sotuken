using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 時間切れを受けてゲームを終了させ、結果を表示する。
/// </summary>
public class GameFlow : MonoBehaviour
{
    [SerializeField] Shooter shooter;
    [SerializeField] TargetSpawner spawner;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI resultText;

    void OnEnable()
    {
        GameTimer.OnTimeUp += HandleTimeUp;
    }

    void OnDisable()
    {
        GameTimer.OnTimeUp -= HandleTimeUp;
    }

    void Start()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void HandleTimeUp()
    {
        // 発射を止める。コンポーネントを無効にするだけでよい
        if (shooter != null) shooter.enabled = false;

        // スポナーは GameObject ごと止める。
        // コンポーネントの enabled を false にしても、
        // 走っている最中のコルーチンは止まらないため
        if (spawner != null) spawner.gameObject.SetActive(false);

        if (resultText != null && ScoreManager.Instance != null)
        {
            resultText.text =
                $"SCORE  {ScoreManager.Instance.Score}\n" +
                $"BEST   {ScoreManager.Instance.HighScore}";
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// リトライボタンの OnClick から呼ぶ。シーンを読み直して最初から。
    /// </summary>
    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}