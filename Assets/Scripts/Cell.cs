using UnityEngine;
using MoonActive.Connect4;

public class Cell : MonoBehaviour
{
    [field: SerializeField] public int Row { get; private set; }
    [field: SerializeField] public int Column { get; private set; }
    [field: SerializeField] public PlayerColor PlayerInCell { get; private set; }

    public void SetRow(int row) { Row = row; }
    public void SetColumn(int column) {  Column = column; }
    public void SetState(PlayerColor player) {  PlayerInCell = player; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Disk>())
        {            
            PlayerInCell = collision.gameObject.GetComponent<DiskIdentifier>().Color;
        }
    }
}