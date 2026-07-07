using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HUDView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI monsterCountText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI gameTimeText;
    [SerializeField] private Slider expSlider;

    private PlayerStats playerStats;

    private void Start()
    {
        playerStats = InGameManager.Instance.GetPlayerStats();

        playerStats.OnChanged += UpdateGoldUI;
        playerStats.OnChanged += UpdateExpUI;
        playerStats.OnChanged += UpdateLevelUI;

        InGameManager.Instance.OnTimeChange += UpdateGameTimeUI;

        expSlider.maxValue = playerStats.BaseExp;
    }

    private void Update()
    {
        monsterCountText.text = $"{InGameManager.Instance.monsterCount}";
    }

    private void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnChanged -= UpdateGoldUI;
            playerStats.OnChanged -= UpdateExpUI;
            playerStats.OnChanged -= UpdateLevelUI;
        }

        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.OnTimeChange -= UpdateGameTimeUI;
        }
    }

    void UpdateExpUI()
    {
        expSlider.value = playerStats.CurrentExp % expSlider.maxValue;
        Debug.Log("경험치 변경 호출");
    }

    void UpdateGoldUI()
    {
       goldText.text = playerStats.currentGold.ToString();
    }

    void UpdateLevelUI()
    {
       levelText.text = $"Lv : {playerStats.Level}";
    }

    void UpdateGameTimeUI(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        gameTimeText.text = $" {minutes:00}:{seconds:00}";
    }

}
