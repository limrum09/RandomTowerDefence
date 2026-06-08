using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectStageButton : UIClickBase
{
    [SerializeField]
    private RectTransform fillRect;
    [SerializeField]
    private Image limitImage;
    [SerializeField]
    private TextMeshProUGUI limitText;
    [SerializeField]
    private IsAchievementCompleteCondition condition;
    private float fillWidth;
    private bool isConditionPass;

    public bool IsInputLocked { get; set; }

    public event Action OnStageMove;
    public event Action OnSelect;
    public event Action OnHover;
    public event Action OnExitHover;

    public RectTransform FillRect => fillRect;
    public float FillWidth => fillWidth;

    private void Awake()
    {
        fillWidth = fillRect.sizeDelta.x;
    }

    private void Start()
    {
        SetLimitImage();
    }

    private void SetLimitImage()
    {
        if (condition == null)
        {
            limitImage.gameObject.SetActive(false);
            isConditionPass = true;
            return;
        }
            

        string limitStr = Managers.Local.GetString("UI", "UI_LIMIT_TEXT");
        string questStr = Managers.Local.GetString("Quest", condition.QuestUID);
        limitText.text = string.Format(limitStr, questStr);
        limitImage.gameObject.SetActive(!condition.IsPass());
        isConditionPass = condition.IsPass();
    }

    protected override void OnClick(PointerEventData eventData)
    {
        if (IsInputLocked || !isConditionPass)
            return;

        OnSelect?.Invoke();
    }

    protected override void OnEnter(PointerEventData eventData)
    {
        if (IsInputLocked || !isConditionPass)
            return;

        OnHover?.Invoke();
    }

    protected override void OnExit(PointerEventData eventData)
    {
        if (IsInputLocked || !isConditionPass)
            return;

        OnExitHover?.Invoke();
    }

    public void OnSelectAnimComplete()
    {
        OnStageMove?.Invoke();
    }
}
