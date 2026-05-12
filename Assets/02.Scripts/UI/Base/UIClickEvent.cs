using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIClickEvent : UIClickBase
{
    public UnityAction onClick;

    public void Bind(UnityAction action) => onClick = action;
    protected override void OnClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }
}
