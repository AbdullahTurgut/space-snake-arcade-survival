using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource sfxAudioSource;
    [SerializeField] private AudioClip bombClip;
    [SerializeField] private AudioClip pickClip;

    private void Awake()
    {
        Instance = this;
        if (sfxAudioSource == null)
        {
            sfxAudioSource = GetComponent<AudioSource>();
            if (sfxAudioSource == null)
            {
                sfxAudioSource = gameObject.AddComponent<AudioSource>();
            }
            sfxAudioSource.playOnAwake = false;
            sfxAudioSource.spatialBlend = 0f; // Pure 2D
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 1f);
            Load();
        }
        else
        {
            Load();
        }

        if (volumeSlider != null)
        {
            AudioListener.volume = volumeSlider.value;
        }
    }

    public void ChangeVolume()
    {
        if (volumeSlider != null)
        {
            AudioListener.volume = volumeSlider.value;
            Save();
        }
    }

    public void PlayBombSound(float pitchMin = 0.9f, float pitchMax = 1.15f)
    {
        PlayClip(bombClip, pitchMin, pitchMax);
    }

    public void PlayPickSound(float pitchMin = 0.95f, float pitchMax = 1.1f)
    {
        PlayClip(pickClip, pitchMin, pitchMax);
    }

    public void PlayClip(AudioClip clip, float pitchMin = 0.95f, float pitchMax = 1.05f)
    {
        if (clip == null || sfxAudioSource == null) return;
        sfxAudioSource.pitch = Random.Range(pitchMin, pitchMax);
        sfxAudioSource.PlayOneShot(clip);
    }

    private void Load()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = PlayerPrefs.GetFloat("musicVolume", 1f);
            AudioListener.volume = volumeSlider.value;
        }
    }

    private void Save()
    {
        if (volumeSlider != null)
        {
            PlayerPrefs.SetFloat("musicVolume", volumeSlider.value);
            PlayerPrefs.Save();
        }
    }
}
