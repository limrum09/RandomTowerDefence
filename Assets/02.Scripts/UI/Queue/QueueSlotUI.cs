using UnityEngine;
using UnityEngine.UI;

public class QueueSlotUI : MonoBehaviour
{
    [SerializeField]
    private Button btn;
    [SerializeField]
    private TowerPreviewCharacter tpc;

    private string towerUID;
    private int slotIndex;
    private QueueController owner;

    public bool IsQueueEmpty => string.IsNullOrEmpty(towerUID);

    public void Init(QueueController getOwner, int getIndex)
    {
        towerUID = string.Empty;
        owner = getOwner;
        slotIndex = getIndex;
        btn.onClick.AddListener(OnClickSlot);
    }

    public void SetSlotUI(string uid)
    {
        towerUID = uid;

        tpc.SetShow();
        tpc.SetTower(uid);
    }

    public void RemoveSlotUI()
    {
        towerUID = string.Empty;
        tpc.SetHide();
    }

    private void OnClickSlot()
    {
        owner.OnClickQueueSlot(slotIndex, towerUID);
    }
}
