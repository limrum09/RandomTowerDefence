using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class QueueSlotUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Button btn;

    private string towerUID;
    private int slotIndex;
    private TowerData data;
    private QueueController owner;

    private void Refresh()
    {
        bool isEmpty = string.IsNullOrEmpty(towerUID);

        icon.gameObject.SetActive(!isEmpty);

        if (!isEmpty)
        {
            data = Managers.TowerData.GetTowerData(towerUID);

            if (data != null)
            {
                // icon.sprite
            }
        }
    }

    public void Init(QueueController getOwner, int getIndex)
    {
        towerUID = string.Empty;
        owner = getOwner;
        slotIndex = getIndex;
        btn.onClick.AddListener(OnClickSlot);
    }

    public void SetSlotUI(string uid)
    {
        towerUID = uid; ;
        Refresh();
    }

    public void RemoveSlotUI()
    {
        towerUID = string.Empty;
        Refresh();
    }

    private void OnClickSlot()
    {
        owner.OnClickQueueSlot(slotIndex, towerUID);
    }
}
