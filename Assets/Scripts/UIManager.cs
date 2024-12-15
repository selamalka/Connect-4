using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages everything UI related
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject choosePlayerPanel;
    [SerializeField] private GameObject header;
    [SerializeField] private GameObject background;
    [SerializeField] private GameObject buttonPanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject settingsButton;
    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private Text announcementText;

    [Header("Game Modes")]
    [SerializeField] private Image playerVsPlayerButton;
    [SerializeField] private Image playerVsComputerButton;
    [SerializeField] private Image computerVsComputerButton;
    [SerializeField] private Color chosenModeColor;

    public static Action<GameMode> OnSelectGameMode;
    public static event Action<GameMode> OnConfirmPressed;
    public static event Action OnRestart;
    public static event Action OnSaveSettings;
    public static event Action OnCancelSettings;
    public static event Action OnDefaultSettings;
    public static Action<string> OnAnnouncement;

    private int delayTimeAfterPress = 250;

    private GameManager GameManager;

    private void Awake()
    {
        InitializeGameManager();
        if (GameManager != null)
        {
            ResetTransformScales();
            DeactivateGameObjects();
        }
    }

    private void OnEnable()
    {
        OnAnnouncement += OnAnnouncementAction;
    }

    private void OnDisable()
    {
        OnAnnouncement -= OnAnnouncementAction;
    }

    private async void Start()
    {
        if (GameManager != null && closeButton != null)
        {
            closeButton.SetActive(GameManager.IsGameActive);
        }

        if (GameManager != null)
        {
            announcementPanel.SetActive(false); // Hide the announcement panel initially
            HandleModeButtonColor(); // Adjust button colors based on the game mode
            await AnimateMenuIn(); // Play the menu animation asynchronously
            buttonPanel.SetActive(true); // Show the button panel after animation
            board.SetActive(true); // Show the game board after animation
        }
    }

    private void InitializeGameManager()
    {
        GameManager = FindObjectOfType<GameManager>();
    }
    private void ResetTransformScales()
    {
        // Reset the scale of various UI elements to Vector3.zero for animations
        if (background != null) background.transform.localScale = Vector3.zero;
        if (header != null) header.transform.localScale = Vector3.zero;
        if (choosePlayerPanel != null) choosePlayerPanel.transform.localScale = Vector3.zero;
        if (announcementPanel != null) announcementPanel.transform.localScale = Vector3.zero;
        if (announcementText != null) announcementText.transform.localScale = Vector3.zero;
        if (settingsPanel != null) settingsPanel.transform.localScale = Vector3.zero;
    }
    private void DeactivateGameObjects()
    {
        // Deactivate various UI elements to hide them from view
        if (buttonPanel != null) buttonPanel.SetActive(false);
        if (board != null) board.SetActive(false);
        if (header != null) header.SetActive(false);
        if (choosePlayerPanel != null) choosePlayerPanel.SetActive(false);
    }
    private void HandleModeButtonColor()
    {
        // Reset all buttons to white first
        ResetAllButtonsColors();

        // Highlight the selected mode's button
        switch (GameManager.GameMode)
        {
            case GameMode.PlayerVsPlayer:
                playerVsPlayerButton.color = chosenModeColor;
                break;

            case GameMode.PlayerVsComputer:
                playerVsComputerButton.color = chosenModeColor;
                break;

            case GameMode.ComputerVsComputer:
                computerVsComputerButton.color = chosenModeColor;
                break;

            default:
                break;
        }
    }
    public void SelectGameMode(GameMode mode)
    {
        OnSelectGameMode?.Invoke(mode);
    }
    private void OnAnnouncementAction(string announcement)
    {
        board.SetActive(false);
        announcementPanel.SetActive(true);
        announcementText.text = announcement;
        announcementPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack)
            .OnComplete(() => announcementText.transform.DOScale(1, 0.3f).SetEase(Ease.OutQuart));
    }

    private async Task AnimateMenuIn()
    {
        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu In 3");

        background.transform.DOScale(1, 0.4f).SetEase(Ease.OutBack);

        await Task.Delay(400);
        header.SetActive(true);
        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu In 1");
        header.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
        await Task.Delay(400);

        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu In 2");

        choosePlayerPanel.SetActive(true);
        choosePlayerPanel.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
    }
    private async Task AnimateMenuOut()
    {
        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu Out 1");
        choosePlayerPanel.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        await Task.Delay(200);
        choosePlayerPanel.SetActive(false);

        header.transform.DOScale(0f, 0.2f).SetEase(Ease.InBack);
        await Task.Delay(200);
        header.SetActive(false);

        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu Out 2");

        background.transform.DOScale(0f, 0.3f).SetEase(Ease.InBack);
        await Task.Delay(300);
    }

    private void ResetAllButtonsColors()
    {
        playerVsPlayerButton.color = Color.white;
        playerVsComputerButton.color = Color.white;
        computerVsComputerButton.color = Color.white;
    }

    // Unity event
    public void SettingsButton()
    {
        if (!settingsPanel.activeInHierarchy)
        {
            AudioManager.Instance.PlayAudio(AudioType.UI, "Menu In 1");
            settingsPanel.SetActive(true);
            settingsPanel.transform.DOScale(1, 0.3f).SetEase(Ease.OutBack);
            OnCancelSettings?.Invoke();
        }
        else
        {
            settingsPanel.transform.DOScale(0, 0.2f).SetEase(Ease.OutSine).OnComplete(() => settingsPanel.SetActive(false));
            AudioManager.Instance.PlayAudio(AudioType.UI, "Menu Out 1");
        }
    }

    // Unity event
    public void DefaultSettingsButton()
    {
        OnDefaultSettings?.Invoke();
    }

    // Unity event
    public void SaveSettingsButton()
    {
        OnSaveSettings?.Invoke();
        settingsPanel.transform.DOScale(0, 0.2f).SetEase(Ease.OutSine).OnComplete(() => settingsPanel.SetActive(false));
        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu Out 1");
    }

    // Unity event
    public void CancelSettingsButton()
    {
        OnCancelSettings?.Invoke();
        settingsPanel.transform.DOScale(0, 0.2f).SetEase(Ease.OutSine).OnComplete(() => settingsPanel.SetActive(false));
        AudioManager.Instance.PlayAudio(AudioType.UI, "Menu Out 1");
    }

    // Unity event
    public async void MenuButton()
    {
        GameManager.SetIsGamePaused(true);
        await Task.Delay(delayTimeAfterPress);
        announcementPanel.SetActive(false);
        await AnimateMenuIn();
        closeButton.SetActive(GameManager.IsGameActive);
        board.SetActive(true);
    }

    // Unity event
    public async void ConfirmButton()
    {
        GameManager.SetIsGamePaused(false);
        AudioManager.Instance.PlayAudio(AudioType.UI, "Start Game");
        await Task.Delay(delayTimeAfterPress);
        await AnimateMenuOut();
        board.SetActive(true);
        OnConfirmPressed?.Invoke(GameManager.GameMode);
    }

    // Unity event
    public void PlayerVsPlayerButton()
    {
        OnSelectGameMode?.Invoke(GameMode.PlayerVsPlayer);
        HandleModeButtonColor();
    }

    // Unity event
    public void PlayerVsComputerButton()
    {
        OnSelectGameMode?.Invoke(GameMode.PlayerVsComputer);
        HandleModeButtonColor();
    }

    // Unity event
    public void ComputerVsComputerButton()
    {
        OnSelectGameMode?.Invoke(GameMode.ComputerVsComputer);
        HandleModeButtonColor();
    }

    // Unity event
    public async void RestartButton()
    {
        await Task.Delay(delayTimeAfterPress);
        announcementPanel.SetActive(false);
        board.SetActive(true);
        OnRestart?.Invoke();
    }

    // Unity event
    public async void CloseButton()
    {
        GameManager.SetIsGamePaused(false);
        await Task.Delay(delayTimeAfterPress);
        await AnimateMenuOut();
    }
}
