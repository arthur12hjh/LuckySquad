using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private List<Card> cards;
    
    private readonly int maxCards = 3;
    private bool isCardOpen = false;
    void Start()
    {
        foreach (Card card in cards)
        {
            card.onSelected += CloseCard;
        }
    }

    private void Update()
    {
        if(isCardOpen)
        {
            foreach (Card card in cards)
            {
                card.gameObject.SetActive(false);
            }
        }
    }

    public void OpenCard()
    {
        foreach (Card card in cards)
        {
           // card.SetData(Random.Range(0, 2), Random.Range(0, 3));
            card.gameObject.SetActive(true);
        }
    }

    public void CloseCard(Card SelectCard)
    {
        foreach (Card card in cards)
        {
            card.gameObject.SetActive(false);
        }
    }

}
