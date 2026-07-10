using Item;
using UnityEngine;
using UnityEngine.U2D;

public static class ItemFactory
{
    static public GameObject AbstractCreateItem<T>(int iItemID)
    where T : BaseItem, new()
    {
        GameObject gameObject = new GameObject("Item");
        gameObject.AddComponent<T>();

        ItemData data = DataManager.Instance.FindItemData(iItemID);
        if (data != null && data is ItemData Iteminfo)
        {
            gameObject.GetComponent<T>().Initalize(Iteminfo);
        }

        return gameObject;
    }

    static public GameObject AbstractCreateItem(GameObject prefab, Transform parent = null, int iItemID = 0)
    {
        if (iItemID == 0)
            return null;

        var obj = GameObject.Instantiate(prefab, parent);

        ItemData data = DataManager.Instance.FindItemData(iItemID);
        if (data != null && data is ItemData Iteminfo)
        {
            obj.GetComponent<BaseItem>().Initalize(Iteminfo);
        }

        return obj;
    }
}

public abstract class BaseItem : MonoBehaviour
{
    public    ItemData          ItemData {
        get { return info; }
    }

    protected SpriteRenderer    spriteRenderer = null;
    protected Sprite[]          spriteTexs = null;

    public int                  level { get; protected set; } = 0;
    protected ItemData          info;

    public virtual void Initalize(ItemData Data)
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        level = 0;

        if(Data.TextureName != "")
        {
            var sprite = AddressablesManager.Instance.GetCommon<SpriteAtlas>(Data.TextureName);
            if (sprite != null)
            {
                spriteTexs = new Sprite[sprite.spriteCount];
                sprite.GetSprites(spriteTexs);
            }
        }
   
        info = Data;
    }
}
