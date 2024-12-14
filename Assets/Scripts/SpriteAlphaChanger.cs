using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SpriteAlphaChanger : MonoBehaviour
{
    [SerializeField] private PlayerColor playerColor;

    private Coroutine fadeCoroutine;

    private void OnEnable()
    {
        GameManager.OnCurrentPlayerChanged += ChangeAlpha;
    }

    private void OnDisable()
    {
        GameManager.OnCurrentPlayerChanged -= ChangeAlpha;
    }

    private void ChangeAlpha(PlayerColor currentPlayerColor)
    {
        Image image = GetComponent<Image>();
        float targetAlpha = currentPlayerColor == playerColor ? 1f : 0.4f;

        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        fadeCoroutine = StartCoroutine(FadeAlpha(image, targetAlpha, 0.5f)); // Adjust the duration as needed
    }

    private IEnumerator FadeAlpha(Image image, float targetAlpha, float duration)
    {
        Color initialColor = image.color;
        float initialAlpha = initialColor.a;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            timeElapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(initialAlpha, targetAlpha, timeElapsed / duration);
            image.color = new Color(initialColor.r, initialColor.g, initialColor.b, alpha);
            yield return null;
        }

        image.color = new Color(initialColor.r, initialColor.g, initialColor.b, targetAlpha);
    }
}
