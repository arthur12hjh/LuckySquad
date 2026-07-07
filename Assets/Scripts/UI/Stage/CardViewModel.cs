using Item;
using System;
using UnityEngine;

public class CardViewModel : BaseViewModel
{
    private const int GradeCount = 5; // Normal < Rare < Epic < Unique < Legendary

    public event Action OnSelected;
    public event Action OnOpened;
    public event Action OnClosed;

    ItemData itemData;

    public override void Initialize() { }

    public override void Release() { }

    int cardID;
    int cardGrade;
    string cardName;
    int currentLevel;

    public void SetData()
    {
        itemData = DataManager.Instance != null ? DataManager.Instance.GetRandomItemData() : null;
        if (itemData == null)
        {
            Debug.LogError("Failed to roll random item data for card");
            return;
        }

        cardID = itemData.iID;
        cardGrade = UnityEngine.Random.Range(0, GradeCount);

        // currentLevel
        //BindSprites();
    }

    public ItemData GetItemData()
    {
        SetData();
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
