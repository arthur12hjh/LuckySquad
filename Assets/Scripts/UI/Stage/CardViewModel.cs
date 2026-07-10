using Item;
using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class CardViewModel : BaseViewModel
{
    private const int GradeCount = 5; // Normal < Rare < Epic < Unique < Legendary
    private static readonly int[] GradeWeights = { 80, 10, 5, 2, 1 };

    public event Action OnSelected;
    public event Action OnOpened;
    public event Action OnClosed;

    private ItemData itemData;
    private int slotIndex;
    private int itemLevel;
    int cardGrade;


    public override void Initialize() { }

    public override void Release() { }

    public void SetData(Tuple<int, int, ItemData> item)
    {
        slotIndex = item.Item1;
        itemLevel = item.Item2;
        itemData = item.Item3;
        // 65 20 10 4 1 확률로 선택
        cardGrade = RollGrade();


        Debug.Log($"{slotIndex} : {itemData.szName}");
        // currentLevel
        //BindSprites();
    }

    private int RollGrade()
    {
        int totalWeight = 0;
        for (int i = 0; i < GradeWeights.Length; i++)
        {
            totalWeight += GradeWeights[i];
        }

        int roll = UnityEngine.Random.Range(0, totalWeight);
        int accumulated = 0;
        for (int i = 0; i < GradeWeights.Length; i++)
        {
            accumulated += GradeWeights[i];
            if (roll < accumulated)
            {
                return i;
            }
        }

        return GradeWeights.Length - 1;
    }

    public ItemData GetItemData()
    {
        return itemData;
    }

    public int GetItemLevel()
    {
        return itemLevel;
    }

    public int GetGrade()
    {
        return cardGrade;
    }

    public void Selected()
    {
        // 아이템 선택
        // Player 아이템 추가 로직
        OnSelected?.Invoke();
        for (int i = 0; i < cardGrade + 1; i++)
        {
            EventBus.Publish(new WeaponSelectEvent(slotIndex, itemData.iID));
        }
        Close();
    }

    public void Open()
    {
        OnOpened?.Invoke();
    }

    public void Close()
    {
        OnClosed?.Invoke();
    }
}
