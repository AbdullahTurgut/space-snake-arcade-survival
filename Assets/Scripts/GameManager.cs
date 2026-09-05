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

    

    [HideInInspector] public int currentScore = 0;
    [HideInInspector] public int highScore = 0;
    private int nearMissCount = 0;

    private int lastDisplayedSec = -1;
    private int lastDisplayedBallCount = -1;
    private int lastDisplayedSegCount = -1;

    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;
        if(PlayerPrefs.HasKey("bestSurviveTime"))
            bestSurviveTime = PlayerPrefs.GetFloat("bestSurviveTime");
        else
            PlayerPrefs.SetFloat("bestSurviveTime", bestSurviveTime);

        highScore = PlayerPrefs.GetInt("highScore", 0);
    }
    void Start()
    {
        if (ballCount != 0)
            ballCount = 0;
        bestTimeText.text = "Best Time : " + PlayerPrefs.GetFloat("bestSurviveTime").ToString("00") + "s | High: " + highScore;
    }

    public void AddScore(int points)
    {
        currentScore += points;
        int currentSec = (int)survivaTime;
        if (timeText != null)
            timeText.text = "Time: " + currentSec.ToString("00") + "s | Score: " + currentScore;
    }

    public void AddNearMiss()
    {
        nearMissCount++;
        AddScore(50);
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayPickSound(1.3f, 1.4f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        survivaTime += Time.deltaTime;

        int currentSec = (int)survivaTime;
        if (currentSec != lastDisplayedSec)
        {
            if (lastDisplayedSec != -1) AddScore(10);
            lastDisplayedSec = currentSec;
            if (timeText != null)
                timeText.text = "Time: " + currentSec.ToString("00") + "s | Score: " + currentScore;
        }

        int segCount = SnakeManager.instance != null ? SnakeManager.instance.snakeBody.Count : 0;
        if (ballCount != lastDisplayedBallCount || segCount != lastDisplayedSegCount)
        {
            lastDisplayedBallCount = ballCount;
            lastDisplayedSegCount = segCount;
            if (energyBallCountText != null)
                energyBallCountText.text = "Energy: " + ballCount + " | Body: " + segCount;
        }

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
        }

        if (asteriodRespawnText != null)
        {
            float waveCountdown = Mathf.Max(0f, (astreoidSpawnTime + spawnInterval) - survivaTime);
            bool canBoost = SnakeManager.instance != null && SnakeManager.instance.snakeBody.Count > 2;
            string boostHint = canBoost ? "<color=#00FFAA>[SPACE] Boost Ready</color>" : "<color=#AAAAAA>[Need 2+ segments]</color>";
            asteriodRespawnText.text = "Wave: " + waveCountdown.ToString("0.0") + "s  " + boostHint;
        }

        if(survivaTime <= 0)
        {
            survivaTime = 0;
        }

        if (endingPanel != null && endingPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return))
            {
                Replay();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                MainMenu();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            PauseMenu();
        }

        if (SnakeHeadScript.Instance != null && SnakeHeadScript.Instance.endingBool)
        {
            SnakeHeadScript.Instance.endingBool = false;
            if (survivaTime > bestSurviveTime)
            {
                bestSurviveTime = survivaTime;
                PlayerPrefs.SetFloat("bestSurviveTime", bestSurviveTime);
            }
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("highScore", highScore);
            }
            PlayerPrefs.Save();
            joystickPanel.SetActive(false);
            playSceneFbx.Stop();
            EndingGame();
        }
    }

    private bool isGameOver = false;

    public void TriggerHitPause(float duration = 0.04f)
    {
        if (!isGameOver && Panel != null && !Panel.activeSelf)
        {
            StartCoroutine(HitPauseRoutine(duration));
        }
    }

    private System.Collections.IEnumerator HitPauseRoutine(float duration)
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(duration);
        if (!isGameOver && Panel != null && !Panel.activeSelf)
        {
            Time.timeScale = 1f;
        }
    }

    public void EndingGame()
    {
        isGameOver = true;
        canvas.sortingOrder = 1;
        pauseBtn.interactable = false;
        endingPanel.SetActive(true);
        endingPanel.GetComponent<AudioSource>().Play();
        Time.timeScale = 0;
        newSurviveTimeText.text = "SURVIVED: " + survivaTime.ToString("00") + "s | SCORE: " + currentScore;
        BestSurviveTimeText.text = "BEST: " + PlayerPrefs.GetFloat("bestSurviveTime").ToString("00") + "s | HIGH: " + highScore;
    }


    public void AstreoidSpawn()
    {
        Vector2 randomPosAstreoid = new Vector2(Random.Range(-14f, 14f), Random.Range(4f, 6f));
        if (astreoidPrefab != null && astreoidPrefab.Count > 0)
        {
            int prefabIndex = Random.Range(0, astreoidPrefab.Count);
            GameObject astroid = Instantiate(astreoidPrefab[prefabIndex], randomPosAstreoid, Quaternion.identity);
            DestroyAsteroid script = astroid.GetComponent<DestroyAsteroid>();
            if (script != null && Random.value < 0.25f)
            {
                script.SetAsComet();
            }
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
