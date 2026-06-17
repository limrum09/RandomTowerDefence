using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUIController : UIPopAnimationBase
{
    [System.Serializable]
    public class FieldTower
    {
        public GameObject root;
        public Image icon;
        public TextMeshProUGUI cntText;
    }

    [System.Serializable]
    public class Item
    {
        public GameObject root;
        public Image icon;
    }

    [SerializeField]
    private CanvasGroup canvas;
    [SerializeField]
    private List<FieldTower> towers;
    [SerializeField]
    private List<Item> items;

    [Header("Bonus Texts")]
    [SerializeField]
    private TextMeshProUGUI stageLevelBonusText;
    [SerializeField]
    private TextMeshProUGUI lifeBonusText;
    [SerializeField]
    private TextMeshProUGUI waveBonusText;
    [SerializeField]
    private TextMeshProUGUI towerBonusText;
    [SerializeField]
    private TextMeshProUGUI itemBonusText;
    [SerializeField]
    private TextMeshProUGUI remainGoldBonusText;
    [SerializeField]
    private TextMeshProUGUI queueGoldBonusText;

    [Header("Score Texts")]
    [SerializeField]
    private TextMeshProUGUI TotalScoreText;
    [SerializeField]
    private TextMeshProUGUI gemCntText;

    [Header("End Text")]
    [SerializeField]
    private TextMeshProUGUI gameEndText;

    private int rewardGemValue;

    private void Start()
    {
        Hide();
    }

    private void ResetUIs()
    {
        rewardGemValue = 0;

        stageLevelBonusText.text = string.Empty;
        lifeBonusText.text = string.Empty;
        waveBonusText.text = string.Empty;
        towerBonusText.text = string.Empty;
        itemBonusText.text = string.Empty;
        remainGoldBonusText.text = string.Empty;
        queueGoldBonusText.text = string.Empty;
        TotalScoreText.text = string.Empty;
        gemCntText.text = string.Empty;
        gameEndText.text = string.Empty;

        for (int i = 0; i < towers.Count; i++)
        {
            towers[i].icon.sprite = null;
            towers[i].cntText.text = string.Empty;
            towers[i].root.SetActive(false);
        }

        for (int i = 0; i < items.Count; i++)
        {
            items[i].icon.sprite = null;
            items[i].root.SetActive(false);
        }
    }

    public void SetGameOverUI(StageResultData data)
    {
        ResetUIs();
        Show();

        Sequence se = DOTween.Sequence();

        se.Append(TypeText(stageLevelBonusText, $"Stage [{data.stageLevel}]  x {data.stageLevelBonus}", 1.2f));
        se.Join(TypeText(lifeBonusText, $"Life Bonus [{data.currentLife}]  x {data.lifeBonus}", 1.2f));
        se.Join(TypeText(waveBonusText, $"Wave Score [{data.clearWave}]  + {data.waveScore}", 1.2f));

        se.AppendInterval(0.5f);

        for (int i = 0; i < data.towers.Count; i++)
        {
            int index = i;
            se.AppendCallback(() =>
            {
                towers[index].root.SetActive(true);
                towers[index].icon.sprite = data.towers[index].icon;
                towers[index].cntText.text = "0";
            });

            se.Join((CountText(towers[index].cntText, data.towers[index].count, "", 0.2f)));
            se.AppendInterval(0.4f);
        }

        se.Append(CountText(towerBonusText, data.towerSellGold, $"+ "));

        se.AppendInterval(0.5f);

        for (int i = 0; i < data.items.Count; i++)
        {
            int index = i;
            se.AppendCallback(() =>
            {
                items[index].root.SetActive(true);
                items[index].icon.sprite = data.items[index].icon;
            });

            se.AppendInterval(0.4f);
        }

        se.Append(CountText(itemBonusText, data.itemSellGold, $"+ "));
        se.AppendInterval(0.5f);

        se.Append(CountText(remainGoldBonusText, data.remainGold, $"Remain Gold  + "));
        se.AppendInterval(0.5f);

        se.Append(CountText(queueGoldBonusText, data.queueGold, $"Queue Gold  + "));
        se.AppendInterval(0.5f);

        se.Append(CountText(TotalScoreText, data.finalScore, $"Score : "));
        se.AppendInterval(0.5f);


        rewardGemValue = Mathf.RoundToInt(data.finalScore / 10);

        se.Append(CountText(gemCntText, rewardGemValue, $"+ "));
        se.AppendInterval(1.0f);


        se.AppendCallback(() =>
        {
            se.Append(TypeText(gameEndText, "GAME END", 1.2f));
            gameEndText.DOFade(1.2f, 2.0f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        });
    }

    public void GoLobby()
    {
        ResetUIs();
        Hide();
        Time.timeScale = 1.0f;
        LoadSceneManager.Instance.OnLoadStringScene("LobbyScene");
    }

    public void Show()
    {
        canvas.alpha = 1.0f;
        canvas.interactable = true;
        canvas.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvas.alpha = 0.0f;
        canvas.interactable = false;
        canvas.blocksRaycasts = false;
    }
}
