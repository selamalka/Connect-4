using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class GameModeTest : MonoBehaviour
{
    private GameMode currentMode;

    [SetUp]
    public void Setup()
    {
        // Clear static event handlers to avoid lingering references
        UIManager.OnSelectGameMode = null;

        // Subscribe to the UIManager event
        UIManager.OnSelectGameMode += UpdateGameMode;

        // Initialize default game mode
        currentMode = GameMode.PlayerVsPlayer;
    }

    [TearDown]
    public void Teardown()
    {
        // Unsubscribe and clear static event handlers
        UIManager.OnSelectGameMode -= UpdateGameMode;
        UIManager.OnSelectGameMode = null;
    }

    private void UpdateGameMode(GameMode mode)
    {
        currentMode = mode;
    }

    [UnityTest]
    public IEnumerator GameModeChangeThroughUIManager()
    {
        // Create a UIManager instance
        var uiManager = new GameObject("UIManager").AddComponent<UIManager>();

        // Assert the initial game mode
        Assert.AreEqual(GameMode.PlayerVsPlayer, currentMode, "Game mode should initially be PlayerVsPlayer.");

        // Simulate selecting a new game mode
        uiManager.SelectGameMode(GameMode.PlayerVsComputer);
        yield return null; // Wait for the event to propagate

        // Assert the game mode change
        Assert.AreEqual(GameMode.PlayerVsComputer, currentMode, "Game mode did not change to PlayerVsComputer.");

        // Simulate another game mode selection
        uiManager.SelectGameMode(GameMode.ComputerVsComputer);
        yield return null;

        // Assert the new game mode
        Assert.AreEqual(GameMode.ComputerVsComputer, currentMode, "Game mode did not change to ComputerVsComputer.");
    }
}
