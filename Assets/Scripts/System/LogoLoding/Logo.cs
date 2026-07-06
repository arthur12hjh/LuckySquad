using DG.Tweening;
using System.Collections;
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

    private bool isReady = false;

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
        if (pressMessageText != null)
        {
            pressMessageText.DOFade(minAlpha, textFadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        StartCoroutine(LoadCommon());

    }

    private IEnumerator LoadCommon()
    {
        yield return AddressablesManager.Instance.LoadCommon();
        isReady = true;
    }

    private void OnPressed(InputAction.CallbackContext ctx)
    {
        if (!isReady)
            return;

        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);

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