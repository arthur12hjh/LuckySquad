using DG.Tweening;
using Item;
using System;
using TMPro;
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
        if (Item == null)
        {
            Debug.LogError("Item data not found for CardView");
            return;
        }

        int grade = _viewModel.GetGrade();
        if (cardRefs == null || grade < 0 || grade >= cardRefs.Length)
        {
            Debug.LogError("No CardSO configured for grade: " + grade);
            return;
        }

        CardSO cardRef = cardRefs[grade];

        InfoText.text = Item.szName;

        var sprite = AddressablesManager.Instance.GetCommon<SpriteAtlas>(Item.TextureName);
        if (sprite != null)
        {
            itemSprites = new Sprite[sprite.spriteCount];
            sprite.GetSprites(itemSprites);
            itemImage.sprite = itemSprites[0];
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


        gradeImage.sprite = cardRef.GradeImage;
        //gradeText.text = cardRef.GradeName;
    }
    
}
