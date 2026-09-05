using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides subtle cosmic depth, a gentle ambient breathing glow,
/// a low-count drifting starfield / space dust particle system,
/// and subtle parallax tracking without distracting from gameplay.
/// </summary>
public class SpaceBackgroundEffects : MonoBehaviour
{
    [Header("Layer 1 - Ambient Cosmic Breathing")]
    [SerializeField] private bool enableAmbientBreathing = true;
    [SerializeField] private float breathingSpeed = 0.12f;
    private Color baseColor = Color.white;
    private Color dimColor = new Color(0.88f, 0.90f, 0.96f, 1f);

    [Header("Layer 2 - Drifting Starfield / Space Dust")]
    [SerializeField] private bool enableStarfield = true;
    [SerializeField] private int maxParticles = 35;

    [Header("Layer 3 - Subtle Parallax")]
    [SerializeField] private bool enableParallax = true;
    [SerializeField] private float parallaxFactor = 0.006f;

    private Graphic bgGraphic;
    private Vector3 initialLocalPos;
    private ParticleSystem starfieldSystem;

    private void Awake()
    {
        bgGraphic = GetComponent<Graphic>();
        if (bgGraphic != null)
        {
            baseColor = bgGraphic.color;
        }
        initialLocalPos = transform.localPosition;

        if (enableStarfield)
        {
            CreateStarfield();
        }
    }

    private void CreateStarfield()
    {
        GameObject starfieldObj = new GameObject("SpaceDustField");
        // Place in world space at Z = 1 (behind gameplay plane Z = 0, ahead of canvas plane Z = 90)
        starfieldObj.transform.position = new Vector3(0f, 0f, 1f);

        starfieldSystem = starfieldObj.AddComponent<ParticleSystem>();

        var main = starfieldSystem.main;
        main.maxParticles = maxParticles;
        main.startLifetime = new ParticleSystem.MinMaxCurve(10f, 16f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.12f, 0.28f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.06f, 0.16f);
        main.startColor = new Color(0.8f, 0.92f, 1.0f, 0.38f);
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.playOnAwake = true;
        main.loop = true;
        main.prewarm = true; // Pre-populate space so particles are already visible

        var emission = starfieldSystem.emission;
        emission.rateOverTime = 2.5f;

        var shape = starfieldSystem.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.scale = new Vector3(48f, 26f, 1f);

        var vel = starfieldSystem.velocityOverLifetime;
        vel.enabled = true;
        vel.x = new ParticleSystem.MinMaxCurve(-0.10f);
        vel.y = new ParticleSystem.MinMaxCurve(-0.18f);
        vel.z = new ParticleSystem.MinMaxCurve(0f);

        var colorOverTime = starfieldSystem.colorOverLifetime;
        colorOverTime.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.85f, 0.95f, 1f), 0f),
                new GradientColorKey(new Color(1f, 1f, 1f), 0.5f),
                new GradientColorKey(new Color(0.7f, 0.85f, 1f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0f, 0f),
                new GradientAlphaKey(0.40f, 0.25f),
                new GradientAlphaKey(0.40f, 0.75f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        colorOverTime.color = grad;

        var psRenderer = starfieldObj.GetComponent<ParticleSystemRenderer>();
        psRenderer.sortingLayerID = 0;
        psRenderer.sortingOrder = -1; // Render behind snake and asteroids (order 0)
    }

    private void Update()
    {
        // 1. Subtle ambient cosmic breathing
        if (enableAmbientBreathing && bgGraphic != null && Time.timeScale > 0)
        {
            float wave = (Mathf.Sin(Time.time * breathingSpeed) + 1f) * 0.5f;
            bgGraphic.color = Color.Lerp(dimColor, baseColor, wave);
        }

        // 2. Subtle parallax tracking snake head position
        if (enableParallax && SnakeManager.instance != null && SnakeManager.instance.snakeBody.Count > 0 && SnakeManager.instance.snakeBody[0] != null)
        {
            Vector3 headPos = SnakeManager.instance.snakeBody[0].transform.position;
            Vector3 targetOffset = new Vector3(-headPos.x * parallaxFactor * 100f, -headPos.y * parallaxFactor * 100f, 0f);
            transform.localPosition = Vector3.Lerp(transform.localPosition, initialLocalPos + targetOffset, Time.deltaTime * 1.5f);
        }
    }
}
