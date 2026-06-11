using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIHoverActionBase : UIHoverBase
{
    [SerializeField]
    private UnityEvent OnHoverSelect;
    [SerializeField]
    private UnityEvent OnHoverEnter;
    [SerializeField]
    private UnityEvent OnHoverExit;

    protected override void OnClick(PointerEventData eventData)
    {
        OnHoverSelect?.Invoke();
    }

    protected override void OnEnter(PointerEventData eventData)
    {
        OnHoverEnter?.Invoke();
    }

    protected override void OnExit(PointerEventData eventData)
    {
        OnHoverExit?.Invoke();
    }
}
