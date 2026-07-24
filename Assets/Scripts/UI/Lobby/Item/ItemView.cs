using UnityEngine;
using UnityEngine.UI;

public class ItemView : BaseView
{
    [SerializeField] private Image StartItemImage;
    [SerializeField] private Sprite[] StartItemImages;
    [SerializeField] private Button[] ItemSelectButtons;

    private ItemViewModel _viewModel;
    private int _selectedIndex;

    private void Start()
    {
        _selectedIndex = 1;
        StartItemImage.sprite = StartItemImages[_selectedIndex];
        for (int i = 0; i < ItemSelectButtons.Length; i++)
        {
            int index = i + 1;
            ItemSelectButtons[i].onClick.AddListener(() => SelectItem(index));
        }

        SelectItem(_selectedIndex);
        GameManager.Instance._playerStatsRef.StartItemIndex = 0;
    }

    private void SelectItem(int index)
    {
        _selectedIndex = index;
        GameManager.Instance._playerStatsRef.StartItemIndex = _selectedIndex;

        StartItemImage.sprite = StartItemImages[_selectedIndex - 1];
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
