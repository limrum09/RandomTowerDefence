using UnityEngine;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour
{
    [SerializeField]
    private string soundUID = "UIClick01";

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(PlaySound);
    }

    private void OnDestroy()
    {
        btn.onClick.RemoveListener(PlaySound);
    }

    private void PlaySound()
    {
        Managers.Sound.PlayUISFX(soundUID);
    }
}
