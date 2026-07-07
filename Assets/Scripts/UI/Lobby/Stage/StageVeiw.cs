using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageVeiw : BaseView
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private Button prevStageButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button StageStartButton;

    private StageViewModel _viewModel;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is StageViewModel stageView)
        {
            _viewModel = stageView;
            _viewModel.Initialize();

            prevStageButton.onClick.AddListener(_viewModel.PrevStage);
            nextStageButton.onClick.AddListener(_viewModel.NextStage);
            StageStartButton.onClick.AddListener(_viewModel.StartStage);
            _viewModel.OnStageChanged += OnStageChanged;
        }
        else
        {
            Debug.Log("잘못된 ViewModel Binding");
            return;
        }
    }

    private void OnStageChanged(uint stageIndex)
    {
        stageText.text = $"STAGE {(stageIndex)}";
    }

    private void OnDestroy()
    {
        prevStageButton.onClick.RemoveListener(_viewModel.PrevStage);
        nextStageButton.onClick.RemoveListener(_viewModel.NextStage);
        StageStartButton.onClick.RemoveListener(_viewModel.StartStage);
        _viewModel.OnStageChanged -= OnStageChanged;
    }
}
