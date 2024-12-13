using DG.Tweening;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject closeButton;
    [SerializeField] private GameObject choosePlayerPanel;
    [SerializeField] private GameObject header;

    [SerializeField] private Image playerVsPlayerButton;
    [SerializeField] private Image playerVsComputerButton;
    [SerializeField] private Color chosenModeColor;

    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private Text announcementText;

    public static event Action<GameMode> OnSelectGameMode;
    public static event Action<GameMode> OnConfirmPressed;
    public static event Action OnRestartPressed;
    public static Action<string> OnAnnouncement;

    private int delayTimeAfterPress = 250;

    private GameManager gameManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
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
        choosePlayerPanel.transform.localScale = Vector3.zero;
        choosePlayerPanel.SetActive(true);
        header.transform.localScale = Vector3.zero;
        header.SetActive(true);
        await Task.Delay(200);
        header.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
        await Task.Delay(100);
        choosePlayerPanel.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
    }

    private void HandleModeButtonColor()
    {
        switch (gameManager.GameMode)
        {
            case GameMode.PlayerVsPlayer:                
                playerVsPlayerButton.DOColor(chosenModeColor, 0.2f).SetEase(Ease.OutQuad);
                playerVsComputerButton.DOColor(Color.white, 0.2f).SetEase(Ease.OutQuad);
                break;

            case GameMode.PlayerVsComputer:
                playerVsComputerButton.DOColor(chosenModeColor, 0.2f).SetEase(Ease.OutQuad);
                playerVsPlayerButton.DOColor(Color.white, 0.2f).SetEase(Ease.OutQuad);
                break;

            default:
                break;
        }
    }

    // Unity event
    public async void MenuButton()
    {
        await Task.Delay(delayTimeAfterPress);
        announcementPanel.SetActive(false);
        mainPanel.SetActive(true);
        closeButton.SetActive(gameManager.IsGameActive);
        board.SetActive(true);
    }

    // Unity event
    public async void ConfirmButton()
    {
        await Task.Delay(delayTimeAfterPress);
        mainPanel.SetActive(false);
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
        mainPanel.SetActive(false);
    }
}
