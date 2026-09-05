using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoystick : MonoBehaviour
{
    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickVector;
    private Vector2 joystickTouchPosition;
    private Vector2 joystickOriginalPosition;
    private float joystickRadius;

    public static MovementJoystick instance;

    private Canvas parentCanvas;
    private int activePointerId = -999; // -999 indicates inactive

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        if (joystickBG != null)
        {
            joystickOriginalPosition = joystickBG.transform.position;
            RectTransform rt = joystickBG.GetComponent<RectTransform>();
            if (rt != null)
            {
                joystickRadius = rt.sizeDelta.y / 3.5f;
            }
        }
    }

    private void Update()
    {
        // Watchdog: Ensure joystick never gets stuck if pointer up event is dropped by the OS/window
        if (activePointerId != -999)
        {
            if (Input.touchSupported && Input.touchCount > 0)
            {
                bool touchStillActive = false;
                for (int i = 0; i < Input.touchCount; i++)
                {
                    Touch t = Input.GetTouch(i);
                    if (t.fingerId == activePointerId)
                    {
                        if (t.phase != TouchPhase.Ended && t.phase != TouchPhase.Canceled)
                        {
                            touchStillActive = true;
                        }
                        break;
                    }
                }
                if (!touchStillActive)
                {
                    PointerUp();
                }
            }
            else if (!Input.GetMouseButton(0))
            {
                // Mouse button released on desktop
                PointerUp();
            }
        }
    }

    public void PointerDown()
    {
        if (activePointerId != -999) return; // Already tracking a touch

        // Find initiating touch or default to mouse
        if (Input.touchSupported && Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (t.phase == TouchPhase.Began)
                {
                    activePointerId = t.fingerId;
                    break;
                }
            }
            if (activePointerId == -999)
            {
                activePointerId = Input.GetTouch(0).fingerId;
            }
        }
        else
        {
            activePointerId = -1; // Mouse pointer
        }

        Vector3 pos = Input.mousePosition;
        if (joystick != null) joystick.transform.position = pos;
        if (joystickBG != null) joystickBG.transform.position = pos;
        joystickTouchPosition = pos;
    }

    public void PointerDown(BaseEventData eData)
    {
        if (activePointerId != -999) return;

        PointerEventData ped = eData as PointerEventData;
        if (ped != null)
        {
            activePointerId = ped.pointerId;
            Vector3 pos = ped.position;
            if (joystick != null) joystick.transform.position = pos;
            if (joystickBG != null) joystickBG.transform.position = pos;
            joystickTouchPosition = pos;
        }
        else
        {
            PointerDown();
        }
    }

    public void Drag(BaseEventData eData)
    {
        PointerEventData pointerEventData = eData as PointerEventData;
        if (pointerEventData == null) return;

        // Multi-touch isolation: ignore drag events from other fingers
        if (activePointerId != -999 && pointerEventData.pointerId != activePointerId)
        {
            return;
        }

        if (activePointerId == -999)
        {
            activePointerId = pointerEventData.pointerId;
            joystickTouchPosition = pointerEventData.position;
        }

        Vector2 dragPos = pointerEventData.position;
        joystickVector = (dragPos - joystickTouchPosition).normalized;

        float joystickDistance = Vector2.Distance(dragPos, joystickTouchPosition);
        float scale = parentCanvas != null ? parentCanvas.scaleFactor : 1f;
        float effectiveRadius = joystickRadius * scale;

        if (joystick != null)
        {
            if (joystickDistance < effectiveRadius)
            {
                joystick.transform.position = joystickTouchPosition + joystickVector * joystickDistance;
            }
            else
            {
                joystick.transform.position = joystickTouchPosition + joystickVector * effectiveRadius;
            }
        }
    }

    public void PointerUp()
    {
        activePointerId = -999;
        joystickVector = Vector2.zero;
        if (joystick != null) joystick.transform.position = joystickOriginalPosition;
        if (joystickBG != null) joystickBG.transform.position = joystickOriginalPosition;
    }

    public void PointerUp(BaseEventData eData)
    {
        PointerEventData ped = eData as PointerEventData;
        if (ped != null && activePointerId != -999 && ped.pointerId != activePointerId)
        {
            // Ignore pointer up from unrelated touch (e.g. boost button release)
            return;
        }
        PointerUp();
    }

    private void OnDisable()
    {
        PointerUp();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PointerUp();
        }
    }
}
