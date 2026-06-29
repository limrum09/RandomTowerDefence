using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIDropDownSound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField]
    private string clickSoundUID = "UIClick01";
    [SerializeField]
    private string valueChangedSoundUID = "Tick01";

    private Toggle toggle;
    private TMP_Dropdown dropdown;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        toggle = GetComponent<Toggle>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!dropdown.IsInteractable())
            return;

        Managers.Sound.PlayUISFX(clickSoundUID);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (toggle != null && !toggle.IsInteractable())
            return;

        Managers.Sound.PlayUISFX(valueChangedSoundUID);
    }
}
