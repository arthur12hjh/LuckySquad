using Item;
using Unity.VisualScripting;

public class WeaponBase : EquipmentBase
{
    public WeaponData WeaponData { get; private set; }

    public override void Initalize(ItemData Data)
    {
       base.Initalize(Data);
       WeaponData = Data as WeaponData;
    }

    protected override bool bIsUseItem()
    {
        return true;
    }

    protected override bool bIsLevelUpItem()
    {
        if (WeaponData.MaxLevel <= level)
            return false;

        return true;
    }

    protected override void UseItemLogic()
    {

    }

    protected override void SettingLevelData()
    {
        if(level > 0)
            SerializationWeaponData();
    }

    private bool SerializationWeaponData()
    {
        if (info.MaxLevel < level)
            return false;

        if(spriteRenderer != null)
            spriteRenderer.sprite = spriteTexs[level - 1];

        return true;
    }
}
