using UnityEngine;

public class Tower : MonoBehaviour
{
    private string towerUid;
    private int index;

    public string TowerUID => towerUid;
    public int Index => index;

    public void Init(string getTowerUID, int getIndex)
    {
        towerUid = getTowerUID;
        index = getIndex;
    }
}
