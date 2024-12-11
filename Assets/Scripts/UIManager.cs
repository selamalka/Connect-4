using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject board;
    [SerializeField] private GameMode gameMode;

    public static event Action<GameMode> OnConfirmPressed;
    public static event Action OnRestartPressed;

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
