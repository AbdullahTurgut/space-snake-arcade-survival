using UnityEngine;

/// <summary>
/// Dynamically adjusts the camera's orthographic size to guarantee the entire
/// 16:9 gameplay arena (35.56 x 20 units) remains fully framed and visible across
/// all aspect ratios (16:10, 21:9, 16:9, etc.) without altering gameplay physics.
/// </summary>
[RequireComponent(typeof(Camera))]
public class ArenaCameraFramer : MonoBehaviour
{
    [SerializeField] private float referenceAspect = 16f / 9f; // 1.777778
    [SerializeField] private float referenceOrthoSize = 10f;

    private Camera targetCamera;
    private float lastScreenWidth;
    private float lastScreenHeight;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        ApplyFraming();
    }

    private void LateUpdate()
    {
        // Only recalculate when screen dimensions change
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            ApplyFraming();
        }
    }

    public void ApplyFraming()
    {
        if (targetCamera == null) return;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float currentAspect = (float)Screen.width / Mathf.Max(1, Screen.height);

        if (currentAspect < referenceAspect)
        {
            // Screen is narrower than 16:9 (e.g. 16:10, 4:3, 5:4)
            // Scale orthographic size so full arena width (35.56 units) remains visible
            targetCamera.orthographicSize = referenceOrthoSize * (referenceAspect / currentAspect);
        }
        else
        {
            // Screen is 16:9 or wider (e.g. 21:9, 32:9)
            // Keep arena height (20 units) fixed; arena remains centered horizontally
            targetCamera.orthographicSize = referenceOrthoSize;
        }
    }
}
