using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class StoreSlotUI : MonoBehaviour
{
    [SerializeField]
    private string uid;
    private StoreController owner;
    private Button btn;
    [SerializeField]
    private TextMeshProUGUI tempText;
    public string UID => uid;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickUI);
    }
    public void SetStoreSlot(string getUID, StoreController store)
    {
        owner = store;
        uid = getUID;
        TowerData data = Managers.TowerData.GetTowerData(getUID);
        tempText.text = data.towerType.ToString();
    }

    private void OnClickUI()
    {
        owner.OnClickSlotUI(uid);
    }
}
