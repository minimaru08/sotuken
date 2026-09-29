using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;   // ★追加

public class Shooter : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float muzzleDistance = 0.3f;
    [SerializeField] float shootSpeed = 8f;
    [SerializeField] float fireInterval = 0.2f;

    float lastShotTime = -999f;

    void OnEnable()
    {
        // ★実機のタッチ入力を有効化
        EnhancedTouchSupport.Enable();
    }

    void OnDisable()
    {
        EnhancedTouchSupport.Disable();
    }

    void Update()
    {
        if (!IsPressedThisFrame()) return;
        if (IsPointerOverUI()) return;
        if (Time.time - lastShotTime < fireInterval) return;

        Shoot();
        lastShotTime = Time.time;
    }

    bool IsPressedThisFrame()
    {
        // 実機：タッチ
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].began) return true;
        }

        // エディタ：マウス
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    void Shoot()
    {
        Vector2 center = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        Ray ray = arCamera.ScreenPointToRay(center);

        Vector3 spawnPos = ray.origin + ray.direction * muzzleDistance;
        GameObject ball = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        if (ball.TryGetComponent(out Rigidbody rb))
            rb.linearVelocity = ray.direction * shootSpeed;
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject();
    }
}