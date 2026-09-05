using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHeadScript : MonoBehaviour
{
    private static SnakeHeadScript instance;
    public static SnakeHeadScript Instance { get { return instance; } }

    public bool endingBool = false;
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "Wall")
        {
            endingBool = true;
        }
    }
}
