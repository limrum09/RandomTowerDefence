using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundIconButton : MonoBehaviour
{    
    [SerializeField]
    private Image icon;

    public void BindFunc(Func<bool> action)
    {
        Button btn = GetComponent<Button>();

        btn.onClick.AddListener(() =>
        {
            bool isView = action.Invoke();
            icon.gameObject.SetActive(!isView);
        });
    }

    public void SetIconView(bool isView)
    {
        icon.gameObject.SetActive(!isView);
    }
}
