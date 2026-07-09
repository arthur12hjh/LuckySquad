using UnityEngine;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button pauseUICloseButton;
    [SerializeField] private PopupPanel pauseUI;
    [SerializeField] private GameObject dimPanel;

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPause);
        pauseUICloseButton.onClick.AddListener(OnResume);
    }

    public void OnPause()
    {
        pauseUI.Open();
        dimPanel.SetActive(true);
        InGameManager.Instance.StopGame();
    }

    public void OnResume()
    {
        pauseUI.Close();
        dimPanel.SetActive(false);
        InGameManager.Instance.ResumeGame();
    }
}
