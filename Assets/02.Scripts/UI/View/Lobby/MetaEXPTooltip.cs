using TMPro;
using UnityEngine;

public class MetaEXPTooltip : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private TextMeshProUGUI currentEXPText;

    public void ShowEXPText()
    {
        canvas.alpha = 1.0f;

        int level = Managers.Player.GetPlayerLevel();

        if(level >= 12)
        {
            currentEXPText.text = $"{Managers.Local.GetString("UI", "UI_MAX_LEVEL")}";
            return;
        }

        int currentExp = Managers.Player.GetCurrentEXP();
        int needEXP = Managers.ResearchLevel.GetNeedExp(level);

        currentEXPText.text = $"{currentExp.ToString()} / {needEXP.ToString()}";
    }

    public void HideEXPText()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
}
