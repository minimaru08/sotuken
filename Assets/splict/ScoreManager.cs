using UnityEngine;

/// <summary>
/// スコアの保持・加算・ハイスコア保存を担当する。
/// シーンに1つだけ置く。
/// </summary>
public class ScoreManager : MonoBehaviour
{
    // どこからでも参照できる唯一の実体(シングルトン)
    public static ScoreManager Instance { get; private set; }

    // PlayerPrefs に保存するときの名前
    const string HighScoreKey = "HighScore";

    public int Score { get; private set; }
    public int HighScore { get; private set; }

    // スコアが変わったことを知らせる放送。UIが購読する
    public static event System.Action<int> OnScoreChanged;

    void Awake()
    {
        // 二重に存在してしまったら後から来たほうを消す
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 前回のハイスコアを端末から読み出す(無ければ 0)
        HighScore = PlayerPrefs.GetInt(HighScoreKey, 0);
    }

    // 的の「倒れた」放送を購読する。
    // 購読したら必ず解除すること。しないとシーンを読み直したときに
    // 古い購読が残って、1回倒しただけで何度も加算される
    void OnEnable()
    {
        Target.OnDown += HandleTargetDown;
    }

    void OnDisable()
    {
        Target.OnDown -= HandleTargetDown;
    }

    void HandleTargetDown(Target target)
    {
        AddScore(target.ScoreValue);
    }

    /// <summary>
    /// 点数を加算する。外から直接呼んでもよい。
    /// </summary>
    public void AddScore(int amount)
    {
        Score += amount;

        // ハイスコアを超えたら更新して即保存
        if (Score > HighScore)
        {
            HighScore = Score;
            PlayerPrefs.SetInt(HighScoreKey, HighScore);
            PlayerPrefs.Save();
        }

        // 変わったことを放送する
        OnScoreChanged?.Invoke(Score);
    }

    /// <summary>
    /// スコアを 0 に戻す。リトライ時に呼ぶ。
    /// </summary>
    public void ResetScore()
    {
        Score = 0;
        OnScoreChanged?.Invoke(Score);
    }
}