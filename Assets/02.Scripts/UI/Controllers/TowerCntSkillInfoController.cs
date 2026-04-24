using System.Collections.Generic;
using UnityEngine;

public class TowerCntSkillInfoController : MonoBehaviour
{
    [SerializeField]
    private List<TowerCntSkillInfo> info = new List<TowerCntSkillInfo>();
    private FieldTowerManager fieldTowers;

    int len;
    private void Init()
    {
        len = info.Count;
        List<string> tower = new List<string> { "T0011" , "T0021" , "T0031" , "T0041" , "T0051" , "T0061" };
        for(int i = 0; i < len; i++)
        {
            info[i].Init((TowerType)i, tower[i]);
        }

        fieldTowers.OnFieldTowerChanged += ChangeFiledTower;
    }

    private void OnDestroy()
    {
        fieldTowers.OnFieldTowerChanged -= ChangeFiledTower;
    }

    public void BindManager(FieldTowerManager fieldTowers)
    {
        this.fieldTowers = fieldTowers;

        Init();
    }

    public void ChangeFiledTower(TowerType type)
    {
        if (fieldTowers == null)
            return;

        int towerCnt = fieldTowers.GetTowerCount(type);
        info.Find(x => x.Type == type).SetTowerCnt(towerCnt);
    }
}
