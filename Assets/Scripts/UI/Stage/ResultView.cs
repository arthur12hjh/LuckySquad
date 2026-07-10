using UnityEngine;
using UnityEngine.UI;

public class ResultView : BaseView
{
    [SerializeField] Button ExitButton;
    [SerializeField] GameObject Panel;

    private void Start()
    {
        if(ExitButton != null)
            ExitButton.onClick.AddListener(InGameManager.Instance.EndStage);
        InGameManager.Instance.OnGameClear += Show;
    }

    public override void Show()
    {
        if(Panel != null)
            Panel.SetActive(true);
    }

    private void OnDestroy()
    {
        if(InGameManager.Instance != null)
            InGameManager.Instance.OnGameClear -= Show;
    }
}
