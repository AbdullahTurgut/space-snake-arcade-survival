using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAsteroid : MonoBehaviour
{
    [SerializeField] private float asteriodFallingSpeed = 5f;
    Transform randPos;

   
    private void Start()
    {
        randPos = GameManager.Instance.AstreoidFallingtransforms[Random.Range(0, 3)];
        //Destroy(this.gameObject,4f);
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            randPos.position,
            asteriodFallingSpeed * Time.deltaTime);

        transform.up = randPos.position - transform.position;

        if (transform.position == randPos.position)
            Destroy(this.gameObject);
    }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "BodyPart")
        {
            
            Destroy(this.gameObject);
            Destroy(collision.gameObject);
            GameManager.Instance.survivaTime -= 3f;
            GameManager.Instance.astreoidSpawnTime -= 3f;
        }
    }
}
