using UnityEngine;
using System.Collections;
using DG.Tweening; // Make sure you have DOTween imported

public class SpriteRotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 100f; // Default speed in degrees per second
    [SerializeField] private bool rotateClockwise = true; // Default rotation direction

    [Header("Randomization Options")]
    [SerializeField] private bool randomizeSpeed = false; // Whether to randomize speed
    [SerializeField] private float minSpeed = 50f; // Minimum random speed
    [SerializeField] private float maxSpeed = 150f; // Maximum random speed
    [SerializeField] private bool randomizeDirection = false; // Whether to randomize direction

    [Header("Scale Pulse Options")]
    [SerializeField] private bool enablePulse = false; // Whether to enable scale pulsing
    [SerializeField] private Vector3 pulseScale = new Vector3(1.2f, 1.2f, 1f); // Target scale for pulsing
    [SerializeField] private float pulseDuration = 1f; // Duration of the pulse

    [Header("Pulse Randomization Options")]
    [SerializeField] private bool randomizePulse = false; // Whether to randomize pulse scale and duration
    [SerializeField] private Vector2 pulseScaleRange = new Vector2(1.1f, 1.5f); // Min and max for random scale
    [SerializeField] private Vector2 pulseDurationRange = new Vector2(0.5f, 2f); // Min and max for random duration

    private Coroutine rotationCoroutine;

    private void OnEnable()
    {
        // Apply randomization if enabled
        if (randomizeSpeed)
        {
            rotationSpeed = Random.Range(minSpeed, maxSpeed);
        }

        if (randomizeDirection)
        {
            rotateClockwise = Random.value > 0.5f; // Randomly choose true or false
        }

        // Start the rotation
        rotationCoroutine = StartCoroutine(RotateSprite());

        // Start pulsing if enabled
        if (enablePulse)
        {
            StartPulsing();
        }
    }

    private void OnDisable()
    {
        // Stop the rotation when the object is disabled
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        // Kill the pulsing tween if enabled
        if (enablePulse)
        {
            StopPulsing();
        }
    }

    private IEnumerator RotateSprite()
    {
        // Determine the direction of rotation
        float direction = rotateClockwise ? -1f : 1f;

        while (true)
        {
            // Apply rotation incrementally
            transform.Rotate(0, 0, direction * rotationSpeed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }
    }

    private void StartPulsing()
    {
        // If randomizePulse is enabled, generate random values
        if (randomizePulse)
        {
            float randomScale = Random.Range(pulseScaleRange.x, pulseScaleRange.y);
            pulseScale = new Vector3(randomScale, randomScale, 1f);
            pulseDuration = Random.Range(pulseDurationRange.x, pulseDurationRange.y);
        }

        // DOTween pulse animation: scale up and down indefinitely
        transform.DOScale(pulseScale, pulseDuration)
                 .SetEase(Ease.InOutSine)
                 .SetLoops(-1, LoopType.Yoyo);
    }

    private void StopPulsing()
    {
        // Reset scale and stop all DOTween tweens on this object
        transform.DOKill();
        transform.localScale = Vector3.one; // Reset to default scale
    }
}
