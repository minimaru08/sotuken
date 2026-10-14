using UnityEngine;

/// <summary>
/// プレイヤーの体力。シーンに1つだけ置く。
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [SerializeField] int maxHealth = 5;
    [SerializeField] float invincibleTime = 1f;   // 被弾直後の無敵時間(秒)

    public int MaxHealth => maxHealth;
    public int Health { get; private set; }
    public bool IsDead => Health <= 0;

    // 体力が変わったことを知らせる放送(現在値, 最大値)
    public static event System.Action<int, int> OnHealthChanged;

    // 被弾した瞬間の放送。画面を赤くする等の演出用
    public static event System.Action OnDamaged;

    // 体力が0になったときの放送。ゲームオーバー処理が購読する
    public static event System.Action OnPlayerDead;

    float lastDamageTime = -999f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Health = maxHealth;
    }

    void Start()
    {
        // UI が購読を終えたあとに初期値を描かせる
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;

        // 無敵時間中は無視する。
        // これが無いと敵弾が固まって飛んできたとき一瞬で全滅する
        if (Time.time - lastDamageTime < invincibleTime) return;

        lastDamageTime = Time.time;
        Health = Mathf.Max(0, Health - amount);

        OnHealthChanged?.Invoke(Health, maxHealth);
        OnDamaged?.Invoke();

        if (Health <= 0) OnPlayerDead?.Invoke();
    }

    public void Heal(int amount)
    {
        if (IsDead) return;

        Health = Mathf.Min(maxHealth, Health + amount);
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

    /// <summary>
    /// リトライ時に呼ぶ。
    /// </summary>
    public void ResetHealth()
    {
        Health = maxHealth;
        lastDamageTime = -999f;
        OnHealthChanged?.Invoke(Health, maxHealth);
    }

#if UNITY_EDITOR
    // エディタでの動作確認用。D キーで 1 ダメージ。
    // #if で囲ってあるので、実機ビルドにはこの処理は含まれない
    void Update()
    {
        var kb = UnityEngine.InputSystem.Keyboard.current;
        if (kb != null && kb.dKey.wasPressedThisFrame) TakeDamage(1);
    }
#endif
}
