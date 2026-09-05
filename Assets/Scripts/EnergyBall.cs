using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBall : MonoBehaviour
{
    [SerializeField] private BoxCollider2D gridArea;
    [SerializeField] GameObject bodyPartObj;
    private AudioSource pickSound;
    
    private void Start()
    {
        pickSound = GetComponent<AudioSource>();
        RandomPositionOfEnergyBall();
    }
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, 1), 180f * Time.deltaTime);
    }
    private void RandomPositionOfEnergyBall()
    {
        Bounds bounds = this.gridArea.bounds;

        float x = Random.Range(bounds.min.x, bounds.max.x);
        float y = Random.Range(bounds.min.y, bounds.max.y);

        this.transform.position = new Vector3(Mathf.Round(x), Mathf.Round(y), 0);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Player")
        {
            pickSound.Play();
            GameManager.Instance.ballCount += 1;
            GameManager.Instance.survivaTime += 1;
            RandomPositionOfEnergyBall();
            SnakeManager.instance.AddBodyPart(bodyPartObj);
        }
    }
}
