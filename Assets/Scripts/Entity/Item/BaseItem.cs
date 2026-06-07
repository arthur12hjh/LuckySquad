using UnityEngine;
using Item;

public static class ItemFactory
{
    static public GameObject AbstractCreateItem<T>(int iItemID)
    where T : BaseItem, new()
    {
        GameObject gameObject = new GameObject("Item");
        gameObject.AddComponent<T>();

        ItemData? data = DataManager.Instance.FindItemData(iItemID);
        if (data is ItemData Iteminfo)
        {
            gameObject.GetComponent<T>().Initalize(Iteminfo);
        }

        return gameObject;
    }
}

public abstract class BaseItem : MonoBehaviour
{
    public    ItemData          ItemData {
        get { return info; }
    }

    protected ItemData          info;
    public virtual void Initalize(ItemData Data)
    {
        info = Data;
    }
}
