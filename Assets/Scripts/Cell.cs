using UnityEngine;

public class Cell : MonoBehaviour
{
    [field: SerializeField] public int Row { get; private set; }
    [field: SerializeField] public int Column { get; private set; }
    [field: SerializeField] public CellState State { get; private set; }

    public void SetRow(int row) { Row = row; }
    public void SetColumn(int column) {  Column = column; }
    public void SetState(CellState state) {  State = state; }
}
