using UnityEngine;

/// <summary>
/// 制限時間のカウントダウンを管理する。シーンに1つだけ置く。
/// </summary>
public class GameTimer : MonoBehaviour
{
    public static GameTimer Instance { get; private set; }

    [SerializeField] float timeLimit = 60f;   // 制限時間(秒)
    [SerializeField] bool autoStart = true;   // 起動と同時に始めるか

    public float Remaining { get; private set; }
    public bool IsRunning { get; private set; }

    // 残り時間が変わったことを知らせる放送(UIが購読)
    public static event System.Action<float> OnTimeChanged;

    // 時間切れの放送(終了処理が購読)
    public static event System.Action OnTimeUp;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Remaining = timeLimit;
    }

    void Start()
    {
        OnTimeChanged?.Invoke(Remaining);   // 開始時の値を一度描かせる
        if (autoStart) StartTimer();
    }

    void Update()
    {
        if (!IsRunning) return;

        // Time.deltaTime は「前のフレームからの経過秒数」。
        // 端末の性能に関係なく、実時間どおりに減る
        Remaining -= Time.deltaTime;

        if (Remaining <= 0f)
        {
            Remaining = 0f;
            IsRunning = false;
            OnTimeChanged?.Invoke(Remaining);
            OnTimeUp?.Invoke();     // 時間切れを放送
            return;
        }

        OnTimeChanged?.Invoke(Remaining);
    }

    public void StartTimer()
    {
        Remaining = timeLimit;
        IsRunning = true;
        OnTimeChanged?.Invoke(Remaining);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    /// <summary>
    /// 時間を加算する。的を倒したらボーナス、といった使い方ができる。
    /// </summary>
    public void AddTime(float seconds)
    {
        if (!IsRunning) return;
        Remaining += seconds;
        OnTimeChanged?.Invoke(Remaining);
    }
}