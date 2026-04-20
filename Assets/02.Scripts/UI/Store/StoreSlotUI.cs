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
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI towerGradeText;
    [SerializeField]
    private TextMeshProUGUI towerPriceText;
    [SerializeField]
    private TextMeshProUGUI tempText;
    public string UID => uid;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickUI);
    }

    public void SetStoreCTR(StoreController store) => owner = store;

    public void SetStoreSlot(string getUID)
    {
        uid = getUID;
        TowerData data = Managers.TowerData.GetTowerData(getUID);

        if(data == null)
        {
            SetStoreSlot();
            return;
        }

        int grade = data.grade;
        towerGradeText.text = grade.ToString();

        int price = data.buyPrice;
        towerPriceText.text = price.ToString();

        string path = data.iconPath;
        
        Sprite icon = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{path}_Idle");
        
        if (icon != null)
        {
            tempText.text = "";
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = icon;
        }
        else
        {
            tempText.text = data.towerType.ToString();
            iconImage.gameObject.SetActive(false);
        }
    }

    public void SetStoreSlot()
    {
        tempText.text = "타워를 넣지 못함";
    }

    private void OnClickUI()
    {
        owner.OnClickSlotUI(uid);
    }
}
