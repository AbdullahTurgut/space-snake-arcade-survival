using UnityEngine;

/// <summary>
/// Reusable Safe Area component that conforms UI elements to Screen.safeArea,
/// preventing controls and HUD text from being obscured by notches, camera cutouts,
/// camera islands, rounded display corners, or Android navigation gesture bars.
/// 
/// Updates dynamically only when resolution, orientation, or safe area boundaries change.
/// On desktop systems, Screen.safeArea spans the full screen, preserving identical PC layout.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class SafeArea : MonoBehaviour
{
    public enum ConstraintMode
    {
        FullContainer,      // Updates anchorMin and anchorMax to match safe area normalized bounds
        PaddingInsets       // Offsets anchoredPosition for individually anchored UI widgets
    }

    [Header("Constraint Settings")]
    [SerializeField] private ConstraintMode mode = ConstraintMode.FullContainer;

    public void SetMode(ConstraintMode newMode)
    {
        mode = newMode;
        ApplySafeArea();
    }
    [SerializeField] private bool applyLeft = true;
    [SerializeField] private bool applyRight = true;
    [SerializeField] private bool applyTop = true;
    [SerializeField] private bool applyBottom = true;

    private RectTransform targetRect;
    private Rect lastSafeArea = Rect.zero;
    private Vector2Int lastScreenSize = Vector2Int.zero;
    private ScreenOrientation lastOrientation = ScreenOrientation.AutoRotation;

    private Vector2 initialAnchoredPosition;
    private bool initializedOffsets = false;

    private void Awake()
    {
        targetRect = GetComponent<RectTransform>();
        if (targetRect != null)
        {
            initialAnchoredPosition = targetRect.anchoredPosition;
            initializedOffsets = true;
        }
        ApplySafeArea();
    }

    private void OnEnable()
    {
        ApplySafeArea();
    }

    private void LateUpdate()
    {
        // Only recalculate when safe area, screen size, or orientation actually changes
        if (Screen.safeArea != lastSafeArea ||
            Screen.width != lastScreenSize.x ||
            Screen.height != lastScreenSize.y ||
            Screen.orientation != lastOrientation)
        {
            ApplySafeArea();
        }
    }

    public void ApplySafeArea()
    {
        if (targetRect == null) return;

        Rect safeArea = Screen.safeArea;
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        if (screenWidth <= 0 || screenHeight <= 0) return;

        lastSafeArea = safeArea;
        lastScreenSize = new Vector2Int(screenWidth, screenHeight);
        lastOrientation = Screen.orientation;

        if (mode == ConstraintMode.FullContainer)
        {
            Vector2 min = safeArea.position;
            Vector2 max = safeArea.position + safeArea.size;

            float minX = applyLeft ? min.x / screenWidth : 0f;
            float minY = applyBottom ? min.y / screenHeight : 0f;
            float maxX = applyRight ? max.x / screenWidth : 1f;
            float maxY = applyTop ? max.y / screenHeight : 1f;

            targetRect.anchorMin = new Vector2(minX, minY);
            targetRect.anchorMax = new Vector2(maxX, maxY);
            targetRect.offsetMin = Vector2.zero;
            targetRect.offsetMax = Vector2.zero;
        }
        else if (mode == ConstraintMode.PaddingInsets)
        {
            if (!initializedOffsets)
            {
                initialAnchoredPosition = targetRect.anchoredPosition;
                initializedOffsets = true;
            }

            Canvas rootCanvas = GetComponentInParent<Canvas>();
            float scale = (rootCanvas != null && rootCanvas.scaleFactor > 0f) ? rootCanvas.scaleFactor : 1f;

            float insetLeft = applyLeft ? (safeArea.xMin / scale) : 0f;
            float insetRight = applyRight ? ((screenWidth - safeArea.xMax) / scale) : 0f;
            float insetBottom = applyBottom ? (safeArea.yMin / scale) : 0f;
            float insetTop = applyTop ? ((screenHeight - safeArea.yMax) / scale) : 0f;

            Vector2 newPos = initialAnchoredPosition;

            // Shift horizontal position based on widget's anchor affinity
            if (targetRect.anchorMin.x >= 0.8f)
            {
                // Right-anchored: push left away from right cutout
                newPos.x = initialAnchoredPosition.x - insetRight;
            }
            else if (targetRect.anchorMax.x <= 0.2f)
            {
                // Left-anchored: push right away from left cutout
                newPos.x = initialAnchoredPosition.x + insetLeft;
            }

            // Shift vertical position based on widget's vertical anchor affinity
            if (targetRect.anchorMin.y >= 0.8f)
            {
                // Top-anchored: push down away from top notch/status bar
                newPos.y = initialAnchoredPosition.y - insetTop;
            }
            else if (targetRect.anchorMax.y <= 0.2f)
            {
                // Bottom-anchored: push up away from home indicator / navigation bar
                newPos.y = initialAnchoredPosition.y + insetBottom;
            }

            targetRect.anchoredPosition = newPos;
        }
    }
}
