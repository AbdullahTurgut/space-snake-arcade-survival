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
            if (SoundManager.Instance != null) SoundManager.Instance.PlayBombSound(0.8f, 0.9f);
            if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.35f, 0.4f);
            endingBool = true;
        }
    }
}
