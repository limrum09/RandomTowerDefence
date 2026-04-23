using Mono.Cecil;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TowerCntSkillInfo : MonoBehaviour
{
    [SerializeField]
    private Image skillIcon;
    [SerializeField]
    private TextMeshProUGUI towerCntText;

    private TowerType towerType;
    private int towerCnt;

    public string skillName;
    public TowerType Type => towerType;
    public int TowerCnt => towerCnt;

    private void Refresh()
    {
        towerCntText.text = towerCnt.ToString();
        this.gameObject.SetActive(towerCnt >= 1);
    }

    public void Init(TowerType getType, string towerUid)
    {
        TowerData tower = Managers.TowerData.GetTowerData(towerUid);

        string uid = tower.skillID;
        towerType = getType;

        string path = tower.iconPath;
        skillIcon.sprite = Resources.Load<Sprite>("Tower/Images/Icon_Tower_" + path + "_Idle");

        towerCnt = 0;

        Refresh();
    }
    public void SetTowerCnt(int value)
    {
        towerCnt = value;
        Refresh();
    }
    public void BuildTowerInField() => towerCnt++;
    public void RemoveTowerInField() => towerCnt--;
}
