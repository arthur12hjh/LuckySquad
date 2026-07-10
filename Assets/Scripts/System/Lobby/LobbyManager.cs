using System;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [Serializable]
    private class ScreenEntry
    {
        public LobbyScreen SceneType;
        public BaseView SreenView;
    }

    [SerializeField] private List<ScreenEntry> _screens = new();

    private LobbyScreen currentScreen;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _screens[(int)(LobbyScreen.Stage)].SreenView.Bind(new StageViewModel());
        _screens[(int)(LobbyScreen.Setting)].SreenView.Bind(new SettingViewModel());

        _screens[(int)(LobbyScreen.Stage)].SreenView.Show();
    }

    public void Start()
    {
        AudioManager.Instance.PlayBGM();
    }

    public void ScreenChage(LobbyScreen sceneType)
    {
        if(currentScreen != sceneType)
        {
            _screens[(int)currentScreen].SreenView.Hide();
            _screens[(int)sceneType].SreenView.Show();
            currentScreen = sceneType;
        }
    }
}
