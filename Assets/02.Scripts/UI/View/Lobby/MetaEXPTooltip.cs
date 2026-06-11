using TMPro;
using UnityEngine;

public class MetaEXPTooltip : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private TextMeshProUGUI currentEXPText;
    [SerializeField]
    private TextMeshProUGUI needEXPText;

    public void ShowEXPText()
    {
        canvas.alpha = 1.0f;

        int level = Managers.Player.GetPlayerLevel();
        int currentExp = Managers.Player.GetCurrentEXP();
        int needEXP = Managers.ResearchLevel.GetNeedExp(level);

        currentEXPText.text = currentExp.ToString();
        needEXPText.text = $"/ {needEXP.ToString()}";
    }

    public void HideEXPText()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
}
