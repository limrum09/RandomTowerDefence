using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UIClickBase : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick(eventData);
    }

    protected abstract void OnClick(PointerEventData eventData);
}
