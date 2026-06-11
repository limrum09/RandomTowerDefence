using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIHoverBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        OnEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit(eventData);
    }

    protected virtual void OnClick(PointerEventData eventData) { }
    protected virtual void OnEnter(PointerEventData eventData) { }
    protected virtual void OnExit(PointerEventData eventData) { }
}
