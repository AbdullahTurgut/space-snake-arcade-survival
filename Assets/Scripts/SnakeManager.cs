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

    private List<MarkerParts> bodyMarkerParts = new List<MarkerParts>();

    public bool IsInvulnerable { get; private set; }
    private bool isBoosting = false;
    private float boostTimer = 0f;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.LeftShift))
        {
            TryActivateBoost();
        }

        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0f)
            {
                isBoosting = false;
                IsInvulnerable = false;
                SetSnakeColor(Color.white);
            }
            else if (boostTimer < 0.35f)
            {
                float pulse = Mathf.PingPong(Time.time * 12f, 1f);
                SetSnakeColor(Color.Lerp(Color.white, new Color(0.3f, 1f, 1f), pulse));
            }
        }
    }

    private void SetSnakeColor(Color color)
    {
        for (int i = 0; i < snakeBody.Count; i++)
        {
            if (snakeBody[i] != null)
            {
                SpriteRenderer sr = snakeBody[i].GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    sr.color = color;
                }
            }
        }
    }

    public bool TryActivateBoost()
    {
        // Sacrificing tail segment requires > 2 parts (head + at least 2 segments)
        if (snakeBody.Count > 2 && !isBoosting)
        {
            int tailIdx = snakeBody.Count - 1;
            GameObject tailPart = snakeBody[tailIdx];
            snakeBody.RemoveAt(tailIdx);
            if (tailIdx < bodyMarkerParts.Count)
            {
                bodyMarkerParts.RemoveAt(tailIdx);
            }
            if (tailPart != null)
            {
                Destroy(tailPart);
            }

            isBoosting = true;
            IsInvulnerable = true;
            boostTimer = 1.2f;
            SetSnakeColor(new Color(0.3f, 1f, 1f));

            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayPickSound(1.4f, 1.5f);
            }
            if (CameraShake.Instance != null)
            {
                CameraShake.Instance.Shake(0.12f, 0.15f);
            }
            return true;
        }
        return false;
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

        float effectiveSpeed = isBoosting ? currentSpeed * 1.75f : currentSpeed;
        if (headRb != null)
        {
            headRb.linearVelocity = snakeBody[0].transform.right * effectiveSpeed;
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
                if (snakeBody[i] == null || i - 1 >= bodyMarkerParts.Count) continue;
                MarkerParts markP = bodyMarkerParts[i - 1];
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
                if (i < bodyMarkerParts.Count)
                {
                    bodyMarkerParts.RemoveAt(i);
                }
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
            MarkerParts headMarker = snakeHead.GetComponent<MarkerParts>();
            if (!headMarker)
                headMarker = snakeHead.AddComponent<MarkerParts>();
            if (!snakeHead.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = snakeHead.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            snakeBody.Add(snakeHead);
            bodyMarkerParts.Add(headMarker);
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
            MarkerParts tempMarker = temp.GetComponent<MarkerParts>();
            if (!tempMarker)
                tempMarker = temp.AddComponent<MarkerParts>();
            if (!temp.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D rb = temp.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
            snakeBody.Add(temp);
            bodyMarkerParts.Add(tempMarker);
            bodyParts.RemoveAt(0);
            tempMarker.ClearMarkerList();
            count = 0;
        }
    }

    public void AddBodyPart(GameObject obj)
    {
        bodyParts.Add(obj);
    }
   
}
