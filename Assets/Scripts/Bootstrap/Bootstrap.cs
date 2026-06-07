using UnityEngine;
using UnityEngine.SceneManagement;

public static class Bootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        CreateManagers();
    }

    static void CreateManagers()
    {
        new GameObject("GameManager").AddComponent<GameManager>();
        new GameObject("Addressables").AddComponent<AddressablesManager>();
    }
}
