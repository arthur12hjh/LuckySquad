using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private Enums.SceneType sceneType;
    private int currentStege = -1;

    void Start()
    {
        string sceneName = GameManager.Instance.currentSceneType.ToString();

        currentStege = GameManager.Instance.currentStage;

        if (currentStege == -1)
            StartCoroutine(SceneChange(sceneName));
        else
            StartCoroutine(StageChange($"{sceneName}_{currentStege}"));
    }

    // 로비, 스토어 등등
    private IEnumerator SceneChange(string sceneName)
    {
        yield return StartCoroutine(LoadSceneObject(sceneName));
        SceneManager.LoadScene(sceneName);
    }

    // 스테이지
    private IEnumerator StageChange(string sceneName)
    {
        yield return StartCoroutine(LoadSceneObject(sceneName));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneObject(string sceneName)
    {
        yield return AddressablesManager.Instance.LoadLabel<GameObject>($"{sceneName}_obj");
        yield return AddressablesManager.Instance.LoadLabel<AudioClip>($"{sceneName}_bmg");
        yield return AddressablesManager.Instance.LoadLabel<Sprite>($"{sceneName}_img");
    }
    


}
