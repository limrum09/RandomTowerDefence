using System;
using UnityEngine.EventSystems;

public class MetaUpgradeSelectViewButton : UIHoverBase
{
    public bool IsInputLocked { get; set; }

    public event Action OnSelect;
    public event Action OnHover;
    public event Action OnExitHover;

    protected override void OnClick(PointerEventData eventData)
    {
        Select();
    }

    protected override void OnEnter(PointerEventData eventData)
    {
        if (IsInputLocked)
            return;

        OnHover?.Invoke();
    }

    protected override void OnExit(PointerEventData eventData)
    {
        if (IsInputLocked)
            return;

        OnExitHover?.Invoke();
    }

    public void Select()
    {
        if (IsInputLocked)
            return;

        OnSelect?.Invoke();
    }

    protected override bool CanClick(PointerEventData evnetData)
    {
        return !IsInputLocked;
    }
}
