using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private Text announcementText;

    [Header("Game Modes")]
    [SerializeField] private Image playerVsPlayerButton;
    [SerializeField] private Image playerVsComputerButton;
    [SerializeField] private Image computerVsComputerButton;
    [SerializeField] private Color chosenModeColor;

    public static event Action<GameMode> OnSelectGameMode;
    public static event Action<GameMode> OnConfirmPressed;
    public static event Action OnRestartPressed;
    public static Action<string> OnAnnouncement;

    private int delayTimeAfterPress = 250;
    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();

        background.transform.localScale = Vector3.zero;
        header.transform.localScale = Vector3.zero;
        choosePlayerPanel.transform.localScale = Vector3.zero;

        buttonPanel.SetActive(false);
        board.SetActive(false);
        header.SetActive(false);
        choosePlayerPanel.SetActive(false);
    }

    private void OnEnable()
    {
        OnAnnouncement += OnAnnouncementAction;
    }

    private void OnDisable()
    {
        OnAnnouncement -= OnAnnouncementAction;
    }

    private void OnAnnouncementAction(string announcement)
    {
        board.SetActive(false);
        announcementPanel.SetActive(true);
        announcementText.text = announcement;
    }

    private async void Start()
    {
        if (gameManager != null) closeButton.SetActive(gameManager.IsGameActive);
        announcementPanel.SetActive(false);

        HandleModeButtonColor();

        await AnimateMenuIn();

        buttonPanel.SetActive(true);
        board.SetActive(true);
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

    private void HandleModeButtonColor()
    {
        // Reset all buttons to white first
        ResetAllButtonsColors();

        // Highlight the selected mode's button
        switch (gameManager.GameMode)
        {
            case GameMode.PlayerVsPlayer:
                playerVsPlayerButton.DOColor(chosenModeColor, 0.2f).SetEase(Ease.OutQuad);
                break;

            case GameMode.PlayerVsComputer:
                playerVsComputerButton.DOColor(chosenModeColor, 0.2f).SetEase(Ease.OutQuad);
                break;

            case GameMode.ComputerVsComputer:
                computerVsComputerButton.DOColor(chosenModeColor, 0.2f).SetEase(Ease.OutQuad);
                break;

            default:
                break;
        }
    }

    // Helper function to reset all button colors
    private void ResetAllButtonsColors()
    {
        playerVsPlayerButton.DOColor(Color.white, 0.2f).SetEase(Ease.OutQuad);
        playerVsComputerButton.DOColor(Color.white, 0.2f).SetEase(Ease.OutQuad);
        computerVsComputerButton.DOColor(Color.white, 0.2f).SetEase(Ease.OutQuad);
    }

    // Unity event
    public async void MenuButton()
    {
        await Task.Delay(delayTimeAfterPress);
        announcementPanel.SetActive(false);
        await AnimateMenuIn();
        closeButton.SetActive(gameManager.IsGameActive);
        board.SetActive(true);
    }

    // Unity event
    public async void ConfirmButton()
    {
        AudioManager.Instance.PlayAudio(AudioType.UI, "Start Game");
        await Task.Delay(delayTimeAfterPress);
        await AnimateMenuOut();
        board.SetActive(true);
        OnConfirmPressed?.Invoke(gameManager.GameMode);
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
        OnRestartPressed?.Invoke();
    }

    // Unity event
    public async void CloseButton()
    {
        await Task.Delay(delayTimeAfterPress);
        //mainPanel.SetActive(false);
        await AnimateMenuOut();
    }
}
