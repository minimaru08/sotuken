using UnityEngine;

/// <summary>
/// 的を生成位置を中心に往復させる。撃たれて物理が有効になったら停止する。
/// </summary>
public class TargetMover : MonoBehaviour
{
    // 動き方の種類
    public enum MoveType { Horizontal, Vertical, Circle }

    [SerializeField] MoveType moveType = MoveType.Horizontal;
    [SerializeField] float amplitude = 0.5f;    // 振れ幅(メートル)
    [SerializeField] float speed = 1f;          // 往復の速さ
    [SerializeField] bool randomPhase = true;   // 的ごとに動きの位相をずらす

    Rigidbody rb;
    Vector3 origin;      // 生成されたときの位置。動きの中心になる
    Vector3 axisRight;   // 横移動に使う軸
    float phase;         // 位相のずれ

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 今いる場所を動きの基準として覚えておく
        origin = transform.position;

        // 的は生成時にプレイヤーのほうを向いているので、
        // その「右方向」をそのまま横移動の軸として使える
        axisRight = transform.right;

        // 全部の的が同じ動きをすると機械的に見えるので、
        // 開始位置を的ごとにランダムにずらす
        phase = randomPhase ? Random.Range(0f, Mathf.PI * 2f) : 0f;
    }

    // 物理で動かすものは Update ではなく FixedUpdate で扱う
    void FixedUpdate()
    {
        // 撃たれて物理が有効になったら、以降は物理に任せて手を引く。
        // ここで止めないと、落下しようとする物理と座標の上書きが
        // 毎フレーム競合して、的がその場で震える
        if (rb == null || !rb.isKinematic) return;

        // Sin は -1 〜 +1 を繰り返す波。これに振れ幅を掛けて往復にする
        float t = Time.time * speed + phase;
        Vector3 offset;

        switch (moveType)
        {
            case MoveType.Vertical:
                offset = Vector3.up * (Mathf.Sin(t) * amplitude);
                break;

            case MoveType.Circle:
                // 横に Cos、縦に Sin を使うと円を描く
                offset = axisRight * (Mathf.Cos(t) * amplitude)
                       + Vector3.up * (Mathf.Sin(t) * amplitude);
                break;

            default:    // Horizontal
                offset = axisRight * (Mathf.Sin(t) * amplitude);
                break;
        }

        // transform.position を直接書き換えず MovePosition を使う。
        // こちらは物理エンジンに移動を伝えるので、当たり判定が正しく働く
        rb.MovePosition(origin + offset);
    }
}