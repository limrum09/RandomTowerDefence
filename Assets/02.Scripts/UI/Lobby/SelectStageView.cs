using Firebase.Auth;
using UnityEngine;
using UnityEngine.Events;

public class SelectStageView : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField]
    private SelectStageViewAnim viewAnim;

    [Header("Background")]
    [SerializeField]
    private UIClickEvent background;

    [Header("Buttons")]
    [SerializeField]
    private SelectStageButton easyStageButton;
    [SerializeField]
    private SelectStageButton normalStageButton;
    [SerializeField]
    private SelectStageButton hardStageButton;
    [SerializeField]
    private SelectStageButton hellStageButton;

    private bool isShowing;
    private bool isHiding;
    private bool isOpened;

    private void Awake()
    {
        background.Bind(Hide);

        viewAnim.ShowEnd += ShowEnd;
        viewAnim.HideEnd += HideEnd;
    }

    private void ShowEnd()
    {
        isShowing = false;
        isOpened = true;
    }

    private void HideEnd()
    {
        isHiding = false;
        isOpened = false;
    }

    public void Show()
    {
        if (isShowing || isHiding || isOpened)
            return;

        isShowing = true;
        viewAnim.Show();
    }

    public void Hide()
    {
        if (isShowing || isHiding || !isOpened)
            return;

        isHiding = true;
        viewAnim.Hide();
    }

    public void BindEasyStageButton(UnityAction<string> action) => easyStageButton.OnStageMove += () => action?.Invoke("EASY");
    public void BindNormalStageButton(UnityAction<string> action) => normalStageButton.OnStageMove += () => action?.Invoke("NORMAL");
    public void BindHardStageButton(UnityAction<string> action) => hardStageButton.OnStageMove += () => action?.Invoke("HARD");
    public void BindHellStageButton(UnityAction<string> action) => hellStageButton.OnStageMove += () => action?.Invoke("HELL");
}
