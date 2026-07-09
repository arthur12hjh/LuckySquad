using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectionView : BaseView
{
    [SerializeField] private GameObject dimPanel;
    [SerializeField] private List<CardView> _cards;
    [SerializeField] private Button testButton;

    private CardSelectionViewModel _viewModel;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if(baseViewModel is CardSelectionViewModel viewModel)
        {
            _viewModel = viewModel;
            _viewModel.OnOpened += Open;
            _viewModel.OnClosed += Close;
            testButton.onClick.AddListener(_viewModel.OpenCards);
            for (int i = 0; i < _cards.Count; i++)
                _cards[i].Bind(_viewModel.GetCardVM(i));
        }
    }

    public void Start()
    {
        CardSelectionViewModel instance = new CardSelectionViewModel();
        if (null != instance)
        {
            instance.Initialize();
            Bind(instance);
        }
    }

    private void Open()
    {
        dimPanel.gameObject.SetActive(true);

        foreach (var cardview in _cards)
        {
            cardview.gameObject.SetActive(true);
            cardview.Open();
        }
    }

    private void Close()
    {
        dimPanel.gameObject.SetActive(false);

        foreach (var cardview in _cards)
        {
            cardview.gameObject.SetActive(false);
        }
    }
}
