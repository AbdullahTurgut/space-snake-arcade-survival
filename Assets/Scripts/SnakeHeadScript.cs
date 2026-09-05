using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHeadScript : MonoBehaviour
{
    private static SnakeHeadScript instance;
    public static SnakeHeadScript Instance { get { return instance; } }

    public bool endingBool = false;
    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Wall"))
        {
            endingBool = true;
        }
    }
}
