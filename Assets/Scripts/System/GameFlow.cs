using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 時間切れまたはプレイヤー死亡を受けてゲームを終了させ、結果を表示する。
/// </summary>
public class GameFlow : MonoBehaviour
{
    [SerializeField] Shooter shooter;
    [SerializeField] TargetSpawner spawner;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TextMeshProUGUI resultText;

    bool ended;   // 二重に終了処理が走らないようにする旗

    void OnEnable()
    {
        // 時間切れと死亡は「やること」が同じなので、同じメソッドを購読させる
        GameTimer.OnTimeUp += HandleGameEnd;
        PlayerHealth.OnPlayerDead += HandleGameEnd;
    }

    void OnDisable()
    {
        GameTimer.OnTimeUp -= HandleGameEnd;
        PlayerHealth.OnPlayerDead -= HandleGameEnd;
    }

    // Start ではなく Awake で消す。
    // Start だと最初の1フレームだけパネルが表示されたままになり、
    // 全画面の Raycast Target がタップを横取りすることがある
    void Awake()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    void HandleGameEnd()
    {
        if (ended) return;
        ended = true;

        // 死亡後も残り時間が減り続けないよう止める
        if (GameTimer.Instance != null) GameTimer.Instance.StopTimer();

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
