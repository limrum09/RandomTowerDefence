using UnityEngine;
using UnityEngine.UI;

public class QueueSlotUI : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Button btn;
    [SerializeField]
    private Animator anim;

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
                string path = data.iconPath;

                AnimatorDatas aniDatas = Resources.Load<AnimatorDatas>("Anis/AnimatorDatas");

                if(aniDatas == null)
                {
                    icon.sprite = null;
                    anim.runtimeAnimatorController = null;
                    return;
                }

                RuntimeAnimatorController aniController = aniDatas.FindByCode("UI_" + path + "Ani");

                if (aniController == null)
                {
                    icon.sprite = null;
                    anim.runtimeAnimatorController = null;
                    return;
                }
                    
                anim.runtimeAnimatorController = aniController;
            }
        }
        else
        {
            icon.sprite = null;
            anim.runtimeAnimatorController = null;
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
