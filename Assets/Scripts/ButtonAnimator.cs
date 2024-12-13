using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private void OnEnable()
    {
        transform.localScale = Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlayAudioWithRandomPitch(AudioType.UI, "Button Click", 0.8f, 1.2f);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlayAudioWithRandomPitch(AudioType.UI, "Button Enter",0.8f, 1.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!RectTransformUtility.RectangleContainsScreenPoint(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera))
        {
            transform.DOScale(1f, 0.2f).SetEase(Ease.InQuad);
        }
    }
}
