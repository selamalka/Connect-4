using MoonActive.Connect4;
using System;
using UnityEngine.EventSystems;
using UnityEngine;

public class DisksSpawnerTester : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject m_DiskInidication;

    private Action m_OnButtonClicked;

    internal void Init(Action onClick)
    {
        m_OnButtonClicked = (Action)Delegate.Combine(m_OnButtonClicked, onClick);
    }

    public void OnButtonClicked()
    {
        m_OnButtonClicked?.Invoke();
    }

    internal Disk CreateDiskAndResetPosition(Disk disk)
    {
        Disk disk2 = UnityEngine.Object.Instantiate(disk, base.transform);
        disk2.transform.localPosition = Vector3.zero;
        return disk2;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_DiskInidication.SetActive(value: true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_DiskInidication.SetActive(value: false);
    }
}