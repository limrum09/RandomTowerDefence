using System.Collections.Generic;
using UnityEngine;

public class TowerCntSkillInfoController : MonoBehaviour
{
    [SerializeField]
    private List<TowerCntSkillInfo> info = new List<TowerCntSkillInfo>();

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

    public void ChangeFiledTower(TowerType type, int towerCnt)
    {
        info.Find(x => x.Type == type).SetTowerCnt(towerCnt);
    }
}
