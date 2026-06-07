using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using TMPro;

public class LogoLoding : MonoBehaviour
{
    private bool isInitialized = false;
    [SerializeField]
    private GameObject text;

    private void Start()
    {
        AddressablesManager.Instance.OnInitialized += OnInitDone;
        AddressablesManager.Instance.Init();
    }

    private void OnInitDone()
    {
        AddressablesManager.Instance.OnInitialized -= OnInitDone;
        isInitialized = true;
        text.SetActive(true);
    }

    private void Update()
    {
        //if (isInitialized)
        //    SceneManager.LoadScene("Lobby");
    }
}
