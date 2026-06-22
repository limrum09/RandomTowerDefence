using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerCntSkillInfoController : MonoBehaviour
{
    [SerializeField]
    private List<TowerCntSkillInfo> info = new List<TowerCntSkillInfo>();
    [SerializeField]
    private TextMeshProUGUI fieldTowerCntText;

    private RunSessionDataManager runSessionDataManager;
    private FieldTowerManager fieldTowerManager;

    int len;
    private void Start()
    {
        len = info.Count;
        List<string> tower = new List<string> { "T0011" , "T0021" , "T0031" , "T0041" , "T0051" , "T0061" };
        for(int i = 0; i < len; i++)
        {
            info[i].Init((TowerType)i, tower[i]);
        }
    }

    private void SetFieldTowerText()
    {
        if (runSessionDataManager == null || fieldTowerManager == null)
            return;

        int currentFieldTowerCnt = fieldTowerManager.GetTotalTowerCount();
        int canBuildTowerMaxCnt = runSessionDataManager.GetMaxBuildTowerCount();

        float ratio = Mathf.Clamp01((float)currentFieldTowerCnt / canBuildTowerMaxCnt);

        Color textColor = Color.white;

        if (ratio == 1.0f)
            textColor = Color.red;
        else if (ratio >= 0.7f)
            textColor = Color.yellow;

        fieldTowerCntText.color = textColor;
        fieldTowerCntText.text = $"{currentFieldTowerCnt} / {canBuildTowerMaxCnt}";
    }

    public void Init(RunSessionDataManager getRunManager, FieldTowerManager getFiledtowerManager)
    {
        runSessionDataManager = getRunManager;
        fieldTowerManager = getFiledtowerManager;

        SetFieldTowerText();
    }

    public void ChangeUserLevel(int level)
    {
        SetFieldTowerText();
    }

    public void ChangeFieldTower(TowerType type, int towerCnt)
    {
        info.Find(x => x.Type == type).SetTowerCnt(towerCnt);

        SetFieldTowerText();
    }
}
