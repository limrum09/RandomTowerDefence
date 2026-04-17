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
    private Image image;
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

        string path = data.iconPath;
        
        Sprite icon = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{path}_Idle");
        
        if (icon != null)
        {
            tempText.text = "";
            image.gameObject.SetActive(true);
            image.sprite = icon;
        }
        else
        {
            tempText.text = data.towerType.ToString();
            image.gameObject.SetActive(false);
        }
    }

    private void OnClickUI()
    {
        owner.OnClickSlotUI(uid);
    }
}
