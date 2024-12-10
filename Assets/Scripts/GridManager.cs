using MoonActive.Connect4;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private ConnectGameGrid connectGameGrid; // Reference to ConnectGameGrid
    [SerializeField] private int rows = 6;
    [SerializeField] private int columns = 7;

    private Cell[,] gridCells; // 2D array to manage Cell components

    private void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        // Ensure gridCells matches the expected grid dimensions
        gridCells = new Cell[rows, columns];

        // Retrieve the array of cell colliders from ConnectGameGrid
        var cellColliders = connectGameGrid.GetComponentsInChildren<Collider2D>();

        for (int i = 0; i < cellColliders.Length; i++)
        {
            Collider2D collider = cellColliders[i];
            Cell cell = collider.GetComponent<Cell>();

            if (cell != null)
            {
                // Calculate the row and column for this cell
                int row = i / columns;
                int column = i % columns;

                // Assign row, column, and initial state
                cell.SetRow(row);
                cell.SetColumn(column);
                cell.SetState(PlayerColor.None);
                collider.enabled = false;

                // Make sure only the bottom row's colliders will be enabled at the beginning
                if (cell.Row == 0) collider.enabled = true;

                // Add to the gridCells array for tracking
                gridCells[row, column] = cell;
            }
        }
    }

    /// <summary>
    /// Get the Cell at a specific grid position.
    /// </summary>
    public Cell GetCell(int row, int column)
    {
        if (row < 0 || row >= rows || column < 0 || column >= columns)
            return null;

        return gridCells[row, column];
    }

    /// <summary>
    /// Update the state of a specific cell.
    /// </summary>
    public void UpdateCellState(int row, int column, PlayerColor state)
    {
        Cell cell = GetCell(row, column);
        if (cell != null)
        {
            cell.SetState(state);
        }
    }
}
