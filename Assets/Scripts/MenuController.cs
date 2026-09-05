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
    }

    private void Start()
    {  
       
       
    }
    private void Update()
    {
     

    }

    public void OnMusic()
    {
        if (isMusicOn)
        {
            soundBtn.GetComponent<Image>().sprite = musicSprite[1];
            music.Stop();
        }
        else
        {
            soundBtn.GetComponent<Image>().sprite = musicSprite[0];
            music.Play();
        }
        isMusicOn = !isMusicOn;
    }

    public void goPlayScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("PlayScene");
    }
}
