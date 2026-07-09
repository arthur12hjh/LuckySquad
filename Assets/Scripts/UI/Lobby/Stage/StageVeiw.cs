using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageVeiw : BaseView
{
    [SerializeField] private TextMeshProUGUI stageText;
    [SerializeField] private TextMeshProUGUI stageName;
    [SerializeField] private Button prevStageButton;
    [SerializeField] private Button nextStageButton;
    [SerializeField] private Button StageStartButton;

    private StageViewModel _viewModel;

    private void Awake()
    { 
        if (null == stageText || null == stageName || null == prevStageButton
            || null == nextStageButton || null == StageStartButton)
        {
            Debug.LogError("Failed StageView Bind");
        }
    }

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
        // TODO stageData 에서 받아오도록 수정
        stageName.text = "폐쇄구역: 제4교차로";
    }

    private void OnDestroy()
    {
        prevStageButton.onClick.RemoveListener(_viewModel.PrevStage);
        nextStageButton.onClick.RemoveListener(_viewModel.NextStage);
        StageStartButton.onClick.RemoveListener(_viewModel.StartStage);
        _viewModel.OnStageChanged -= OnStageChanged;
    }
}
