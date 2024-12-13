using UnityEngine;

public class DiskIdentifier : MonoBehaviour
{
    [field: SerializeField] public PlayerColor PlayerColor { get; private set; }

    public void SetPlayerColor(PlayerColor playerColor) { PlayerColor = playerColor; }
}
