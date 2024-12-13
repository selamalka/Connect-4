using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0.2f), 0.2f).SetEase(Ease.OutQuad);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.DOScale(1.1f, 0.2f).SetEase(Ease.OutQuad);
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
