using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class SettingView : BaseView
{
    [SerializeField] private Button BGMIcon;
    [SerializeField] private Button EffectsoundIcon;
    [SerializeField] private Scrollbar BGMscrollbar;
    [SerializeField] private Scrollbar Effectscrollbar;

    private SettingViewModel _viewModel;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is SettingViewModel stageView)
        {
            _viewModel = stageView;
            _viewModel.Initialize();

           // BGMIcon.onClick.AddListener();
        }
        else
        {
            Debug.Log("잘못된 SettingViewModel Binding");
            return;
        }
    }
}
