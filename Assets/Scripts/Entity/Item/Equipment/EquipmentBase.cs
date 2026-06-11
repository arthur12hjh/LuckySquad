using UnityEngine;

public abstract class EquipmentBase : BaseItem
{
    public bool Active { get { return IsActive; } }

    protected bool              IsActive = false;
    protected float             Range = 0f;

    public virtual void Update_Directation(Vector2 dir)
    {

    }

    public void UseItem()
    {
        if(bIsUseItem())
        {
            UseItemLogic();
        }
    }

    public void LevelUp()
    {
        if(bIsLevelUpItem())
        {
            level++;
            SettingLevelData();
        }
            
    }

    protected virtual bool bIsUseItem()
    {
        return true;
    }

    protected virtual bool bIsLevelUpItem()
    {
        if (level == info.MaxLevel)
            return false;

        return true;
    }

    protected virtual void UseItemLogic()
    {

    }

    protected virtual void SettingLevelData()
    {

    }
}
