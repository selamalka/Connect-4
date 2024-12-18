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
        AudioManager.Instance.PlayAudio(AudioType.UI, "Button Click");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad);
        AudioManager.Instance.PlayAudio(AudioType.UI, "Button Enter");
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
