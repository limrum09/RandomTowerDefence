using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIToggleSound : MonoBehaviour, IPointerClickHandler
{
    [SerializeField]
    private string soundUID = "BookSlide01";

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!toggle.IsInteractable())
            return;

        Managers.Sound.PlayUISFX(soundUID);
    }
}
