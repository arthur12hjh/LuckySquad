using UnityEngine;

public class WeaponSelectEvent
{
    public int SlotIdx { get; private set; }
    public int ItemID { get; private set; }

    public WeaponSelectEvent(int slotIdx, int itemID)
    {
        SlotIdx = slotIdx;
        ItemID = itemID;
    }
}
