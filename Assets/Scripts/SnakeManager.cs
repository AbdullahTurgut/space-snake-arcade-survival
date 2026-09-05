using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    public static SnakeManager instance;

    [SerializeField] float distanceBetween = .3f;

    [SerializeField] private float speed = 280f;
    [SerializeField] private float turnSpeed = 180f;
    [SerializeField] List<GameObject> bodyParts = new List<GameObject>();
    public List<GameObject> snakeBody = new List<GameObject>();

    float count = 0;
    private float currentSpeed;
    private Rigidbody2D headRb;

    private void Awake()
    {
        instance = this;
        currentSpeed = speed > 50f ? speed * 0.02f : speed;
    }

    private void Start()
    {
        Time.timeScale = 1;
        CreateBodyPart();
    }

    private void FixedUpdate()
    {
        ManageSnakeBody();
        Movement();
    }

    // Snake Movement
    void Movement()
    {
        if (snakeBody.Count == 0 || snakeBody[0] == null) return;

        if (GameManager.Instance != null && GameManager.Instance.survivaTime > 3f)
        {
            currentSpeed += Time.fixedDeltaTime * 0.02f;
            currentSpeed = Mathf.Min(currentSpeed, 9f);
        }

        if (headRb == null)
        {
            headRb = snakeBody[0].GetComponent<Rigidbody2D>();
        }

        if (headRb != null)
        {
            headRb.linearVelocity = snakeBody[0].transform.right * currentSpeed;
        }

        // Steer with Joystick or Keyboard (A/D / Left/Right)
        float steerInput = 0f;
        if (MovementJoystick.instance != null && Mathf.Abs(MovementJoystick.instance.joystickVector.x) > 0.01f)
        {
            steerInput = MovementJoystick.instance.joystickVector.x;
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f)
        {
            steerInput = Input.GetAxis("Horizontal");
        }

        if (Mathf.Abs(steerInput) > 0.01f)
        {
            snakeBody[0].transform.Rotate(new Vector3(0, 0, -turnSpeed * Time.fixedDeltaTime * steerInput));
        }

        if (snakeBody.Count > 1)
        {
            for (int i = 1; i < snakeBody.Count; i++)
            {
                if (snakeBody[i] == null || snakeBody[i - 1] == null) continue;
                MarkerParts markP = snakeBody[i - 1].GetComponent<MarkerParts>();
                if (markP != null && markP.markerList != null && markP.markerList.Count > 0)
                {
                    snakeBody[i].transform.position = markP.markerList[0].pos;
                    snakeBody[i].transform.rotation = markP.markerList[0].rot;
                    markP.markerList.RemoveAt(0);
                }
            }
        }
    }

    // Snake Body Manage
    void ManageSnakeBody()
    {
        if (bodyParts.Count > 0)
        {
            CreateBodyPart();
        }

        // Clean up destroyed body parts
        for (int i = 0; i < snakeBody.Count; i++)
        {
            if (snakeBody[i] == null)
            {
                snakeBody.RemoveAt(i);
                i--;
            }
        }

        if (snakeBody.Count == 1 && GameManager.Instance != null && GameManager.Instance.survivaTime > 0.5f)
        {
            if (SnakeHeadScript.Instance != null)
            {
                SnakeHeadScript.Instance.endingBool = true;
            }
        }
    }

    void CreateBodyPart()
    {
        if (snakeBody.Count == 0)
        {
            GameObject snakeHead = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);
            if (!snakeHead.GetComponent<MarkerParts>())
                snakeHead.AddComponent<MarkerParts>();
            if (!snakeHead.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = snakeHead.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            snakeBody.Add(snakeHead);
            headRb = snakeHead.GetComponent<Rigidbody2D>();
            bodyParts.RemoveAt(0);
            return;
        }

        if (snakeBody.Count == 0 || snakeBody[snakeBody.Count - 1] == null) return;
        MarkerParts markP = snakeBody[snakeBody.Count - 1].GetComponent<MarkerParts>();
        if (markP == null) return;

        if (count == 0)
        {
            markP.ClearMarkerList();
        }

        count += Time.fixedDeltaTime;
        if (count >= distanceBetween && bodyParts.Count > 0)
        {
            Vector3 spawnPos = (markP.markerList != null && markP.markerList.Count > 0) ? markP.markerList[0].pos : markP.transform.position;
            Quaternion spawnRot = (markP.markerList != null && markP.markerList.Count > 0) ? markP.markerList[0].rot : markP.transform.rotation;

            GameObject temp = Instantiate(bodyParts[0], spawnPos, spawnRot, transform);
            if (!temp.GetComponent<MarkerParts>())
                temp.AddComponent<MarkerParts>();
            if (!temp.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = temp.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            snakeBody.Add(temp);
            bodyParts.RemoveAt(0);
            temp.GetComponent<MarkerParts>().ClearMarkerList();
            count = 0;
        }
    }

    public void AddBodyPart(GameObject obj)
    {
        bodyParts.Add(obj);
    }
   
}
