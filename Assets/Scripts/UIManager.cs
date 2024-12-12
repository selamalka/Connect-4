using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject board;
    [SerializeField] private GameObject closeButton;

    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private Text announcementText;

    public static event Action<GameMode> OnSelectGameMode;
    public static event Action<GameMode> OnConfirmPressed;
    public static event Action OnRestartPressed;
    public static Action<string> OnAnnouncement;

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

    private void Start()
    {       
        if (gameManager != null) closeButton.SetActive(gameManager.IsGameActive);
        announcementPanel.SetActive(false);
    }

    // Unity event
    public void MenuButton()
    {
        announcementPanel.SetActive(false);
        mainPanel.SetActive(true);
        closeButton.SetActive(gameManager.IsGameActive);
        board.SetActive(true);
    }

    // Unity event
    public void ConfirmButton()
    {
        mainPanel.SetActive(false);
        board.SetActive(true);
        OnConfirmPressed?.Invoke(gameManager.GameMode);
    }

    // Unity event
    public void PlayerVsPlayerButton()
    {
        OnSelectGameMode?.Invoke(GameMode.PlayerVsPlayer);
    }

    // Unity event
    public void PlayerVsComputerButton()
    {
        OnSelectGameMode?.Invoke(GameMode.PlayerVsComputer);
    }

    // Unity event
    public void RestartButton()
    {
        announcementPanel.SetActive(false);
        board.SetActive(true);
        OnRestartPressed?.Invoke();
    }

    // Unity event
    public void CloseButton()
    {
        mainPanel.SetActive(false);
    }
}
