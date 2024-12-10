using UnityEngine;

public abstract class BasePlayerController : MonoBehaviour
{
    [field: SerializeField] public PlayerColor PlayerColor { get; private set; }

    public void SetPlayerColor(PlayerColor color)
    {
        PlayerColor = color;
    }
}