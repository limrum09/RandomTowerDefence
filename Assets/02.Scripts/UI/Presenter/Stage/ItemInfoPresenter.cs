using System;
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
        if (getModel == null)
            return;

        model = getModel;
        index = getIndex;

        Sprite icon = ResourceCache.Load<Sprite>($"Item/Images/{model.iconUID}");
        view.SetIcon(icon);

        string name = Managers.Local.GetString("Item", model.stringKey);
        view.SetItemName(name);
        view.SetItemGrade(model.grade);
        view.SetItemScope(model.scopeRange);
        view.SetItemTarget(model.target);

        string des = string.Format(Managers.Local.GetString("Item", model.itemDesc), Mathf.Abs(model.value));
        view.SetItemDes(des);
        view.SetItemPrice(model.salePrice);
    }

    public void Hide()
    {
        view.Hide();
        index = 0;
    }

    private void OnClickItemSellButton() => OnItemSell?.Invoke(index);
}
