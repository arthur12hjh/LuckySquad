using UnityEngine;

public class BaseView : MonoBehaviour
{
    public virtual void Show()
    {
        OnShow();
    }
    public virtual void Hide()
    {
        OnHide();
    }

    public virtual void Bind(BaseViewModel baseViewModel) { }

    public virtual void OnShow() { gameObject.SetActive(true); }
    public virtual void OnHide() { gameObject.SetActive(false); }
}
