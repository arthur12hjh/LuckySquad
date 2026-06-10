using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.InputSystem;

public class Logo : MonoBehaviour
{
    [SerializeField]
    private GameObject text;

    private InputAction action;

    private void Start()
    {
        action = new InputAction(type: InputActionType.Button);

        action.AddBinding("<Touchscreen>/primaryTouch/press");

        action.AddBinding("<Mouse>/leftButton");

        action.canceled += ChangeScene;

        action.Enable();
    }

    private void ChangeScene(InputAction.CallbackContext ctx)
    {
        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);
        SceneManager.LoadScene("Loading");
    }

    private void OnDisable()
    {
        action.canceled -= ChangeScene;
    }
}
