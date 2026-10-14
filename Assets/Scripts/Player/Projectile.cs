using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float lifeTime = 4f;

    void Start()
    {
        // Œ»À‹óŠÔ‚É‚Í•Ç‚ª‚È‚¢‚Ì‚ÅA•K‚¸õ–½‚ÅÁ‚·
        Destroy(gameObject, lifeTime);
    }
}