using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIHoverBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!CanClick(eventData))
            return;

        if(Managers.HasInstance)
            Managers.Sound.PlayUISFX("UIClick01");
        OnClick(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CanClick(eventData) && Managers.HasInstance)
            Managers.Sound.PlayUISFX("Tick01");

        OnEnter(eventData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        OnExit(eventData);
    }

    protected virtual bool CanClick(PointerEventData evnetData)
    {
        return true;
    }

    protected virtual void OnClick(PointerEventData eventData) { }
    protected virtual void OnEnter(PointerEventData eventData) { }
    protected virtual void OnExit(PointerEventData eventData) { }
}
