using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private static CameraShake instance;
    public static CameraShake Instance
    {
        get
        {
            if (instance == null)
            {
                Camera cam = Camera.main;
                if (cam != null)
                {
                    instance = cam.GetComponent<CameraShake>();
                    if (instance == null)
                    {
                        instance = cam.gameObject.AddComponent<CameraShake>();
                    }
                }
            }
            return instance;
        }
    }

    private Vector3 initialPos;
    private float shakeTimer;
    private float shakeIntensity;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        initialPos = transform.localPosition;
    }

    public void Shake(float duration = 0.2f, float intensity = 0.25f)
    {
        initialPos = transform.localPosition;
        shakeTimer = duration;
        shakeIntensity = intensity;
    }

    private void LateUpdate()
    {
        if (shakeTimer > 0)
        {
            Vector2 offset = Random.insideUnitCircle * shakeIntensity;
            transform.localPosition = new Vector3(initialPos.x + offset.x, initialPos.y + offset.y, initialPos.z);
            shakeTimer -= Time.unscaledDeltaTime;
            if (shakeTimer <= 0)
            {
                transform.localPosition = initialPos;
            }
        }
    }
}
