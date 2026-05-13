using TMPro;
using UnityEngine;

public class MetaUpgradeSelectView : MonoBehaviour
{
    [SerializeField]
    private Sprite con;
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI info;
    [SerializeField]
    private TextMeshProUGUI upgradeText1;
    [SerializeField]
    private TextMeshProUGUI currentValue1;
    [SerializeField]
    private TextMeshProUGUI nextValue1;
    [SerializeField]
    private TextMeshProUGUI upgradeText2;
    [SerializeField]
    private TextMeshProUGUI currentValue2;
    [SerializeField]
    private TextMeshProUGUI nextValue2;

    private TowerType type;
    private int towerGrade;
    private string publicType;
    public void SetSelectView(TowerType getType, int getGrade)
    {
        type = getType;
        towerGrade = getGrade;

        
    }
}
