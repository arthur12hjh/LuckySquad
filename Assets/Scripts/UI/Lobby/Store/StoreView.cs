using UnityEngine;

public class StoreView : BaseView
{
    private StoreViewModel _viewModel;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is StoreViewModel storeView)
        {
            _viewModel = storeView;
            _viewModel.Initialize();
        }
        else
        {
            Debug.Log("잘못된 ViewModel Binding");
            return;
        }
    }

}
