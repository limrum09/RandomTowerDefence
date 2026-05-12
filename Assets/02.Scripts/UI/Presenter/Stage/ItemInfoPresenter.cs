using System;
using UnityEditor;
using UnityEngine;

public class ItemInfoPresenter
{
    private ItemData model;
    private ItemInfoView view;
    private int index;

    public event Action<int> OnItemSell;
    public ItemInfoPresenter(ItemInfoView getView)
    {
        view = getView;
        index = 0;
        view.BindItemSell(OnClickItemSellButton);
    }

    public void SetModel(ItemData getModel, int getIndex)
    {
        model = getModel;
        index = getIndex;

        Sprite icon = Resources.Load<Sprite>($"Item/Images/{model.iconUID}");
        view.SetIcon(icon);

        string name = Managers.Local.GetString(model.stringKey);
        view.SetItemName(name);
        view.SetItemGrade(model.grade);
        view.SetItemScope(model.scopeRange);
        view.SetItemTarget(model.target);

        string des = Managers.Local.GetString(model.itemDesc);
        view.SetItemDes(des);
        view.SetItemPrice(model.salePrice);

        Show();
    }

    public void Hide()
    {
        view.Hide();
        index = 0;
    }

    public void Show()
    {
        view.Show();
    }

    private void OnClickItemSellButton() => OnItemSell?.Invoke(index);
}
