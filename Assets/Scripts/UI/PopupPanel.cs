using DG.Tweening;
using UnityEngine;

public class PopupPanel : MonoBehaviour
{
    [SerializeField] protected float duration = 0.2f;

    public virtual void Open()
    {
        gameObject.SetActive(true);
        transform.localScale = Vector3.zero;
        transform.DOScale(1f, duration)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public virtual void Close()
    {
        transform.DOScale(0f, duration)
            .SetEase(Ease.InBack)
            .SetUpdate(true)
            .OnComplete(() => gameObject.SetActive(false));
    }
}
