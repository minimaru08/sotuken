using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

/// <summary>
/// 画面をタップすると、レティクル(画面中央)の向きへ球を発射する。
/// </summary>
public class Shooter : MonoBehaviour
{
    [SerializeField] Camera arCamera;              // ARカメラ(XR Origin配下のMain Camera)
    [SerializeField] GameObject projectilePrefab;  // 発射する球のPrefab
    [SerializeField] float muzzleDistance = 0.3f;  // カメラから何メートル前に球を出すか
    [SerializeField] float shootSpeed = 8f;        // 球の初速(メートル/秒)
    [SerializeField] float fireInterval = 0.2f;    // 連射の最短間隔(秒)
    [SerializeField] bool debugLog = true;         // 発射時に位置と向きをConsoleへ出すか

    // 最後に撃った時刻。連射制限の判定に使う。
    // -999 という大きな負の値にしておくと、ゲーム開始直後の1発目が
    // 必ず「十分な時間が経った」と判定されるので、待たずに撃てる。
    float lastShotTime = -999f;

    /// <summary>
    /// 生成直後に1回だけ呼ばれる。参照の取りこぼしを補う。
    /// </summary>
    void Awake()
    {
        // Inspectorでカメラを入れ忘れていたら、MainCameraタグの付いた
        // カメラを自動で探して入れる(保険)
        if (arCamera == null) arCamera = Camera.main;
    }

    /// <summary>
    /// このコンポーネントが有効になったとき(シーン開始時など)に呼ばれる。
    /// </summary>
    void OnEnable()
    {
        // 実機のタッチ入力を使えるようにする。
        // これを呼ばないと Touch.activeTouches が常に空のままで、
        // エディタのマウスでは動くのに実機だけ反応しない状態になる。
        EnhancedTouchSupport.Enable();
    }

    /// <summary>
    /// 無効になったときに呼ばれる。有効化したものを元に戻す。
    /// </summary>
    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    /// <summary>
    /// 毎フレーム呼ばれる。発射条件を順に確認し、すべて通れば撃つ。
    /// </summary>
    void Update()
    {
        // 「押された瞬間」でなければ何もしない(押しっぱなしでは撃たない)
        if (!IsPressedThisFrame()) return;

        // UIの上を触っていたら発射しない(ボタン押下での暴発を防ぐ)
        if (IsPointerOverUI()) return;

        // 前回の発射から fireInterval 秒経っていなければ撃たない
        if (Time.time - lastShotTime < fireInterval) return;

        Shoot();
        lastShotTime = Time.time;   // 今の時刻を記録して次の連射制限に使う
    }

    /// <summary>
    /// このフレームで新しく押され始めたか判定する。
    /// 実機のタッチとエディタのマウス、両方に対応。
    /// </summary>
    bool IsPressedThisFrame()
    {
        // --- 実機:タッチ ---
        // activeTouches は今画面に触れている指の一覧。
        // began は「このフレームで触れ始めた指か」を表す。
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].began) return true;
        }

        // --- エディタ:マウス ---
        // Mouse.current は実機では null になるので、null チェックが要る。
        // wasPressedThisFrame は「このフレームで押された瞬間か」。
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    /// <summary>
    /// 球を1発生成して、カメラの正面へ飛ばす。
    /// </summary>
    void Shoot()
    {
        // 参照が欠けていたら例外で落ちる前にエラーを出して抜ける
        if (arCamera == null || projectilePrefab == null)
        {
            Debug.LogError("Shooter: arCamera または projectilePrefab が未設定です");
            return;
        }

        Transform cam = arCamera.transform;

        // レティクルは画面のちょうど中央にあり、そこはカメラの正面軸にあたる。
        // Transform から直接読むことで、画面座標やRayを経由せずに済み、
        // 発射点が常に端末の動きについてくる。
        Vector3 dir = cam.forward;

        // カメラの内側に球が出ると見えない・自分と衝突するので、
        // 少し前(muzzleDistance メートル)に離して生成する。
        Vector3 spawnPos = cam.position + dir * muzzleDistance;

        // Prefabから実体を作る。回転はカメラと揃えておくと、
        // 模様や形のあるモデルを使ったときに向きが自然になる。
        GameObject ball = Instantiate(projectilePrefab, spawnPos, cam.rotation);

        // Rigidbody があれば速度を直接与えて飛ばす。
        // TryGetComponent は「あれば true を返して rb に入れる」という書き方で、
        // GetComponent して null チェックするより簡潔かつ高速。
        if (ball.TryGetComponent(out Rigidbody rb))
            rb.linearVelocity = dir * shootSpeed;

        // 発射位置と向きを記録しておくと、
        // 「狙いがずれる」「的に届かない」といった不具合を切り分けやすい。
        if (debugLog)
            Debug.Log($"[Shooter] cam={cam.position:F2} dir={dir:F2}");
    }

    /// <summary>
    /// 指やマウスカーソルがUI要素の上にあるかを返す。
    /// </summary>
    bool IsPointerOverUI()
    {
        // EventSystem はUIのクリック判定を担当する仕組み。
        // シーンに無い場合もあるので null チェックしてから問い合わせる。
        return EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject();
    }
}