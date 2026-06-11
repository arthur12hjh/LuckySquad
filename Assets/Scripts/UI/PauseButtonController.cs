using UnityEngine;
using UnityEngine.UI;

public class PauseButtonController : MonoBehaviour
{
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button pauseUICloseButton;
    [SerializeField] private PopupPanel pauseUI;
    [SerializeField] private GameObject dimPanel;

    private void Start()
    {
        pauseButton.onClick.AddListener(OnPauseClick);
        pauseUICloseButton.onClick.AddListener(OnResumeClick);
    }

    private void OnPauseClick()
    {
        pauseUI.Open();
        dimPanel.SetActive(true);
        InGameManager.Instance.StopGame();
    }

    public void OnResumeClick()
    {
        pauseUI.Close();
        dimPanel.SetActive(false);
        InGameManager.Instance.ResumeGame();
    }
}
