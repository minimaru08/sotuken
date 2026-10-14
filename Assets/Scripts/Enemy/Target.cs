using UnityEngine;

/// <summary>
/// 的。撃たれるまで空中に固定され、当たると物理で倒れる。
/// </summary>
public class Target : MonoBehaviour
{
    [Header("点数")]
    [SerializeField] int scoreValue = 100;             // 通常ヒットの点数
    [SerializeField] float criticalMultiplier = 3f;    // クリティカル時の倍率

    [Header("倒れ方")]
    [SerializeField] float knockForce = 1.5f;          // 倒す力
    [SerializeField] float criticalKnockBonus = 2f;    // クリティカル時の力の倍率
    [SerializeField] float despawnDelay = 1.5f;        // 倒れてから消えるまで(0で消さない)

    [Header("演出")]
    [SerializeField] GameObject damagePopupPrefab;     // 点数表示(空でも可)

    // 倒れたことを知らせる放送
    public static event System.Action<Target> OnDown;

    // クリティカルだったか。倒れたあとに読む
    public bool WasCritical { get; private set; }

    // 実際に入る点数。倍率を適用済みの値を返す
    public int Points => WasCritical
        ? Mathf.RoundToInt(scoreValue * criticalMultiplier)
        : scoreValue;

    Rigidbody rb;
    bool isDown;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 置いた瞬間に落下しないよう、最初は物理を止めておく
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDown) return;

        // ぶつかってきたのが球でなければ無視する
        if (collision.collider.GetComponentInParent<Projectile>() == null) return;

        ContactPoint contact = collision.GetContact(0);

        // thisCollider は「自分側のどの Collider が当たられたか」。
        // 弱点の Collider なら WeakPoint が付いている
        WasCritical = contact.thisCollider != null
                   && contact.thisCollider.GetComponent<WeakPoint>() != null;

        isDown = true;
        OnDown?.Invoke(this);     // スコア側へ放送

        rb.isKinematic = false;   // ここで初めて物理が効き始める

        // 球の進行方向 ≒ -relativeVelocity
        Vector3 dir = -collision.relativeVelocity.normalized;
        float force = knockForce * (WasCritical ? criticalKnockBonus : 1f);

        // 当たった「点」に力を加えると、回転が生まれて自然に倒れる
        rb.AddForceAtPosition(dir * force, contact.point, ForceMode.Impulse);

        ShowPopup(contact.point);

        if (despawnDelay > 0f) Destroy(gameObject, despawnDelay);
    }

    void ShowPopup(Vector3 worldPos)
    {
        if (damagePopupPrefab == null) return;

        GameObject popup = Instantiate(damagePopupPrefab, worldPos, Quaternion.identity);
        if (popup.TryGetComponent(out DamagePopup dp))
            dp.Setup(Points, WasCritical);
    }
}