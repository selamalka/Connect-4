using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject board;
    [SerializeField] private GameMode gameMode;

    // Unity event
    public void MenuButton()
    {
        mainPanel.SetActive(true);
    }

    // Unity event
    public void ConfirmButton()
    {

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

    }

    // Unity event
    public void CloseButton()
    {
        mainPanel.SetActive(false);
    }
}
