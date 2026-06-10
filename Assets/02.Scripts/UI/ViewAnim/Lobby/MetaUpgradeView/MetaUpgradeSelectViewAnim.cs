using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MetaUpgradeSelectViewAnim : UIPopAnimationBase
{
    [System.Serializable]
    public class SelectPanelAnimData
    {
        public MetaUpgradeSelectView view;
        public MetaUpgradeSelectViewButton button;
        public RectTransform rect;
        public Image selectImage;
    }

    [SerializeField]
    private SelectPanelAnimData[] panels;
    [SerializeField]
    private float hoverScale = 1.04f;
    [SerializeField]
    private float hoverDuration = 0.15f;

    private Dictionary<RectTransform, Vector2> originPosMap = new Dictionary<RectTransform, Vector2>();
    private Sequence swapSequence;
    private Tween[] hoverTween;
    private Vector3[] originScale;

    private void HoverSelect(int index)
    {
        if (panels[index].button.IsInputLocked)
            return;


        HideSelectImage();
        panels[index].selectImage.gameObject.SetActive(true);
    }

    private void HoverEnter(int index)
    {
        if (panels[index].button.IsInputLocked)
            return;

        hoverTween[index]?.Kill();

        hoverTween[index] = HoverScale(panels[index].rect, hoverScale, hoverDuration);
    }

    private void HoverExit(int index)
    {
        if (panels[index].button.IsInputLocked)
            return;

        hoverTween[index]?.Kill();

        hoverTween[index] = HoverScale(panels[index].rect, 1f, hoverDuration);
    }

    private void HideSelectImage()
    {
        for(int i = 0; i < panels.Length; i++)
        {
            panels[i].selectImage.gameObject.SetActive(false);
        }
    }

    private void PlayAllTextSwap(System.Action changeContent)
    {
        swapSequence?.Kill();

        List<RectTransform> targets = new List<RectTransform>();
        List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();

        for (int i = 0; i < panels.Length; i++)
        {
            if (!panels[i].view.gameObject.activeInHierarchy)
                continue;

            texts.AddRange(panels[i].view.Texts);
            targets.AddRange(panels[i].view.TextRects);
        }

        swapSequence = UISwapFromRightToLeft(targets.ToArray(), texts.ToArray(), originPosMap, changeContent, 200f, 0.12f);
        swapSequence.AppendCallback(() =>
        {
            changeContent?.Invoke();
        });
    }

    public void Init()
    {
        hoverTween = new Tween[panels.Length];
        originScale = new Vector3[panels.Length];

        for (int i = 0; i < panels.Length; i++)
        {
            int index = i;

            originScale[index] = panels[index].rect.localScale;

            foreach(var rect in panels[index].view.TextRects)
            {
                if (!originPosMap.ContainsKey(rect))
                    originPosMap.Add(rect, rect.anchoredPosition);
            }

            panels[index].selectImage.gameObject.SetActive(false);
            panels[index].button.OnSelect += () => HoverSelect(index);
            panels[index].button.OnHover += () => HoverEnter(index);
            panels[index].button.OnExitHover += () => HoverExit(index);
        }
    }

    public void ChangedToggle()
    {
        PlayAllTextSwap(() =>
        {
            HoverSelect(0);
        });
    }
}
