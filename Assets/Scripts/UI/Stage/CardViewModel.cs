using Item;
using NUnit.Framework.Interfaces;
using System;
using UnityEngine;

public class CardViewModel : BaseViewModel
{
    private const int GradeCount = 5; // Normal < Rare < Epic < Unique < Legendary

    public event Action OnSelected;
    public event Action OnOpened;
    public event Action OnClosed;

    private ItemData itemData;
    int cardGrade;

    public override void Initialize() { }

    public override void Release() { }

    public void SetData(Tuple<int, int, ItemData> item)
    {
        itemData = item.Item3;
        cardGrade = UnityEngine.Random.Range(0, GradeCount);
  
        // currentLevel
        //BindSprites();
    }

    public ItemData GetItemData()
    {
        return itemData;
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
