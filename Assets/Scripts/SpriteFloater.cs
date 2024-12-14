using UnityEngine;
using DG.Tweening;

public class SpriteFloater : MonoBehaviour
{
    [Header("Floating Settings")]
    [SerializeField] private float moveIntensity = 0.5f; // Maximum distance to move in any direction
    [SerializeField] private float moveDuration = 1f; // Base duration of each movement
    [SerializeField] private Vector2 durationRange = new Vector2(0.5f, 1.5f); // Randomized duration range
    [SerializeField] private bool randomizeDuration = true; // Whether to randomize the duration
    [SerializeField] private bool randomizeDirection = true; // Whether to randomize the floating direction

    private Vector3 initialPosition; // The starting position of the sprite

    private void Start()
    {
        initialPosition = transform.position;
        StartFloating();
    }

    private void StartFloating()
    {
        // Start the floating loop
        MoveToRandomPosition();
    }

    private void MoveToRandomPosition()
    {
        // Generate a random offset for the floating effect
        float offsetX = Random.Range(-moveIntensity, moveIntensity);
        float offsetY = Random.Range(-moveIntensity, moveIntensity);

        // Determine the next position
        Vector3 targetPosition = initialPosition + new Vector3(offsetX, offsetY, 0);

        // Randomize the movement duration if enabled
        float duration = randomizeDuration ? Random.Range(durationRange.x, durationRange.y) : moveDuration;

        // Move the object to the target position
        transform.DOMove(targetPosition, duration)
            .SetEase(Ease.Linear) // Smooth ease for a floating effect
            .OnComplete(() =>
            {
                // Return to the initial position or pick another random position
                if (randomizeDirection)
                {
                    MoveToRandomPosition(); // Float in another random direction
                }
                else
                {
                    transform.DOMove(initialPosition, duration).SetEase(Ease.Linear).OnComplete(MoveToRandomPosition);
                }
            });
    }

    private void OnDisable()
    {
        // Reset to the initial position and stop floating when disabled
        transform.DOKill();
        transform.position = initialPosition;
    }
}
