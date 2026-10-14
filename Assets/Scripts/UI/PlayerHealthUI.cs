using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 体力の表示と、被弾時の赤いフラッシュ。
/// </summary>
public class PlayerHealthUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image damageFlash;          // 全画面の赤い Image(未設定でも可)
    [SerializeField] float flashDuration = 0.3f;
    [SerializeField] float flashAlpha = 0.4f;    // フラッシュの濃さの上限

    float flashTimer;

    void Awake()
    {
        // 同じオブジェクトに付いている TMP を自動で拾う
        if (healthText == null) healthText = GetComponent<TextMeshProUGUI>();

        if (healthText == null)
            Debug.LogError("PlayerHealthUI: healthText が未設定です", this);
    }

    void OnEnable()
    {
        PlayerHealth.OnHealthChanged += UpdateDisplay;
        PlayerHealth.OnDamaged += Flash;

        // 購読を始めた時点の値で一度描いておく
        if (PlayerHealth.Instance != null)
            UpdateDisplay(PlayerHealth.Instance.Health, PlayerHealth.Instance.MaxHealth);
    }

    void OnDisable()
    {
        PlayerHealth.OnHealthChanged -= UpdateDisplay;
        PlayerHealth.OnDamaged -= Flash;
    }

    void UpdateDisplay(int current, int max)
    {
        if (healthText == null) return;

        healthText.text = $"HP {current}/{max}";

        // ハートで表示したい場合はこちら。
        // フォントアセットに該当の文字が含まれている必要がある
        // healthText.text = new string('♥', current) + new string('♡', max - current);
    }

    void Flash()
    {
        flashTimer = flashDuration;
    }

    void Update()
    {
        if (damageFlash == null || flashTimer <= 0f) return;

        flashTimer -= Time.deltaTime;

        // 残り時間の割合をそのまま濃さにする(だんだん薄くなる)
        float a = Mathf.Max(0f, flashTimer / flashDuration) * flashAlpha;

        Color c = damageFlash.color;
        damageFlash.color = new Color(c.r, c.g, c.b, a);
    }
}
