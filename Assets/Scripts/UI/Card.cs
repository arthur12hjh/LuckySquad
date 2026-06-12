using DG.Tweening;
using Item;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image gradeImage;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private Button button;

    [SerializeField] private Sprite[] itemSprites;

    [SerializeField] private CardSO[] cardRefs;

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
        rectTransform.DOKill();
        rectTransform.localScale = Vector2.one;
        rectTransform.DOPunchScale(Vector3.one *  0.2f, 0.3f, vibrato: 1, elasticity: 0.5f).SetUpdate(true);

        cardID = id;
        cardGrade = grade;

        // currentLevel
        BindSprites();
    }

    void BindSprites()
    {
        //ItemData Item = dataManager.FindItemData(cardID);
        CardSO cardRef = cardRefs[cardGrade];
        //if (null == Item)
        //{
        //    Debug.LogError("Item data not found for ID: " + cardID);
        //    return;
        //}

        // 값 대입
        //cardName = Item.szName;

        //if (itemImage != null && itemSprites != null && cardID >= 0 && cardID < itemSprites.Length)
        //    itemImage.sprite = itemSprites[cardID];

        gradeImage.sprite = cardRef.GradeImage;
        //gradeText.text = cardRef.GradeName;
    }
    
}
