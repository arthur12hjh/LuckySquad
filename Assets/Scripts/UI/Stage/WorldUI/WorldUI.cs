using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WorldUI : MonoBehaviour
{
    [SerializeField] private Slider HpBar;
    [SerializeField] private Vector3 playerHpBarOffset;

    private Transform playerTransform;
    private Transform HpBarTransform;
    private PlayerStats playerStats;

    void Start()
    {
        playerTransform = InGameManager.Instance.GetPlayerTransform();
        playerStats = InGameManager.Instance.GetPlayerStats();

        playerStats.OnChanged += UpdatePlayerHp;

        HpBarTransform = HpBar.transform;
    }

    void LateUpdate()
    {
        HpBarTransform.position = playerTransform.position + playerHpBarOffset;
    }

    private void UpdatePlayerHp()
    {
        HpBar.maxValue = playerStats.MaxHp;
        HpBar.value = playerStats.CurrentHp;
    }
}
