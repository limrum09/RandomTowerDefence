using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 상점 슬롯 UI
/// StoreProduct 정보를 화면에 표시,
/// 클릭, 마우스 Enter, Exit 정보를 StoreController에게 전달
/// </summary>
public class StoreSlotUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
    private StoreProductType type;
    private int price;
    public string UID => uid;

    /// <summary>
    /// 현제 슬롯에 표시 중인 상점 상품 정보
    /// </summary>
    public StoreProduct Product;

    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickUI);
        price = 0;
    }

    /// <summary>
    /// 슬롯 클릭 시 구매 요청을 StoreController에게 전달
    /// 실제 구매 가능 여부, 골드 차감, 아이템 지급은 StoreController가 처리
    /// </summary>
    private void OnClickUI()
    {
        if (type == StoreProductType.None || string.IsNullOrEmpty(uid) || price <= -1)
            return;

        owner.RequestBuy(this);
    }

    /// <summary>
    /// 해당 슬롯을 관리하는 StoreContrller를 등록
    /// </summary>
    /// <param name="store"></param>
    public void SetStoreCTR(StoreController store) => owner = store;

    /// <summary>
    /// StoreProduct 정보를 기반으로 슬롯 UI를 갱신
    /// </summary>
    /// <param name="getProduct"></param>
    public void SetStoreSlot(StoreProduct getProduct)
    {
        Product = getProduct;

        uid = Product.uid;
        price = Product.price;
        type = Product.type;

        gradeText.text = Product.gradeText;
        priceText.text = Product.price.ToString();

        if(Product.Icon != null)
        {
            tempText.text = "";
            iconImage.gameObject.SetActive(true);
            iconImage.sprite = Product.Icon;
        }
        else
        {
            tempText.text = Product.type == StoreProductType.None ? "비어있음" : Product.uid;
            iconImage.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 슬롯을 빈 상태로 초기화
    /// </summary>
    public void SetStoreSlot()
    {
        uid = string.Empty;
        type = StoreProductType.None;
        price = 0;

        tempText.text = "타워를 넣지 못함";
        gradeText.text = "0";
        priceText.text = "0";
        iconImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 마우스가 슬롯 위에 올라왔을 때, StoreController에게 알린다
    /// Tooltip 표시 여부는 StoreController가 판단
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        owner.OnPointerEnterSlot(this);
    }

    /// <summary>
    /// 마우스가 슬롯을 벗어 났을 때, StoreController에게 알린다
    /// Tooltip 숨김 여부는 StoreController가 판단
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        owner.OnPointerExitSlot(this);
    }
}
