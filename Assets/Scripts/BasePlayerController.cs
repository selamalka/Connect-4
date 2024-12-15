using UnityEngine;

/// <summary>
/// Abstract base class for player controllers
/// </summary>
public abstract class BasePlayerController : MonoBehaviour
{
    [field: SerializeField] public int PlayerIndex { get; private set; }
    [field: SerializeField] public PlayerColor PlayerColor { get; private set; }

    public void SetPlayerIndex(int index) { PlayerIndex = index; }    
    public void SetPlayerColor(PlayerColor color) { PlayerColor = color; }

    // Abstract method for making a move
    public abstract void MakeMove();
}
