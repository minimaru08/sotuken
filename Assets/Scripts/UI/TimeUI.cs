using TMPro;
using UnityEngine;

/// <summary>
/// 残り時間を mm:ss 形式で表示する。残りが少なくなったら色を変える。
/// </summary>
public class TimeUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] float warningThreshold = 10f;      // 何秒以下で警告色にするか
    [SerializeField] Color normalColor = Color.white;
    [SerializeField] Color warningColor = Color.red;

    // 前回表示した秒数。毎フレーム文字列を作り直さないための記録
    int lastShownSecond = -1;

    void Awake()
    {
        if (timeText == null) timeText = GetComponent<TextMeshProUGUI>();
        if (timeText == null) Debug.LogError("TimeUI: timeText が未設定です", this);
    }

    void OnEnable()
    {
        GameTimer.OnTimeChanged += UpdateText;
        if (GameTimer.Instance != null) UpdateText(GameTimer.Instance.Remaining);
    }

    void OnDisable()
    {
        GameTimer.OnTimeChanged -= UpdateText;
    }

    void UpdateText(float remaining)
    {
        if (timeText == null) return;

        // 切り上げ。残り0.3秒でも「1」と出したいので Ceil を使う。
        // Floor だと最後の1秒が「0」と表示されて不自然になる
        int seconds = Mathf.CeilToInt(remaining);

        // 表示が変わらないフレームでは何もしない。
        // 毎フレーム文字列を作るとゴミが溜まって動作が重くなる
        if (seconds == lastShownSecond) return;
        lastShownSecond = seconds;

        int m = seconds / 60;
        int s = seconds % 60;

        // {0:00} は「2桁、足りなければ0埋め」という書式指定
        timeText.text = $"{m:00}:{s:00}";
        timeText.color = remaining <= warningThreshold ? warningColor : normalColor;
    }
}
