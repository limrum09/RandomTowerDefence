using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIClickView : UIClickBase
{
    public UnityEvent onClick;
    protected override void OnClick(PointerEventData eventData)
    {
        if(onClick != null)
        {
            onClick?.Invoke();
        }
    }
}
