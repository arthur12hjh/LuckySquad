using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        CreateManagers();
        SceneManager.LoadScene("Logo");
    }

    void CreateManagers()
    {
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("Addressables").AddComponent<AddressablesManager>();
    }
}
