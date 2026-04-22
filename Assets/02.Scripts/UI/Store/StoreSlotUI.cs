using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
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
    private StoreController owner;
    private Button btn;
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI gradeText;
    [SerializeField]
    private TextMeshProUGUI priceText;
    [SerializeField]
    private TextMeshProUGUI tempText;

    private SlotType type;
    public string UID => uid;
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickUI);
    }

    private void OnClickUI()
    {
        if (type == SlotType.Tower)
            owner.OnClickTowerSlotUI(uid);
        else if (type == SlotType.Item)
            owner.OnClickItemSlotUI(uid);
    }

    private void GetTowerData(TowerData data)
    {
        int grade = data.grade;
        gradeText.text = grade.ToString();
        priceText.text = data.buyPrice.ToString();

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
            return;
        }

        ItemData item = Managers.Item.GetItemData(getUID);
        if (item != null)
        {
            type = SlotType.Item;
            GetItemData(item);
            return;
        }

        type = SlotType.None;
        SetStoreSlot();
    }

    public void SetStoreSlot()
    {
        tempText.text = "타워를 넣지 못함";
        gradeText.text = "0";
        iconImage.gameObject.SetActive(false);
    }
}
