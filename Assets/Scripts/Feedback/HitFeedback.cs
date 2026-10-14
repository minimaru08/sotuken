using UnityEngine;

/// <summary>
/// 的が倒れたときの音・エフェクト・振動をまとめて担当する。
/// シーンに1つ置く。
/// </summary>
public class HitFeedback : MonoBehaviour
{
    [Header("効果音")]
    [SerializeField] AudioClip hitClip;          // 通常ヒット
    [SerializeField] AudioClip criticalClip;     // クリティカル(空なら通常音を使う)
    [SerializeField] float volume = 1f;

    [Header("エフェクト")]
    [SerializeField] GameObject hitEffectPrefab;
    [SerializeField] GameObject criticalEffectPrefab;
    [SerializeField] float effectLifeTime = 2f;  // 念のための後始末

    [Header("振動")]
    [SerializeField] bool vibrateOnCritical = true;

    void OnEnable()
    {
        Target.OnDown += HandleDown;
    }

    void OnDisable()
    {
        Target.OnDown -= HandleDown;
    }

    void HandleDown(Target target)
    {
        Vector3 pos = target.transform.position;
        bool crit = target.WasCritical;

        // --- 音 ---
        // クリティカル用が未設定なら通常音で代用する
        AudioClip clip = (crit && criticalClip != null) ? criticalClip : hitClip;

        if (clip != null)
        {
            // PlayClipAtPoint は一時的な AudioSource を作って鳴らし、
            // 再生が終わると自動で片付けてくれる。単発の効果音に向く
            AudioSource.PlayClipAtPoint(clip, pos, volume);
        }

        // --- エフェクト ---
        GameObject fx = (crit && criticalEffectPrefab != null)
                      ? criticalEffectPrefab
                      : hitEffectPrefab;

        if (fx != null)
        {
            GameObject e = Instantiate(fx, pos, Quaternion.identity);
            Destroy(e, effectLifeTime);
        }

        // --- 振動 ---
        // エディタでは何も起きないので、#if で実機のときだけ有効にする
#if UNITY_ANDROID || UNITY_IOS
        if (crit && vibrateOnCritical) Handheld.Vibrate();
#endif
    }
}