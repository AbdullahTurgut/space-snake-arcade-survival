using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    [HideInInspector]
    public float survivaTime = 0f;
    [HideInInspector]
    public float bestSurviveTime;

    public Text timeText;
    public Text bestTimeText;
    public Text newSurviveTimeText;
    public Text BestSurviveTimeText;
    public Canvas canvas;

    [SerializeField] private GameObject Panel;
    [SerializeField] private GameObject endingPanel;

    public List<GameObject> astreoidPrefab = new List<GameObject>();
    
    public float astreoidSpawnTime = 0;
    public List<Transform> AstreoidFallingtransforms = new List<Transform>();

    public GameObject joystickPanel;
    public Button pauseBtn;

    public AudioSource playSceneFbx;

    public GameObject soundSlider;

    public Text energyBallCountText;
    public Text asteriodRespawnText;
    [HideInInspector]
    public int ballCount = 0;

    

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        if(PlayerPrefs.HasKey("bestSurviveTime"))
            bestSurviveTime = PlayerPrefs.GetFloat("bestSurviveTime");
        else
            PlayerPrefs.SetFloat("bestSurviveTime", bestSurviveTime);
    }
    void Start()
    {
        if (ballCount != 0)
            ballCount = 0;
        bestTimeText.text = "Best Time : " + PlayerPrefs.GetFloat("bestSurviveTime").ToString("00") + " Sec";
    }

    // Update is called once per frame
    void Update()
    {
        survivaTime += Time.deltaTime;
        timeText.text = "Survive Time : " + survivaTime.ToString("00") + " Sec";
        energyBallCountText.text = "Energy Ball : " + ballCount.ToString();

        float spawnInterval = 3f;
        if (ballCount >= 20)
        {
            spawnInterval = 1.5f;
        }
        else if (ballCount >= 10)
        {
            spawnInterval = 2f;
        }

        if (survivaTime >= (astreoidSpawnTime + spawnInterval))
        {
            AstreoidSpawn();
            astreoidSpawnTime = survivaTime;
            asteriodRespawnText.text = "Asteroid Respawn In " + spawnInterval.ToString("0.0") + " Sec";
        }

        if(survivaTime <= 0)
        {
            survivaTime = 0;
        }

        if (SnakeHeadScript.Instance != null && SnakeHeadScript.Instance.endingBool)
        {
            SnakeHeadScript.Instance.endingBool = false;
            if (survivaTime > bestSurviveTime)
            {
                bestSurviveTime = survivaTime;
                PlayerPrefs.SetFloat("bestSurviveTime", bestSurviveTime);
            }
            joystickPanel.SetActive(false);
            playSceneFbx.Stop();
            EndingGame();
        }
    }

    public void EndingGame()
    {
        canvas.sortingOrder = 1;
        pauseBtn.interactable = false;
        endingPanel.SetActive(true);
        endingPanel.GetComponent<AudioSource>().Play();
        Time.timeScale = 0;
        newSurviveTimeText.text = "NEW SURVIVE TIME : " + survivaTime.ToString("00") + " Sec";
        BestSurviveTimeText.text = "BEST SURVIVE TIME : " + PlayerPrefs.GetFloat("bestSurviveTime").ToString("00") + " Sec";
    }


    public void AstreoidSpawn()
    {
        Vector2 randomPosAstreoid = new Vector2(Random.Range(-14f, 14f), Random.Range(4f, 6f));
        if (astreoidPrefab != null && astreoidPrefab.Count > 0)
        {
            int prefabIndex = Random.Range(0, astreoidPrefab.Count);
            Instantiate(astreoidPrefab[prefabIndex], randomPosAstreoid, Quaternion.identity);
        }
    }

    public void OnSoundSlider()
    {
        soundSlider.SetActive(!soundSlider.activeSelf);
    }
    public void PauseMenu()
    {
        
        Panel.SetActive(!Panel.activeSelf);
        joystickPanel.SetActive(!Panel.activeSelf);
        if (!Panel.activeSelf)
        {
            canvas.sortingOrder = 0;
            Time.timeScale = 1;
        }
        else
        {
            canvas.sortingOrder = 1;
            Time.timeScale = 0;
        }
    }
   
    public void Replay()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
