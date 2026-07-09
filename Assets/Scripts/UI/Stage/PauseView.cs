using UnityEngine;
using UnityEngine.UI;

public class PauseView : BaseView
{
    [SerializeField] private PauseController PauseController;
    [SerializeField] private Button ExitButton;

    private void Start()
    {
        if (ExitButton == null)
            return;

        if (PauseController != null)
            ExitButton.onClick.AddListener(PauseController.OnResume);
        if (InGameManager.Instance != null)
            ExitButton.onClick.AddListener(InGameManager.Instance.EndStage);
    }

    public override void Bind(BaseViewModel baseViewModel) 
    {
        
    }
}
