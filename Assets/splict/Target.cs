using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] float knockForce = 1.5f;   // 倒す力
    [SerializeField] float despawnDelay = 1.5f;   // 倒れてから消えるまで(0で消さない)

    Rigidbody rb;
    bool isDown;

    // 倒れたことを知らせる放送。誰でも購読できる。
    // static なので「このクラスの的が倒れた」という一本の窓口になる
    public static event System.Action<Target> OnDown;

    [SerializeField] int scoreValue = 100;   // この的を倒したときの点数

    // 外から点数を読めるようにする(書き換えはさせない)
    public int ScoreValue => scoreValue;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // 置いた瞬間に落下しないよう、最初は物理を止めておく
        rb.isKinematic = true;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isDown) return;
        if (collision.collider.GetComponentInParent<Projectile>() == null) return;

        isDown = true;
        OnDown?.Invoke(this);   // 倒れたことを放送する
        rb.isKinematic = false;   // ここで初めて物理が効き始める

        // 球の進行方向 ≒ -relativeVelocity
        Vector3 dir = -collision.relativeVelocity.normalized;
        Vector3 hitPoint = collision.GetContact(0).point;

        // 当たった「点」に力を加えると、回転が生まれて自然に倒れる
        rb.AddForceAtPosition(dir * knockForce, hitPoint, ForceMode.Impulse);

        if (despawnDelay > 0f) Destroy(gameObject, despawnDelay);
    }
}