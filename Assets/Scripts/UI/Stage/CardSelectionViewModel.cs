using Item;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSelectionViewModel : BaseViewModel
{
    private List<CardViewModel> Cards;

    public event Action OnOpened;
    public event Action OnClosed;

    private readonly int maxCards = 3;

    public override void Initialize()
    {
        Cards = new List<CardViewModel>();

        for (int i = 0; i < maxCards; i++)
        {
            CardViewModel card = new CardViewModel();
            if(null != card)
            {
                card.Initialize();
                card.OnSelected += OnCardSelected;
                Cards.Add(card);
            }
        }

        PlayerStats playerStats = InGameManager.Instance.GetPlayerStats();
        if(playerStats != null) 
        {
            playerStats.OnChangedLevel += OpenCards;
        }
    
    }

    private void OnCardSelected()
    {
        CloseCards();
    }

    public CardViewModel GetCardVM(int Index)
    {
        if (maxCards <= Index)
            return null;

        return Cards[Index]; 
    }

    public void OpenCards()
    {
        List<Tuple<int, int, ItemData>> items = StageManager.Instance.Get_RandomItem();
        int iSize = items.Count;
        for (int i = 0; i < iSize; i++)
        {
            Cards[i].SetData(items[i]);
            Cards[i].Open();
        }

        OnOpened?.Invoke();

        InGameManager.Instance.StopGame();
    }

    public void CloseCards()
    {
        OnClosed?.Invoke();

        foreach (var card in Cards)
        {
            card.Close();
        }

        InGameManager.Instance.ResumeGame();
    }

}
