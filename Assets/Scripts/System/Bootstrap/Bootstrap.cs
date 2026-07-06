using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        CreateManagers();
    }

    private IEnumerator Start()
    {
        yield return null;

        SceneManager.LoadScene("Logo");

    }

    void CreateManagers()
    {
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("Addressables").AddComponent<AddressablesManager>();
        new GameObject("Audio").AddComponent<AudioManager>();
    }
}   