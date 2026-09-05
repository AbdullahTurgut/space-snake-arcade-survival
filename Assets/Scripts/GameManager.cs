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
        if(PlayerPrefs.HasKey("bestSurviveTime"))
            bestSurviveTime = PlayerPrefs.GetFloat("bestSurviveTime");
        else
            PlayerPrefs.SetFloat("bestSurviveTime", bestSurviveTime);
    }
    void Start()
    {
        instance = this;
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

        if (survivaTime > (astreoidSpawnTime + 3f) && ballCount >= 0)
        {
            AstreoidSpawn();
            astreoidSpawnTime += 3f;
            asteriodRespawnText.text = "Asteroid Respawn In 3 Sec";
        }
        else if(survivaTime > (astreoidSpawnTime + 2f) && ballCount >= 10)
        {
            AstreoidSpawn();
            astreoidSpawnTime += 2f;
            asteriodRespawnText.text = "Asteroid Respawn In 2 Sec";
        }
        else if(survivaTime > (astreoidSpawnTime + 1.5f) && ballCount >= 20)
        {
            AstreoidSpawn();
            astreoidSpawnTime += 1.5f;
            asteriodRespawnText.text = "Asteroid Respawn In 1.5 Sec";
        }

        if(survivaTime <= 0)
        {
            survivaTime = 0;
        }

        if (SnakeHeadScript.Instance.endingBool)
        {
            SnakeHeadScript.Instance.endingBool = !SnakeHeadScript.Instance.endingBool;
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
        Vector2 randomPosAstreoid = new Vector2(Random.Range(-14, 14), Random.Range(4,6));
        GameObject astroid = Instantiate(astreoidPrefab[Random.Range(0,2)],randomPosAstreoid,Quaternion.identity);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
