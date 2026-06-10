using DG.Tweening;
using Item;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Image gradeImage;

    [SerializeField] private Sprite[] itemSprites;
    [SerializeField] private Sprite[] gradeSprites;

    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Button button;

    public Action<Card> onSelected;

    DataManager dataManager;

    int cardID;
    int cardGrade;
    string cardName;
    int currentLevel;

    private void Start()
    {
        dataManager = DataManager.Instance;
    }

    public void Initialize(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public void OnClick()
    {
        onSelected?.Invoke(this);
    }

    public void SetData(int id, int grade)
    {
        cardID = id;
        cardGrade = grade;

        // currentLevel
        BindSprites();
    }

    void BindSprites()
    {
        ItemData? Item = dataManager.FindItemData(cardID);

        if(null == Item)
        {
            Debug.LogError("Item data not found for ID: " + cardID);
            return;
        }

        // 값 대입
        cardName = Item.szName;
       

        if (itemImage != null && itemSprites != null && cardID >= 0 && cardID < itemSprites.Length)
            itemImage.sprite = itemSprites[cardID];

        if (gradeImage != null && gradeSprites != null && cardGrade >= 0 && cardGrade < gradeSprites.Length)
            gradeImage.sprite = gradeSprites[cardGrade];
    }

    
}
