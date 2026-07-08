using UnityEngine;

public class MessageView : BaseView
{
    private MessageViewModel _viewModel;

    public override void Bind(BaseViewModel baseViewModel)
    {
        if (null != _viewModel)
            return;

        if (baseViewModel is MessageViewModel messageView)
        {
            _viewModel = messageView;
            _viewModel.Initialize();
        }
        else
        {
            Debug.Log("잘못된 ViewModel Binding");
            return;
        }
    }

}
