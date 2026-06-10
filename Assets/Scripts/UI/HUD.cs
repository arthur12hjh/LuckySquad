using TMPro;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI goldText;
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
    }

    private void OnDestroy()
    {
        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.OnTimeChange -= UpdateGameTimeUI;
        }
    }

    void UpdateExpUI()
    {
       expSlider.value = playerStats.CurrentExp;
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
