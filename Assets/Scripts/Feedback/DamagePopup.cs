using TMPro;
using UnityEngine;

/// <summary>
/// 当たった位置に点数を浮かび上がらせて、上昇しながら消える。
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [SerializeField] TextMeshPro text;        // UGUI ではなく 3D 用の TextMeshPro
    [SerializeField] float riseSpeed = 0.4f;  // 上昇の速さ(m/秒)
    [SerializeField] float lifeTime = 1f;

    Camera cam;
    Color baseColor;
    float elapsed;

    // Instantiate した瞬間に走るので、Setup より先に用意が整う
    void Awake()
    {
        if (text == null) text = GetComponentInChildren<TextMeshPro>();
        cam = Camera.main;
        baseColor = text != null ? text.color : Color.white;
    }

    public void Setup(int amount, bool critical)
    {
        if (text == null) return;

        text.text = critical ? $"{amount}!" : amount.ToString();
        baseColor = critical ? new Color(1f, 0.85f, 0.2f) : Color.white;
        text.color = baseColor;

        // クリティカルは大きく出して目立たせる
        if (critical) transform.localScale *= 1.6f;
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        transform.position += Vector3.up * riseSpeed * Time.deltaTime;

        // 常にカメラのほうを向かせる(ビルボード)。
        // カメラ「へ」ではなくカメラ「から」の向きを使うのがポイント。
        // 逆にすると文字が裏返る
        if (cam != null)
            transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        // 時間とともに薄くする
        if (text != null)
        {
            float alpha = 1f - elapsed / lifeTime;
            text.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        if (elapsed >= lifeTime) Destroy(gameObject);
    }
}