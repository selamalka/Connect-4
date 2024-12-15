using MoonActive.Connect4;
using UnityEngine;

/// <summary>
/// Manages the creation, initialization, and functionality of the grid.
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private ConnectGameGrid connectGameGrid;
    [SerializeField] private int rows = 6;
    [field: SerializeField] public int Columns { get; private set; } = 7;

    private Cell[,] gridCells; // 2D array to store and track individual cells

    public void ClearGrid()
    {
        // Find and destroy all active disks in the scene
        var allDisks = FindObjectsOfType<Disk>();
        foreach (Disk disk in allDisks)
        {
            Destroy(disk.gameObject);
        }

        // Reset all cells to their initial empty state
        var allCells = FindObjectsOfType<Cell>();
        for (int i = 0; i < allCells.Length; i++)
        {
            Cell cell = allCells[i];
            Collider2D cellCollider = cell.GetComponent<Collider2D>();

            cell.SetPlayerInCell(PlayerColor.None); // Set cell to empty
            cell.GetComponent<Collider2D>().enabled = false; // Disable collider
            if (cell.Row == 0) cellCollider.enabled = true; // Enable bottom row colliders
        }
    }
    public bool CheckDraw()
    {
        // Check if every cell in the grid is occupied
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                if (GetCell(row, col).PlayerInCell == PlayerColor.None)
                {
                    return false; // Empty cell found = not a draw
                }
            }
        }

        return true; // No empty cells. it's a draw
    }
    public bool CheckWin(int row, int column, PlayerColor player)
    {
        int[,] directions = new int[,]
        {
        { 0, 1 },  // Horizontal
        { 1, 0 },  // Vertical
        { 1, 1 },  // Diagonal (\)
        { 1, -1 }  // Diagonal (/)
        };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int rowDir = directions[i, 0];
            int colDir = directions[i, 1];

            // Count consecutive cells in both directions
            int count = 1 + CountInDirection(row, column, rowDir, colDir, player)
                          + CountInDirection(row, column, -rowDir, -colDir, player);

            // A valid win must be exactly 4 cells
            if (count == 4)
            {
                return true;
            }
        }

        return false; // No win detected
    }
    private int CountInDirection(int startRow, int startCol, int rowDir, int colDir, PlayerColor player)
    {
        // Counts consecutive cells in a specific direction matching the given player
        int count = 0;
        int row = startRow + rowDir;
        int col = startCol + colDir;

        // Continue counting while in bounds and matching the player's color
        while (IsInBounds(row, col) && GetCell(row, col)?.PlayerInCell == player)
        {
            count++;
            row += rowDir;
            col += colDir;
        }

        return count; // Return total count in this direction
    }

    public void InitializeGrid(GameMode notInUse)
    {
        // Initialize grid array with dimensions matching rows and columns
        gridCells = new Cell[rows, Columns];

        // Get all child Cell components from the grid prefab
        var cellColliders = connectGameGrid.GetComponentsInChildren<Cell>();

        // Ensure the number of cell objects matches the expected grid dimensions
        if (cellColliders.Length != rows * Columns)
        {
            return;
        }

        for (int i = 0; i < cellColliders.Length; i++)
        {
            // Cache Collider2D and Cell components
            Collider2D collider = cellColliders[i].GetComponent<Collider2D>();
            Cell cell = collider.GetComponent<Cell>();

            if (cell != null)
            {
                // Calculate grid position from index
                int row = i / Columns;
                int column = i % Columns;

                // Assign cell properties: position, initial state, and reference to GridManager
                cell.SetRow(row);
                cell.SetColumn(column);
                cell.SetPlayerInCell(PlayerColor.None); // Set to empty state
                cell.SetGridManager(this);

                // Disable all cell colliders initially
                collider.enabled = false;

                // Enable colliders only for the bottom row
                if (row == 0) collider.enabled = true;

                // Add cell to the grid array for easy access
                gridCells[row, column] = cell;
            }
        }
    }
    public bool IsColumnFull(int column)
    {
        // Check if all rows in the specified column are occupied
        for (int i = 0; i < rows; i++)
        {
            if (GetCell(i, column).PlayerInCell == PlayerColor.None)
            {
                return false; // Found an empty cell
            }
        }

        return true; // All cells are filled
    }
    private bool IsInBounds(int row, int col)
    {
        // Validates if a cell is within the grid's boundaries
        return row >= 0 && row < rows && col >= 0 && col < Columns;
    }

    public int GetNextAvailableRow(int column)
    {
        // Finds the first empty row in a specified column
        for (int row = 0; row < rows; row++)
        {
            if (GetCell(row, column).PlayerInCell == PlayerColor.None)
            {
                return row; // Return first available row
            }
        }

        return -1; // Return -1 if the column is full (shouldn't happen if IsColumnFull is used)
    }
    public Cell GetCell(int row, int column)
    {
        // Returns the cell at the specified position or null if out of bounds
        if (row < 0 || row >= rows || column < 0 || column >= Columns)
            return null;

        return gridCells[row, column];
    }
}
