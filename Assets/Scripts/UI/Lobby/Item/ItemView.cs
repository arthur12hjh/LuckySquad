using UnityEngine;
using UnityEngine.UI;

public class ItemView : BaseView
{
    [SerializeField] private Image StartItemImage;
    [SerializeField] private Sprite[] StartItemImages;
    [SerializeField] private Button[] ItemSelectButtons;
    private PlayerStatsRef playerStatsRef;
    private ItemViewModel _viewModel;
    private int _selectedIndex;

    private void Start()
    {
        _selectedIndex = 0;
        playerStatsRef.StartItemIndex = 0;
        StartItemImage.sprite = StartItemImages[_selectedIndex];
        for (int i = 0; i < ItemSelectButtons.Length; i++)
        {
            int index = i;
            ItemSelectButtons[i].onClick.AddListener(() => SelectItem(index));
        }

        SelectItem(_selectedIndex);
    }

    private void SelectItem(int index)
    {
        _selectedIndex = index;
        playerStatsRef.StartItemIndex = _selectedIndex;

        for (int i = 0; i < StartItemImages.Length; i++)
        {
            StartItemImage.sprite = StartItemImages[i];
        }
    }

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is ItemViewModel itemView)
        {
            _viewModel = itemView;
            _viewModel.Initialize();

        }
        else
        {
            Debug.Log("잘못된 ViewModel Binding");
            return;
        }
    }

}
