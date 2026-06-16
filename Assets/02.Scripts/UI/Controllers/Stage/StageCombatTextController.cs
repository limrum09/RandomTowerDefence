using DG.Tweening;
using TMPro;
using UnityEngine;

public class StageCombatTextController : MonoBehaviour
{
    [SerializeField]
    private Transform root;
    [SerializeField]
    private TextMeshProUGUI combatText;

    public void SetText(string text)
    {
        TextMeshProUGUI newText = Instantiate(combatText, root);
        newText.text = text;

        newText.DOFade(0f, 2f).OnComplete(() =>
        {
            Destroy(newText.gameObject);
        });
    }
}
