using UnityEngine;

/// <summary>
/// ‹…‚ğÕ‚éáŠQ•¨B“–‚½‚Á‚½‹…‚ğÁ‚µ‚ÄA“]‚ª‚è‘±‚¯‚é‚Ì‚ğ–h‚®B
/// </summary>
public class Obstacle : MonoBehaviour
{
    [SerializeField] float destroyDelay = 0.1f;   // “–‚½‚Á‚½‹…‚ğÁ‚·‚Ü‚Å‚Ì—P—\

    void OnCollisionEnter(Collision collision)
    {
        Projectile p = collision.collider.GetComponentInParent<Projectile>();
        if (p == null) return;

        // •¨—ŒvZ‚ÌÅ’†‚ÉÁ‚·‚Æ‹““®‚ª•sˆÀ’è‚É‚È‚é‚Ì‚ÅA
        // ­‚µ‚¾‚¯’x‚ç‚¹‚Ä‚©‚çÁ‚·
        Destroy(p.gameObject, destroyDelay);
    }
}