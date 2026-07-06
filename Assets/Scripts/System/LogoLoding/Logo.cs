using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Logo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressMessageText;

    private InputAction action;

    private float minAlpha = 0.3f;
    private float textFadeDuration = 1.5f;

    private void OnEnable()
    {
        // InputAction 생성 (빌드 안정성 위해 OnEnable 권장)
        action = new InputAction(type: InputActionType.Button);

        action.AddBinding("<Mouse>/leftButton");
        action.AddBinding("<Touchscreen>/primaryTouch/press");

        // performed 사용 (canceled보다 안정적)
        action.performed += OnPressed;

        action.Enable();
    }

    private void Start()
    {
        Debug.Log("[Logo] Start Enter");

        if (pressMessageText != null)
        {
            Debug.Log("[Logo] Text Fade Start");

            pressMessageText.DOFade(minAlpha, textFadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);

            Debug.Log("[Logo] Text Fade Running");
        }
        else
        {
            Debug.LogWarning("[Logo] pressMessageText is NULL");
        }

        Debug.Log("[Logo] Start End");
    }

    private void OnPressed(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);

        Debug.Log("Logo clicked");

        // 중복 입력 방지
        action.Disable();

        // 안전한 씬 전환 (프레임 보장)
        SceneManager.LoadScene("Loading");
    }

    private void OnDisable()
    {
        if (action != null)
        {
            action.performed -= OnPressed;
            action.Disable();
        }

        if (pressMessageText != null)
        {
            pressMessageText.DOKill();
        }
    }
}