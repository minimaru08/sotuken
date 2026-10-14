using TMPro;
using UnityEngine;

/// <summary>
/// スコアの変化を受け取って画面に表示する。
/// </summary>
public class ScoreUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] string format = "SCORE {0}";   // {0} に数値が入る

    void OnEnable()
    {
        ScoreManager.OnScoreChanged += UpdateText;

        // 購読を始めた時点の値で一度描いておく。
        // これが無いと、最初の1点が入るまで何も表示されない
        UpdateText(ScoreManager.Instance != null ? ScoreManager.Instance.Score : 0);
    }

    void OnDisable()
    {
        ScoreManager.OnScoreChanged -= UpdateText;
    }

    void UpdateText(int score)
    {
        if (scoreText != null)
            scoreText.text = string.Format(format, score);
    }
}