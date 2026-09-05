using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Provides an on-screen Tactical Boost button for mobile touch players.
/// Positioned in the lower-right interactive zone with visual ready/unavailable feedback.
/// Invokes SnakeManager.instance.TryActivateBoost() ensuring zero gameplay duplication
/// and 100% parity with desktop keyboard input (Space / LeftShift).
/// </summary>
public class MobileBoostButton : MonoBehaviour
{
    [SerializeField] private Button boostButton;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text buttonLabel;

    [Header("Visual Feedback Colors")]
    [SerializeField] private Color readyColor = new Color(0f, 1f, 0.75f, 0.85f);
    [SerializeField] private Color unavailableColor = new Color(0.35f, 0.45f, 0.55f, 0.35f);
    [SerializeField] private Color boostingColor = new Color(0.3f, 0.9f, 1f, 1f);

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (boostButton == null)
            boostButton = GetComponent<Button>();

        if (buttonImage == null && boostButton != null)
            buttonImage = boostButton.GetComponent<Image>();

        if (buttonLabel == null)
            buttonLabel = GetComponentInChildren<Text>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        if (boostButton != null)
        {
            boostButton.onClick.AddListener(OnBoostClicked);
        }
    }

    public void OnBoostClicked()
    {
        if (SnakeManager.instance != null)
        {
            SnakeManager.instance.TryActivateBoost();
        }
    }

    private void Update()
    {
        // When game is paused (timeScale == 0) or game over, cleanly hide button
        bool isGameOver = GameManager.Instance != null && GameManager.Instance.isGameOver;
        if (Time.timeScale == 0f || isGameOver)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
            return;
        }

        bool isBoosting = SnakeManager.instance != null && SnakeManager.instance.IsInvulnerable;
        bool canBoost = SnakeManager.instance != null &&
                        SnakeManager.instance.snakeBody.Count > 2 &&
                        !isBoosting;

        if (boostButton != null)
        {
            boostButton.interactable = canBoost;
        }

        if (canvasGroup != null)
        {
            canvasGroup.interactable = canBoost;
            canvasGroup.blocksRaycasts = canBoost;
        }

        if (buttonImage != null)
        {
            if (isBoosting)
            {
                buttonImage.color = boostingColor;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                    canvasGroup.blocksRaycasts = false;
                }
                if (buttonLabel != null) buttonLabel.text = "BOOSTING";
            }
            else if (canBoost)
            {
                buttonImage.color = readyColor;
                if (canvasGroup != null) canvasGroup.alpha = 0.9f;
                if (buttonLabel != null) buttonLabel.text = "BOOST";
            }
            else
            {
                buttonImage.color = unavailableColor;
                if (canvasGroup != null) canvasGroup.alpha = 0.4f;
                if (buttonLabel != null) buttonLabel.text = "BOOST";
            }
        }
    }
}
