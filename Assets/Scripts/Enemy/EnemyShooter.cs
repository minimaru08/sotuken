using UnityEngine;

/// <summary>
/// 的に付けると、一定間隔でプレイヤーを狙って弾を撃つ。
/// 撃たれて倒れたら攻撃をやめる。
/// </summary>
public class EnemyShooter : MonoBehaviour
{
    [SerializeField] GameObject enemyProjectilePrefab;
    [SerializeField] float fireInterval = 3f;       // 発射間隔(秒)
    [SerializeField] float firstDelay = 2f;         // 最初の1発までの猶予(秒)
    [SerializeField] float projectileSpeed = 2.5f;  // 弾の速さ(メートル/秒)
    [SerializeField] float muzzleOffset = 0.2f;     // 自分の体から離して出す距離
    [SerializeField] float fireIntervalMin = 2f;
    [SerializeField] float fireIntervalMax = 5f;

    Camera cam;
    Rigidbody rb;
    float nextFireTime;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();

        // 全員が同時に撃つと避けようがないので、最初の発射をばらけさせる
        nextFireTime = Time.time + firstDelay + Random.Range(0f, fireInterval);
    }

    void Update()
    {
        // 撃たれて物理が有効になった = 倒れたので、以降は攻撃しない。
        // TargetMover が移動をやめるのと同じ判定方法
        if (rb != null && !rb.isKinematic) return;

        if (cam == null || Time.time < nextFireTime) return;

        Fire();
        nextFireTime = Time.time + fireInterval;
    }

    void Fire()
    {
        if (enemyProjectilePrefab == null) return;

        Vector3 dir = (cam.transform.position - transform.position).normalized;
        Vector3 pos = transform.position + dir * muzzleOffset;

        GameObject bullet = Instantiate(
            enemyProjectilePrefab, pos, Quaternion.LookRotation(dir));

        if (bullet.TryGetComponent(out EnemyProjectile ep))
            ep.Launch(dir, projectileSpeed);
    }
}
