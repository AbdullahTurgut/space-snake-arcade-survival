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

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        joystickOriginalPosition = joystickBG.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 3.5f;

    }
    public void PointerDown()
    {
        joystick.transform.position =Input.mousePosition;
        joystickBG.transform.position = Input.mousePosition;
        joystickTouchPosition = Input.mousePosition;
    }

    public void Drag(BaseEventData eData)
    {
        PointerEventData pointerEventData = eData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVector = (dragPos - joystickTouchPosition).normalized;

        float joystickDistance = Vector2.Distance(dragPos, joystickTouchPosition);

        if( joystickDistance < joystickRadius )
        {
            joystick.transform.position = joystickTouchPosition + joystickVector * joystickDistance;
        }

        else
        {
            joystick.transform.position = joystickTouchPosition + joystickVector * joystickRadius;
        }
    }

    public void PointerUp()
    {
        joystickVector = Vector2.zero;
        joystick.transform.position = joystickOriginalPosition;
        joystickBG.transform.position = joystickOriginalPosition;
    }
   
}
