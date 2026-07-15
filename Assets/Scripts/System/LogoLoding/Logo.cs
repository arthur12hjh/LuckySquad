using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI pressMessageText;
    [SerializeField] private Button button;

    private InputAction action;

    private float minAlpha = 0.3f;
    private float textFadeDuration = 1.5f;

    private bool isReady = false;

    private void OnEnable()
    {
        // InputAction 생성(빌드 안정성 위해 OnEnable 권장)
        action = new InputAction(type: InputActionType.Button);

#if UNITY_EDITOR
        action.AddBinding("<Mouse>/leftButton");
#endif
        action.AddBinding("<Touchscreen>/primaryTouch/press");
        action.performed += OnPressed;

        action.Enable();
    }

    private void Start()
    {
#if UNITY_EDITOR
        button.gameObject.SetActive(false);
#elif UNITY_ANDROID
        pressMessageText.gameObject.SetActive(false);
#endif
        StartCoroutine(LoadCommon());

        GoogleSigeinManager.Instance.OnTutch += LoginSuccess;
    }

    private IEnumerator LoadCommon()
    {
        yield return AddressablesManager.Instance.LoadCommon();
#if UNITY_EDITOR
        isReady = true;
#endif

    }

    private void LoginSuccess()
    {
#if UNITY_ANDROID
        button.gameObject.SetActive(false);
        if (pressMessageText != null)
        {
            pressMessageText.gameObject.SetActive(true);
            pressMessageText.DOFade(minAlpha, textFadeDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
        isReady = true;
#endif

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