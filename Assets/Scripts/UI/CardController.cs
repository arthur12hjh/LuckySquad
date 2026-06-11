using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour
{
    [SerializeField] private List<Card> cards;
    [SerializeField] private GameObject dimPanel;

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
        dimPanel.SetActive(true);

        foreach (Card card in cards)
        {
            card.gameObject.SetActive(true);
            card.SetData(Random.Range(0, 2), Random.Range(0, 3));
            InGameManager.Instance.StopGame();
        }
    }

    public void CloseCard(Card SelectCard)
    {
        dimPanel.SetActive(false);

        foreach (Card card in cards)
        {
            card.gameObject.SetActive(false);
            InGameManager.Instance.ResumeGame();
        }
    }

}
