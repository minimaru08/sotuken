using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

public class Shooter : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float muzzleDistance = 0.3f;
    [SerializeField] float shootSpeed = 8f;
    [SerializeField] float fireInterval = 0.2f;
    [SerializeField] bool debugLog = true;

    float lastShotTime = -999f;

    void Awake()
    {
        // Fallback so a lost inspector reference does not break the shooter.
        if (arCamera == null) arCamera = Camera.main;
    }

    void OnEnable()
    {
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
        // Device: touch
        var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].began) return true;
        }

        // Editor: mouse
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            return true;

        return false;
    }

    void Shoot()
    {
        if (arCamera == null || projectilePrefab == null)
        {
            Debug.LogError("Shooter: arCamera or projectilePrefab is not set.");
            return;
        }

        Transform cam = arCamera.transform;

        // The reticle sits at the exact centre of the screen, which is the
        // camera's forward axis. Read it straight off the transform so the
        // spawn point follows the device instead of a screen-space ray.
        Vector3 dir = cam.forward;
        Vector3 spawnPos = cam.position + dir * muzzleDistance;

        GameObject ball = Instantiate(projectilePrefab, spawnPos, cam.rotation);

        if (ball.TryGetComponent(out Rigidbody rb))
            rb.linearVelocity = dir * shootSpeed;

        if (debugLog)
            Debug.Log($"[Shooter] cam={cam.position:F2} dir={dir:F2}");
    }

    bool IsPointerOverUI()
    {
        return EventSystem.current != null
            && EventSystem.current.IsPointerOverGameObject();
    }
}
