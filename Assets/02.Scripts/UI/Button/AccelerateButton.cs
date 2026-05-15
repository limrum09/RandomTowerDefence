using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AccelerateButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private TextMeshProUGUI buttonText;

    public void ChangedGameSpeed(int speed) => buttonText.text = $"x{speed.ToString()}";

    public void BindButton(UnityAction action) => button.onClick.AddListener(action);
}
