using Item;
using UnityEngine;

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

    protected SpriteRenderer    spriteRenderer = null;
    protected Sprite[]          spriteTexs = null;

    protected int               level = 1;
    protected ItemData          info;

    [SerializeField] string     SpriteTextureUrl;


    public virtual void Initalize(ItemData Data)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteTexs = Resources.LoadAll<Sprite>(SpriteTextureUrl);

        info = Data;
    }
}
