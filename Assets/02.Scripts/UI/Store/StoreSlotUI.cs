using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreSlotUI : MonoBehaviour
{
    private enum SlotType
    {
        None,
        Tower,
        Item
    }

    [SerializeField]
    private string uid;
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI gradeText;
    [SerializeField]
    private TextMeshProUGUI priceText;
    [SerializeField]
    private TextMeshProUGUI tempText;
    [SerializeField]
    private HorizontalLayoutGroup layoutGroup;

    private StoreController owner;
    private Button btn;
    private SlotType type;
    private int price;
    public string UID => uid;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickUI);
        price = 0;
    }

    private void OnClickUI()
    {
        if (!owner.UsingGold(-price))
            return;

        if (type == SlotType.Tower)
        {
            if (owner.OnClickTowerSlotUI(uid))
                price = 0;
            else
                owner.UsingGold(price);
        }            
        else if (type == SlotType.Item)
        {
            if(owner.OnClickItemSlotUI(uid))
                price = 0;
            else
                owner.UsingGold(price);
        }
    }

    private void GetTowerData(TowerData data)
    {
        int grade = data.grade;
        gradeText.text = grade.ToString();
        priceText.text = data.buyPrice.ToString();

        string path = data.iconPath;

        Sprite icon = Resources.Load<Sprite>($"Tower/Images/Icon_Tower_{data.towerType}_{data.grade}_Idle");

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

    private void GetItemData(ItemData data)
    {
        string grade = "N";

        switch(data.grade)
        {
            case ItemGrade.Normal : 
                grade = "N"; 
                break;
            case ItemGrade.Rare:
                grade = "R";
                break;
            case ItemGrade.Epic:
                grade = "E";
                break;
            case ItemGrade.Legend:
                grade = "L";
                break;
            case ItemGrade.Mythic:
                grade = "M";
                break;
            default:
                grade = string.Empty;
                break;
        }

        gradeText.text = grade;
        priceText.text = data.salePrice.ToString();

        Sprite icon = Resources.Load<Sprite>($"Item/Images/{data.iconUID}");

        if (icon != null)
        {
            tempText.text = "";
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = icon;
        }
        else
        {
            tempText.text = data.grade.ToString();
            iconImage.gameObject.SetActive(false);
        }
    }

    public void SetStoreCTR(StoreController store) => owner = store;

    public void SetStoreSlot(string getUID)
    {
        uid = getUID;

        TowerData tower = Managers.TowerData.GetTowerData(getUID);
        if (tower != null)
        {
            type = SlotType.Tower;
            GetTowerData(tower);
            price = tower.buyPrice;
            return;
        }

        ItemData item = Managers.Item.GetItemData(getUID);
        if (item != null)
        {
            type = SlotType.Item;
            GetItemData(item);
            price = item.buyPrice;
            return;
        }

        type = SlotType.None;
        layoutGroup.CalculateLayoutInputHorizontal();
        layoutGroup.SetLayoutHorizontal();
        SetStoreSlot();
    }

    public void SetStoreSlot()
    {
        tempText.text = "타워를 넣지 못함";
        gradeText.text = "0";
        price = 0;
        iconImage.gameObject.SetActive(false);
    }
}
