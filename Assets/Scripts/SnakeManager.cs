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
    private void Start()
    {
       instance = this;
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
        if(GameManager.Instance.survivaTime > 3f)
        {
            speed += Time.deltaTime;
        }

       snakeBody[0].GetComponent<Rigidbody2D>().linearVelocity = snakeBody[0].transform.right * speed * Time.deltaTime;
        
        // TODO : Do with Joystick
        if(MovementJoystick.instance.joystickVector.x != 0)
        {
            snakeBody[0].transform.Rotate(new Vector3(0, 0, -turnSpeed * Time.deltaTime * MovementJoystick.instance.joystickVector.x));
        }

       // if(Input.GetAxis("Horizontal") != 0)
       // {
       //     snakeBody[0].transform.Rotate(new Vector3(0, 0, -turnSpeed * Time.deltaTime * Input.GetAxis("Horizontal")));
       // }



        if(snakeBody.Count > 1)
        {
            for(int i = 1; i < snakeBody.Count; i++)
            {
                MarkerParts markP = snakeBody[i-1].GetComponent<MarkerParts>();
                snakeBody[i].transform.position = markP.markerList[0].pos;
                snakeBody[i].transform.rotation = markP.markerList[0].rot;
                markP.markerList.RemoveAt(0);
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

        // When the destroy one of BodyPart 
        for (int i = 0; i< snakeBody.Count; i++)
        {
            if (snakeBody[i] == null)
            {
                snakeBody.RemoveAt(i);
                i = i - 1;
            }

            if(snakeBody.Count == 1 && GameManager.Instance.survivaTime > 0.5f)
            {
                SnakeHeadScript.Instance.endingBool = true;
            }

        }
    }
    void CreateBodyPart()
    {
       if(snakeBody.Count == 0)
       {
           GameObject snakeHead = Instantiate(bodyParts[0], transform.position, transform.rotation, transform);
           if (!snakeHead.GetComponent<MarkerParts>())
               snakeHead.AddComponent<MarkerParts>();
           if (!snakeHead.GetComponent<Rigidbody2D>())
           {
               snakeHead.AddComponent<Rigidbody2D>();
               snakeHead.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
           }
           snakeBody.Add(snakeHead);
           bodyParts.RemoveAt(0);
       }

        MarkerParts markP = snakeBody[snakeBody.Count - 1].GetComponent<MarkerParts>();
        if(count == 0)
        {
            markP.ClearMarkerList();
        }
        count += Time.deltaTime;
        if(count >= distanceBetween)
        {
            GameObject temp = Instantiate(bodyParts[0], markP.markerList[0].pos, markP.markerList[0].rot, transform);
            if (!temp.GetComponent<MarkerParts>())
                temp.AddComponent<MarkerParts>();
            if (!temp.GetComponent<Rigidbody2D>())
            {
                temp.AddComponent<Rigidbody2D>();
                temp.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
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
