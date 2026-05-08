using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveEnemyInfo : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private Button btn;
    [SerializeField]
    private TextMeshProUGUI enemyLevel;
    [SerializeField]
    private TextMeshProUGUI enemyCnt;
    private string enemyUID;
    private WaveEnemyInfoUIController owner;
    private int index;

    public void Init(WaveEnemyInfoUIController getOwner, int getIndex)
    {
        owner = getOwner;
        index = getIndex;

        btn.onClick.AddListener(OnClickEnemyInfo);
        enemyUID = string.Empty;
    }

    public void SetWaveEnemyInfo(string uid, int level, int cnt)
    {
        enemyUID = uid;

        if (!string.IsNullOrEmpty(enemyUID))
            icon.sprite = Resources.Load<Sprite>($"Enemy/SpriteLibrary/{enemyUID}");

        enemyLevel.text = $"Lv. {level}";
        enemyCnt.text = cnt.ToString();
    }

    public void Clear()
    {
        enemyUID = string.Empty;
        enemyLevel.text = "";
        enemyCnt.text = "";
        icon.sprite = null;
    }

    public void OnClickEnemyInfo()
    {
        owner.OnClickEnemyInfo(index);
    }
}
