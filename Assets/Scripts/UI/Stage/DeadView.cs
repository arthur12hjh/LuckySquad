using System;
using UnityEngine;
using UnityEngine.UI;

public class DeadView : MonoBehaviour
{
    [SerializeField] private Button LobbyReturnButton;
    [SerializeField] private GameObject DeadPopup;

    void Start()
    {
        PlayerStats playerStats = InGameManager.Instance.GetPlayerStats();

        if (null != LobbyReturnButton)
        {
            LobbyReturnButton.onClick.AddListener(InGameManager.Instance.EndStage);
            playerStats.OnDead += OnDeadPlayer;
        }

        if (null != DeadPopup)
        {
            DeadPopup.gameObject.SetActive(false);
        }
    }

    void OnDeadPlayer()
    { 
        // Dead UI
        if(DeadPopup != null)
            DeadPopup.gameObject.SetActive(true);
    }

}
