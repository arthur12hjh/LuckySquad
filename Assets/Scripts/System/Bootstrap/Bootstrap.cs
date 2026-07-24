using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private PlayerStatsRef playerStatsRef;
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
        var gameManager = new GameObject("GameManager").AddComponent<GameManager>();
        gameManager._playerStatsRef = playerStatsRef;
        new GameObject("Addressables").AddComponent<AddressablesManager>();
        new GameObject("Audio").AddComponent<AudioManager>();
    }
}