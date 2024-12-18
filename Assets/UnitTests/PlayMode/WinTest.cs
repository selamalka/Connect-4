using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;

public class WinTest
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
        for (int i = 0; i < Rows * Columns; i++) // Create enough colliders for all cells in the grid.
        {
            var cellObject = new GameObject($"CellCollider[{i}]"); // Name each collider uniquely.
            var collider = cellObject.AddComponent<BoxCollider2D>(); // Add a BoxCollider2D.
            mockColliders.Add(collider); // Add collider to the list of mock colliders.
        }

        // Generate mock spawners for each column.
        var mockSpawners = new List<DisksSpawnerTester>();
        for (int i = 0; i < Columns; i++) // Create spawners for each column.
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
    public IEnumerator CheckWinHorizontal()
    {
        PlayerColor testPlayer = PlayerColor.Blue; // Define the player color for the test.

        // Place Player 1's disks horizontally in the first row.
        for (int col = 0; col < 4; col++)
        {
            gridManager.GetCell(0, col).SetPlayerInCell(testPlayer);
        }

        // Assert that a horizontal win is detected.
        Assert.IsTrue(gridManager.CheckWin(0, 3, testPlayer), "Horizontal win was not detected!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckWinVertical()
    {
        PlayerColor testPlayer = PlayerColor.Red; // Define the player color for the test.

        // Place Player 2's disks vertically in the first column.
        for (int row = 0; row < 4; row++)
        {
            gridManager.GetCell(row, 0).SetPlayerInCell(testPlayer);
        }

        // Assert that a vertical win is detected.
        Assert.IsTrue(gridManager.CheckWin(3, 0, testPlayer), "Vertical win was not detected!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckWinDiagonal()
    {
        PlayerColor testPlayer = PlayerColor.Blue; // Define the player color for the test.

        // Place Player 1's disks diagonally from top-left to bottom-right.
        for (int i = 0; i < 4; i++)
        {
            gridManager.GetCell(i, i).SetPlayerInCell(testPlayer);
        }

        // Assert that a diagonal win is detected.
        Assert.IsTrue(gridManager.CheckWin(3, 3, testPlayer), "Diagonal win was not detected!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator CheckWinAntiDiagonal()
    {
        PlayerColor testPlayer = PlayerColor.Red; // Define the player color for the test.

        // Place Player 2's disks anti-diagonally from top-right to bottom-left.
        for (int i = 0; i < 4; i++)
        {
            var cell = gridManager.GetCell(i, 3 - i); // Get the appropriate anti-diagonal cell.
            Assert.IsNotNull(cell, $"Cell at ({i}, {3 - i}) is null."); // Ensure the cell exists.
            cell.SetPlayerInCell(testPlayer);
        }

        // Assert that an anti-diagonal win is detected.
        Assert.IsTrue(gridManager.CheckWin(3, 0, testPlayer), "Anti-diagonal win was not detected!");

        yield return null; // Allow Unity to process frame updates.
    }

    [UnityTest]
    public IEnumerator NoWinDetected()
    {
        PlayerColor testPlayer = PlayerColor.Blue; // Define the first player's color.
        PlayerColor otherPlayer = PlayerColor.Red; // Define the second player's color.

        // Set up alternating disks in the first row without forming a win.
        gridManager.GetCell(0, 0).SetPlayerInCell(testPlayer);
        gridManager.GetCell(0, 1).SetPlayerInCell(otherPlayer);
        gridManager.GetCell(0, 2).SetPlayerInCell(testPlayer);
        gridManager.GetCell(0, 3).SetPlayerInCell(otherPlayer);

        // Assert that no win is detected for Player 1.
        Assert.IsFalse(gridManager.CheckWin(0, 3, testPlayer), "False positive win detected!");

        yield return null; // Allow Unity to process frame updates.
    }
}
