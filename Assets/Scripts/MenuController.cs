using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    private AudioSource music;
    [SerializeField] private Sprite[] musicSprite;
    [SerializeField] private Button soundBtn;

    private bool isMusicOn = true;

    private void Awake()
    {
        music = GetComponent<AudioSource>();

        // Mobile platform setup: enforce landscape auto-rotation and 60fps refresh
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
        Application.targetFrameRate = 60;
    }

    private void Start()
    {  
        SetupMobileUI();
    }

    private void SetupMobileUI()
    {
        if (soundBtn != null && soundBtn.GetComponent<SafeArea>() == null)
        {
            SafeArea sa = soundBtn.gameObject.AddComponent<SafeArea>();
            sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            goPlayScene();
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            OnMusic();
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OnMusic()
    {
        if (isMusicOn)
        {
            if (soundBtn != null && musicSprite != null && musicSprite.Length > 1)
                soundBtn.GetComponent<Image>().sprite = musicSprite[1];
            if (music != null) music.Stop();
        }
        else
        {
            if (soundBtn != null && musicSprite != null && musicSprite.Length > 0)
                soundBtn.GetComponent<Image>().sprite = musicSprite[0];
            if (music != null) music.Play();
        }
        isMusicOn = !isMusicOn;
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (music != null)
        {
            if (pauseStatus)
            {
                if (music.isPlaying) music.Pause();
            }
            else
            {
                if (isMusicOn) music.UnPause();
            }
        }
    }

    public void goPlayScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayScene");
    }
}
