using System.Collections;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] Camera arCamera;
    [SerializeField] GameObject targetPrefab;

    [Header("Wave shape")]
    [SerializeField] int count = 5;
    [SerializeField] float distance = 3f;        // metres in front of the camera
    [SerializeField] float spread = 1.2f;        // half-width of the row, in metres
    [SerializeField] float heightJitter = 0.3f;  // random up/down offset per target

    [Header("Timing")]
    [SerializeField] float startDelay = 1.5f;    // let AR tracking settle first

    void Awake()
    {
        if (arCamera == null) arCamera = Camera.main;
    }

    IEnumerator Start()
    {
        yield return new WaitForSeconds(startDelay);
        SpawnWave();
    }

    // Public so a UI button can call it to re-stock the targets.
    public void SpawnWave()
    {
        if (arCamera == null || targetPrefab == null)
        {
            Debug.LogError("TargetSpawner: arCamera or targetPrefab is not set.");
            return;
        }

        Transform cam = arCamera.transform;

        // Flatten the view direction onto the horizontal plane, so the row
        // stands level no matter how the phone happens to be tilted.
        Vector3 forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up);
        if (forward.sqrMagnitude < 0.0001f) forward = Vector3.forward;
        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);
        Vector3 centre = cam.position + forward * distance;

        for (int i = 0; i < count; i++)
        {
            // -1 .. +1 across the row (a single target sits dead centre).
            float t = count == 1 ? 0f : (i / (float)(count - 1)) * 2f - 1f;

            Vector3 pos = centre
                        + right * (t * spread)
                        + Vector3.up * Random.Range(-heightJitter, heightJitter);

            // Face the targets back towards the player.
            Instantiate(targetPrefab, pos, Quaternion.LookRotation(-forward, Vector3.up));
        }

        Debug.Log($"[TargetSpawner] spawned {count} targets around {centre:F2}");
    }
}
