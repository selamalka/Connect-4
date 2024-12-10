using UnityEngine;
using MoonActive.Connect4;

public class Cell : MonoBehaviour
{
    [field: SerializeField] public int Row { get; private set; }
    [field: SerializeField] public int Column { get; private set; }
    [field: SerializeField] public PlayerColor PlayerInCell { get; private set; }
    private GridManager gridManager;

    public void SetRow(int row) { Row = row; }
    public void SetColumn(int column) { Column = column; }
    public void SetPlayerInCell(PlayerColor player) { PlayerInCell = player; }
    public void SetGridManager(GridManager gridManager) { this.gridManager = gridManager; }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Disk>() != null)
        {
            PlayerColor diskColor = collision.gameObject.GetComponent<DiskIdentifier>().Color;
            SetPlayerInCell(diskColor);
            ActivateCellColliderAbove();
        }
    }

    private void ActivateCellColliderAbove()
    {
        // Get the cell above
        Cell cellAbove = gridManager.GetCell(Row + 1, Column);

        // Check if the cell exists and has a collider
        if (cellAbove == null)
        {
            Debug.LogWarning($"No cell above at Row: {Row + 1}, Column: {Column}");
            return;
        }

        Collider2D cellAboveCollider = cellAbove.GetComponent<Collider2D>();
        if (cellAboveCollider == null)
        {
            Debug.LogWarning($"Cell at Row: {Row + 1}, Column: {Column} has no collider");
            return;
        }

        // Enable the collider of the cell above
        cellAboveCollider.enabled = true;
    }
}