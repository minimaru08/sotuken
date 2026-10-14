using UnityEngine;

/// <summary>
/// 敵が撃ってくる弾。カメラ(プレイヤーの顔)に近づいたらダメージを与えて消える。
/// </summary>
public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] float lifeTime = 6f;
    [SerializeField] float hitRadius = 0.35f;   // 命中とみなす距離(メートル)

    Vector3 velocity;
    Camera cam;

    void Start()
    {
        cam = Camera.main;

        // 外れた弾が飛び続けないよう、寿命で必ず消す
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 発射方向と速さを与える。生成した直後に呼ぶ。
    /// </summary>
    public void Launch(Vector3 direction, float speed)
    {
        velocity = direction.normalized * speed;
    }

    void FixedUpdate()
    {
        // Rigidbody を使わず自前で動かす。
        // 障害物やプレイヤーの球とぶつかって弾かれないので挙動が読みやすい
        transform.position += velocity * Time.fixedDeltaTime;

        if (cam == null) return;

        // Collider ではなく距離で判定する。
        // カメラに当たり判定を置くと、プレイヤー自身が目の前に出した球と
        // ぶつかって自爆するなど、面倒な問題が起きる
        if (Vector3.Distance(transform.position, cam.transform.position) <= hitRadius)
        {
            if (PlayerHealth.Instance != null)
                PlayerHealth.Instance.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
