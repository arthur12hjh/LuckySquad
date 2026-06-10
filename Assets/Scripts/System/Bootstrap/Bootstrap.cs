using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        CreateManagers();
    }

    private void Start()
    {
        SceneManager.LoadScene("Logo");
    }

    void CreateManagers()
    {
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("Addressables").AddComponent<AddressablesManager>();
        new GameObject("Stage").AddComponent<StageManager>();
    }
}
