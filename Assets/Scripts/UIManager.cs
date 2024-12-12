using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameMode gameMode;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject board;

    [SerializeField] private GameObject announcementPanel;
    [SerializeField] private Text announcementText;

    public static event Action<GameMode> OnConfirmPressed;
    public static event Action OnRestartPressed;
    public static Action<string> OnAnnouncement;

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
        announcementPanel.SetActive(true);
        announcementText.text = announcement;
    }

    // Unity event
    public void MenuButton()
    {
        mainPanel.SetActive(true);
    }

    // Unity event
    public void ConfirmButton()
    {
        mainPanel.SetActive(false);
        board.SetActive(true);
        OnConfirmPressed?.Invoke(gameMode);
    }

    // Unity event
    public void PlayerVsPlayerButton()
    {
        gameMode = GameMode.PlayerVsPlayer;
    }

    // Unity event
    public void PlayerVsComputerButton()
    {
        gameMode = GameMode.PlayerVsComputer;
    }

    // Unity event
    public void RestartButton()
    {
        board.SetActive(true);
        OnRestartPressed?.Invoke();
    }

    // Unity event
    public void CloseButton()
    {
        mainPanel.SetActive(false);
    }
}
