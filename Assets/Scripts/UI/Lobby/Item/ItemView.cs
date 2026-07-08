using UnityEngine;

public class ItemView : BaseView
{
    private ItemViewModel _viewModel;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is ItemViewModel itemView)
        {
            _viewModel = itemView;
            _viewModel.Initialize();
        }
        else
        {
            Debug.Log("잘못된 ViewModel Binding");
            return;
        }
    }

}
