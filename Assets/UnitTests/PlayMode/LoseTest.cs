using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class LoseTest
{
    private GameObject gridManagerObject; 
    private GridManager gridManager;
    private const int Rows = 6;
    private const int Columns = 7;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Create the GridManager GameObject and attach the GridManager component.
        gridManagerObject = new GameObject("GridManager");
        gridManager = gridManagerObject.AddComponent<GridManager>();

        // Add a mocked ConnectGameGridTester to simulate grid creation.
        var connectGameGridTester = gridManagerObject.AddComponent<ConnectGameGridTester>();
        SetupMockGridCreator(connectGameGridTester);

        // Initialize a mock grid for testing.
        InitializeMockGrid();

        yield return null; // Allow Unity to process setup operations.
    }

    [UnityTearDown]
    public IEnumerator Teardown()
    {
        // Destroy the GridManager GameObject after the test to clean up.
        Object.DestroyImmediate(gridManagerObject);
        yield return null; // Allow Unity to process cleanup operations.
    }

    private void SetupMockGridCreator(ConnectGameGridTester connectGameGridTester)
    {
        // Create a mock GridCreatorTester to simulate the grid logic.
        var gridCreatorObject = new GameObject("MockGridCreator");
        var mockGridCreator = gridCreatorObject.AddComponent<GridCreatorTester>();

        // Generate mock colliders for grid cells.
        var mockColliders = new List<Collider2D>();
        for (int i = 0; i < Rows * Columns; i++)
        {
            var cellObject = new GameObject($"CellCollider[{i}]"); // Name each collider uniquely.
            var collider = cellObject.AddComponent<BoxCollider2D>(); // Add a BoxCollider2D.
            mockColliders.Add(collider); // Add collider to the list of mock colliders.
        }

        // Generate mock spawners for each column.
        var mockSpawners = new List<DisksSpawnerTester>();
        for (int i = 0; i < Columns; i++)
        {
            var spawnerObject = new GameObject($"DiskSpawner[{i}]"); // Name each spawner uniquely.
            var spawner = spawnerObject.AddComponent<DisksSpawnerTester>(); // Add a DisksSpawnerTester.
            mockSpawners.Add(spawner); // Add spawner to the list of mock spawners.
        }

        // Assign mock colliders and spawners to the GridCreatorTester.
        mockGridCreator.MockColliders = mockColliders;
        mockGridCreator.MockSpawners = mockSpawners;

        // Link the mock GridCreatorTester to the ConnectGameGridTester.
        connectGameGridTester.SetGridCreator(mockGridCreator);
    }

    private void InitializeMockGrid()
    {
        // Access and initialize the private gridCells array in GridManager.
        var gridCellsField = typeof(GridManager).GetField("gridCells", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        Cell[,] mockGridCells = new Cell[Rows, Columns]; // Create a 2D array to hold mock cells.
        gridCellsField.SetValue(gridManager, mockGridCells);

        // Populate the grid with mock Cell objects.
        for (int row = 0; row < Rows; row++)
        {
            for (int col = 0; col < Columns; col++)
            {
                var cellObject = new GameObject($"Cell[{row},{col}]"); // Name each cell uniquely.
                var cell = cellObject.AddComponent<Cell>(); // Add a Cell component.
                cell.SetRow(row); // Assign the row index to the cell.
                cell.SetColumn(col); // Assign the column index to the cell.
                cell.SetPlayerInCell(PlayerColor.None); // Initialize the cell as empty.
                mockGridCells[row, col] = cell; // Add the cell to the grid array.
            }
        }
    }

    [UnityTest]
    public IEnumerator CheckLoseDiagonal()
    {
        PlayerColor player1 = PlayerColor.Blue; // Define the player color for the test.

        // Set up diagonal cells for Player 1.
        for (int i = 0; i < 4; i++)
        {
            gridManager.GetCell(i, i).SetPlayerInCell(player1); // Place Player 1's disks diagonally.
        }
        gridManager.GetCell(1, 1).SetPlayerInCell(PlayerColor.None); // Break the diagonal streak.

        LogGridState(); // Log the current grid state for debugging.

        // Assert that no diagonal win is detected for Player 1.
        Assert.IsFalse(gridManager.CheckWin(0, 0, player1), "False positive win detected for Player 1 diagonally!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckLoseHorizontal()
    {
        PlayerColor player1 = PlayerColor.Blue; // Define the player color for the test.

        // Set up horizontal cells for Player 1.
        for (int i = 0; i < 4; i++)
        {
            gridManager.GetCell(0, i).SetPlayerInCell(player1); // Place Player 1's disks horizontally.
        }
        gridManager.GetCell(0, 2).SetPlayerInCell(PlayerColor.None); // Break the horizontal streak.

        LogGridState(); // Log the current grid state for debugging.

        // Assert that no horizontal win is detected for Player 1.
        Assert.IsFalse(gridManager.CheckWin(0, 0, player1), "False positive win detected for Player 1 horizontally!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckLoseVertical()
    {
        PlayerColor player1 = PlayerColor.Blue; // Define the player color for the test.

        // Set up vertical cells for Player 1.
        for (int i = 0; i < 4; i++)
        {
            gridManager.GetCell(i, 0).SetPlayerInCell(player1); // Place Player 1's disks vertically.
        }
        gridManager.GetCell(2, 0).SetPlayerInCell(PlayerColor.None); // Break the vertical streak.

        LogGridState(); // Log the current grid state for debugging.

        // Assert that no vertical win is detected for Player 1.
        Assert.IsFalse(gridManager.CheckWin(0, 0, player1), "False positive win detected for Player 1 vertically!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckLoseAntiDiagonal()
    {
        PlayerColor player1 = PlayerColor.Blue; // Define the player color for the test.

        // Set up anti-diagonal cells for Player 1.
        for (int i = 0; i < 4; i++)
        {
            gridManager.GetCell(i, 3 - i).SetPlayerInCell(player1); // Place Player 1's disks anti-diagonally.
        }
        gridManager.GetCell(2, 1).SetPlayerInCell(PlayerColor.None); // Break the anti-diagonal streak.

        LogGridState(); // Log the current grid state for debugging.

        // Assert that no anti-diagonal win is detected for Player 1.
        Assert.IsFalse(gridManager.CheckWin(3, 0, player1), "False positive win detected for Player 1 anti-diagonally!");

        yield return null; // Allow Unity to process frame updates.
    }

    private void LogGridState()
    {
        // Log the current state of the grid for debugging purposes.
        for (int row = 0; row < Rows; row++)
        {
            string rowState = "";
            for (int col = 0; col < Columns; col++)
            {
                rowState += gridManager.GetCell(row, col).PlayerInCell + " "; // Add cell content to the row state.
            }
        }
    }
}
