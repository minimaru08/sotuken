using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    [SerializeField] GameObject targetPrefab;

    [Header("配置")]
    [SerializeField] float distance = 3f;        // カメラから何メートル前に出すか
    [SerializeField] float spread = 1.2f;        // 横方向の広がり(中央から±この値)
    [SerializeField] float heightJitter = 0.3f;  // 的ごとの上下のばらつき

    [Header("ウェーブ")]
    [SerializeField] int baseCount = 3;          // ウェーブ1の的の数
    [SerializeField] int addPerWave = 1;         // 1ウェーブごとに増える数
    [SerializeField] int maxWave = 10;          //  ウェーブの数の上限
    [SerializeField] float waveInterval = 2f;    // 全滅してから次が出るまでの待ち時間

    [Header("タイミング")]
    [SerializeField] float startDelay = 1.5f;    // ARのトラッキングが安定するまで待つ

    // 現在のウェーブ数。UIから読めるよう public にしてある
    public int WaveNumber { get; private set; }

    // 今出ている的の一覧。破棄されると要素が null になる
    readonly List<GameObject> alive = new List<GameObject>();

    bool waiting;   // 次のウェーブを待っている最中か

    void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        SpawnWave();
    }

    void Update()
    {
        // 待機中、またはまだ1体も出していないときは何もしない
        if (waiting || alive.Count == 0) return;

        // 破棄された的は null になっているので取り除く
        alive.RemoveAll(t => t == null);

        // 全部消えたら次のウェーブへ
        if (alive.Count == 0)
            StartCoroutine(NextWaveAfterDelay());
    }

    IEnumerator NextWaveAfterDelay()
    {
        waiting = true;
        yield return new WaitForSeconds(waveInterval);
        waiting = false;
        SpawnWave();
    }

    public void SpawnWave()
    {
        if (arCamera == null || targetPrefab == null)
        {
            Debug.LogError("TargetSpawner: arCamera または targetPrefab が未設定です");
            return;
        }

        WaveNumber++;

        // ウェーブが進むほど的を増やす(上限あり)
        int count = Mathf.Min(baseCount + (WaveNumber - 1) * addPerWave, maxWave);

        Transform cam = arCamera.transform;

        // 端末がどう傾いていても的が水平に並ぶよう、
        // カメラの向きを水平面に投影してから使う
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 centre = cam.position + forward * distance;

        alive.Clear();

        for (int i = 0; i < count; i++)
        {
            // 横一列の位置を -1 〜 +1 で表す(1個のときは中央)
            float t = count == 1 ? 0f : (i / (float)(count - 1)) * 2f - 1f;

            Vector3 pos = centre
                        + right * (t * spread)
                        + Vector3.up * Random.Range(-heightJitter, heightJitter);

            // 的をプレイヤーのほうへ向ける
            GameObject target = Instantiate(
                targetPrefab, pos, Quaternion.LookRotation(-forward, Vector3.up));

            // 全滅判定に使うので記録しておく
            alive.Add(target);
        }

        Debug.Log($"[TargetSpawner] ウェーブ {WaveNumber}: 的を {count} 個生成");
    }
}