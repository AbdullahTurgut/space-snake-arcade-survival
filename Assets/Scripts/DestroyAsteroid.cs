using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAsteroid : MonoBehaviour
{
    [SerializeField] private float asteriodFallingSpeed = 5f;
    private Transform randPos;
    private bool hasRewardedNearMiss = false;
    private bool isComet = false;

    public void SetAsComet()
    {
        isComet = true;
        asteriodFallingSpeed = 8.5f;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1f, 0.4f, 0.2f); // Fiery comet orange
        }
        transform.localScale *= 0.85f;
    }

    private void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.AstreoidFallingtransforms != null && GameManager.Instance.AstreoidFallingtransforms.Count > 0)
        {
            int index = Random.Range(0, GameManager.Instance.AstreoidFallingtransforms.Count);
            randPos = GameManager.Instance.AstreoidFallingtransforms[index];
        }
        Destroy(gameObject, 10f);
    }

    private void Update()
    {
        if (randPos == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = Vector2.MoveTowards(
            transform.position,
            randPos.position,
            asteriodFallingSpeed * Time.deltaTime);

        Vector3 direction = randPos.position - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.up = direction;
        }

        // Near-Miss check
        if (!hasRewardedNearMiss && SnakeHeadScript.Instance != null)
        {
            float dist = Vector2.Distance(transform.position, SnakeHeadScript.Instance.transform.position);
            if (dist < 1.7f && dist > 0.6f)
            {
                hasRewardedNearMiss = true;
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddNearMiss();
                }
            }
        }

        if (Vector2.Distance(transform.position, randPos.position) <= 0.15f || transform.position.y < -12f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the snake is in tactical boost invulnerability, destroy asteroid and award bonus points
        if (SnakeManager.instance != null && SnakeManager.instance.IsInvulnerable)
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBombSound(1.1f, 1.25f);
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.15f, 0.2f);
            if (GameManager.Instance != null) GameManager.Instance.AddScore(150);
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("BodyPart"))
        {
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBombSound();
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.2f, 0.25f);
            if (GameManager.Instance != null)
            {
                GameManager.Instance.TriggerHitPause(0.04f);
                GameManager.Instance.survivaTime = Mathf.Max(0f, GameManager.Instance.survivaTime - 3f);
                GameManager.Instance.astreoidSpawnTime = Mathf.Max(0f, GameManager.Instance.astreoidSpawnTime - 3f);
            }
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            // Head collision with asteroid triggers Game Over
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBombSound(0.8f, 0.9f);
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.4f, 0.45f);
            Destroy(gameObject);
            if (SnakeHeadScript.Instance != null)
            {
                SnakeHeadScript.Instance.endingBool = true;
            }
        }
    }
}
