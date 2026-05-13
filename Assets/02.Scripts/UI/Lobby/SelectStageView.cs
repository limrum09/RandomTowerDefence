using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SelectStageView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private RectTransform panelRect;

    [Header("Background")]
    [SerializeField]
    private UIClickEvent background;

    [Header("Buttons")]
    [SerializeField]
    private Button easyStageButton;
    [SerializeField]
    private Button normalStageButton;
    [SerializeField]
    private Button hardStageButton;
    [SerializeField]
    private Button hellStageButton;

    [Header("Animtion")]
    [SerializeField]
    private float fadeDuration = 0.2f;
    [SerializeField]
    private float panelScaleDuration = 0.25f;
    [SerializeField]
    private float buttonMoveDuration = 0.25f;
    [SerializeField]
    private float buttonDelay = 0.06f;
    [SerializeField]
    private float buttonStartYOffset = -40f;

    private Sequence se;
    private RectTransform[] buttonRects;
    private Vector2[] buttonOriginPos;
    private void Awake()
    {
        buttonRects = new RectTransform[]
        {
            easyStageButton.GetComponent<RectTransform>(),
            normalStageButton.GetComponent<RectTransform>(),
            hardStageButton.GetComponent<RectTransform>(),
            hellStageButton.GetComponent<RectTransform>()
        };

        buttonOriginPos = new Vector2[buttonRects.Length];

        for(int i = 0; i <  buttonRects.Length; i++)
        {
            buttonOriginPos[i] = buttonRects[i].anchoredPosition;
        }

        background.Bind(Hide);
    }

    public void Show()
    {
        se?.Kill();

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void BindEasyStageButton(UnityAction<string> action) => easyStageButton.onClick.AddListener(() => action?.Invoke("EASY"));
    public void BindNormalStageButton(UnityAction<string> action) => normalStageButton.onClick.AddListener(() => action?.Invoke("NORMAL"));
    public void BindHardStageButton(UnityAction<string> action) => hardStageButton.onClick.AddListener(() => action?.Invoke("HARD"));
    public void BindHellStageButton(UnityAction<string> action) => hellStageButton.onClick.AddListener(() => action?.Invoke("HELL"));
}
