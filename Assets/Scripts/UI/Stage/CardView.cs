using DG.Tweening;
using Item;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CardView : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image itemImage;
    [SerializeField] private Image gradeImage;
    [SerializeField] private TextMeshProUGUI InfoText;
    [SerializeField] private TextMeshProUGUI LevelText;
    [SerializeField] private Button button;

    [SerializeField] private Sprite[] itemSprites;

    [SerializeField] private CardSO[] cardRefs;

    private CardViewModel _viewModel;

    public void Bind(CardViewModel cardViewModel)
    {
        if(cardViewModel is CardViewModel viewModel)
        {
            _viewModel = viewModel;
            //_viewModel.OnOpened += Open;
            button.onClick.AddListener(_viewModel.Selected);
        }
        else
        {
            Debug.Log("Failed to Bind CardView");
            return;
        }
    }

    public void Initialize(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }

    public void Open()
    {
        Refresh();
        rectTransform.DOKill();
        rectTransform.localScale = Vector2.one;
        rectTransform.DOPunchScale(Vector3.one * 0.2f, 0.3f, vibrato: 1, elasticity: 0.5f).SetUpdate(true);
    }

    public void Close()
    {
   
    }

    void Refresh()
    {
        // 값 대입

        ItemData Item = _viewModel.GetItemData();
        int grade = _viewModel.GetGrade();
        int itemLevel = _viewModel.GetItemLevel() + grade + 1;
        if (Item == null)
        {
            Debug.LogError("Item data not found for CardView");
            return;
        }

        if (cardRefs == null || grade < 0 || grade >= cardRefs.Length)
        {
            Debug.LogError("No CardSO configured for grade: " + grade);
            return;
        }

        LevelText.text = "Lv. " + itemLevel.ToString();
        InfoText.text = Item.szName;

        var sprite = AddressablesManager.Instance.GetCommon<SpriteAtlas>(Item.IconName);
        if (sprite != null)
        {
            itemSprites = new Sprite[sprite.spriteCount];
            sprite.GetSprites(itemSprites);
            itemImage.sprite = itemSprites[Math.Min(sprite.spriteCount - 1, itemLevel)];
        }
        else
       { 
            Debug.LogError("Failed to Get SpriteAtlas");
        }

        //if (itemImage != null && itemSprites != null && cardID >= 0 && cardID < itemSprites.Length)
        //    itemImage.sprite = itemSprites[cardID];

        //baseitem 보셈
        //Sprite sprite = AddressablesManager.Instance.GetCommon<SpriteAtlas>(Item.IconName);

        //if (sprite != null)
        //    itemImage.sprite = sprite;
        //else
        //    Debug.Log("Failed to Load icon");

        CardSO cardRef = cardRefs[grade];

        gradeImage.sprite = cardRef.GradeImage;
        //gradeText.text = cardRef.GradeName;
    }
    
}
