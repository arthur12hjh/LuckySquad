using DG.Tweening;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    [SerializeField] private GameObject text;
    [SerializeField] private TextMeshProUGUI pressMessageText;

    private InputAction action;

    private float minAlpha = 0.3f;
    private float textFadeduration = 1.5f;

    private void Start()
    {
        action = new InputAction(type: InputActionType.Button);

        action.AddBinding("<Touchscreen>/primaryTouch/press");

        action.AddBinding("<Mouse>/leftButton");

        action.canceled += ChangeScene;

        action.Enable();

        pressMessageText.DOFade(minAlpha, textFadeduration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void ChangeScene(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);
        SceneManager.LoadScene("Loading");
    }

    private void OnDisable()
    {
        action.canceled -= ChangeScene;
        pressMessageText.DOKill();
    }
}
