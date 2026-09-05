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

    private bool isGameOver = false;

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1f;

        // Mobile platform setup: enforce landscape auto-rotation and 60fps refresh
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.orientation = ScreenOrientation.AutoRotation;
        Application.targetFrameRate = 60;

        if (PlayerPrefs.HasKey("bestSurviveTime"))
            bestSurviveTime = PlayerPrefs.GetFloat("bestSurviveTime");
        else
            PlayerPrefs.SetFloat("bestSurviveTime", bestSurviveTime);

        highScore = PlayerPrefs.GetInt("highScore", 0);
    }

    void Start()
    {
        if (ballCount != 0)
            ballCount = 0;
        if (bestTimeText != null)
            bestTimeText.text = "Best Time : " + PlayerPrefs.GetFloat("bestSurviveTime").ToString("00") + "s | High: " + highScore;

        SetupMobileUI();
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
            string boostHint = canBoost ? "<color=#00FFAA>[SPACE / BOOST] Ready</color>" : "<color=#AAAAAA>[Need 2+ segments]</color>";
            asteriodRespawnText.text = "Wave: " + waveCountdown.ToString("0.0") + "s  " + boostHint;
        }

        if (survivaTime <= 0)
        {
            survivaTime = 0;
        }

        // Handle Android Back (KeyCode.Escape) and Keyboard shortcuts
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
        else if (Panel != null && Panel.activeSelf)
        {
            // Paused state: Back / Escape / P resumes gameplay
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                ResumeGame();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            // Active gameplay: Back / Escape / P opens Pause menu
            PauseGame();
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
            if (joystickPanel != null) joystickPanel.SetActive(false);
            if (playSceneFbx != null) playSceneFbx.Stop();
            EndingGame();
        }
    }

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
        if (canvas != null) canvas.sortingOrder = 1;
        if (pauseBtn != null) pauseBtn.interactable = false;
        if (endingPanel != null)
        {
            endingPanel.SetActive(true);
            AudioSource endAudio = endingPanel.GetComponent<AudioSource>();
            if (endAudio != null) endAudio.Play();
        }
        Time.timeScale = 0f;
        if (newSurviveTimeText != null)
            newSurviveTimeText.text = "SURVIVED: " + survivaTime.ToString("00") + "s | SCORE: " + currentScore;
        if (BestSurviveTimeText != null)
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
        if (soundSlider != null)
            soundSlider.SetActive(!soundSlider.activeSelf);
    }

    public void PauseGame()
    {
        if (isGameOver) return;
        if (Panel != null && !Panel.activeSelf)
        {
            Panel.SetActive(true);
            if (joystickPanel != null) joystickPanel.SetActive(false);
            if (canvas != null) canvas.sortingOrder = 1;
            Time.timeScale = 0f;
            if (playSceneFbx != null && playSceneFbx.isPlaying)
            {
                playSceneFbx.Pause();
            }
        }
    }

    public void ResumeGame()
    {
        if (isGameOver) return;
        if (Panel != null && Panel.activeSelf)
        {
            Panel.SetActive(false);
            if (joystickPanel != null) joystickPanel.SetActive(true);
            if (canvas != null) canvas.sortingOrder = 0;
            Time.timeScale = 1f;
            if (playSceneFbx != null)
            {
                playSceneFbx.UnPause();
            }
        }
    }

    public void PauseMenu()
    {
        if (isGameOver) return;
        if (Panel != null && Panel.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App losing focus or sent to background: pause safely
            if (!isGameOver)
            {
                PauseGame();
            }
        }
        // When returning (pauseStatus == false), preserve the pause menu to avoid instant death
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            if (!isGameOver)
            {
                PauseGame();
            }
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

    private void SetupMobileUI()
    {
        // 1. Ensure SafeArea on Pause Button
        if (pauseBtn != null && pauseBtn.GetComponent<SafeArea>() == null)
        {
            SafeArea sa = pauseBtn.gameObject.AddComponent<SafeArea>();
            sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);
        }

        // 2. Ensure SafeArea on Top HUD Texts
        if (timeText != null && timeText.GetComponent<SafeArea>() == null)
        {
            SafeArea sa = timeText.gameObject.AddComponent<SafeArea>();
            sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);
        }
        if (bestTimeText != null && bestTimeText.GetComponent<SafeArea>() == null)
        {
            SafeArea sa = bestTimeText.gameObject.AddComponent<SafeArea>();
            sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);
        }
        if (energyBallCountText != null && energyBallCountText.GetComponent<SafeArea>() == null)
        {
            SafeArea sa = energyBallCountText.gameObject.AddComponent<SafeArea>();
            sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);
        }
        if (asteriodRespawnText != null && asteriodRespawnText.GetComponent<SafeArea>() == null)
        {
            SafeArea sa = asteriodRespawnText.gameObject.AddComponent<SafeArea>();
            sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);
        }

        // 3. Ensure Mobile Boost Button is present in the lower-right interactive zone
        if (FindAnyObjectByType<MobileBoostButton>() == null && canvas != null)
        {
            CreateMobileBoostButton();
        }
    }

    private void CreateMobileBoostButton()
    {
        GameObject boostObj = new GameObject("MobileBoostButton");
        boostObj.transform.SetParent(canvas.transform, false);

        RectTransform rt = boostObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 0f);
        rt.pivot = new Vector2(1f, 0f);
        rt.anchoredPosition = new Vector2(-45f, 40f);
        rt.sizeDelta = new Vector2(140f, 70f);

        Image bgImage = boostObj.AddComponent<Image>();
        if (pauseBtn != null && pauseBtn.image != null)
        {
            bgImage.sprite = pauseBtn.image.sprite;
            bgImage.type = Image.Type.Sliced;
        }
        bgImage.color = new Color(0f, 1f, 0.75f, 0.85f);

        Button btn = boostObj.AddComponent<Button>();
        btn.targetGraphic = bgImage;
        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1f, 1f, 1f, 1f);
        cb.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        cb.disabledColor = new Color(0.35f, 0.45f, 0.55f, 0.4f);
        btn.colors = cb;

        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(boostObj.transform, false);
        RectTransform labelRt = labelObj.AddComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = Vector2.zero;
        labelRt.offsetMax = Vector2.zero;

        Text labelText = labelObj.AddComponent<Text>();
        labelText.text = "BOOST";
        if (timeText != null && timeText.font != null)
        {
            labelText.font = timeText.font;
        }
        labelText.fontSize = 22;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.color = new Color(0.05f, 0.1f, 0.15f, 1f);

        SafeArea sa = boostObj.AddComponent<SafeArea>();
        sa.SetMode(SafeArea.ConstraintMode.PaddingInsets);

        boostObj.AddComponent<MobileBoostButton>();
    }
}
