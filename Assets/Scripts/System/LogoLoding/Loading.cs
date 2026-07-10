using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

public class Loading : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loadingText;

    private Enums.SceneType sceneType;
    private uint currentStege = 0;

    void Start()
    {
        string sceneName = GameManager.Instance.currentSceneType.ToString();

        currentStege = GameManager.Instance.currentStage;

        if (currentStege == 0)
            StartCoroutine(SceneChange(sceneName));
        else
            StartCoroutine(StageChange($"{sceneName}{currentStege}"));
        StartCoroutine(AnimateLoadingText());
    }

    // 로비, 스토어 등등
    private IEnumerator SceneChange(string sceneName)
    {
        yield return StartCoroutine(LoadSceneObject(sceneName));
        AudioManager.Instance.SettingScene();
        SceneManager.LoadScene(sceneName);
        //SceneManager.LoadScene("Weapon");
    }

    // 스테이지
    private IEnumerator StageChange(string sceneName)
    {
        yield return StartCoroutine(LoadStageObject(sceneName));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator LoadSceneObject(string sceneName)
    {
        float objP = 0f;

        var obj = AddressablesManager.Instance.LoadLabel(sceneName);

        while (true)
        {
            objP = obj.PercentComplete;

            if (obj.IsDone)
                break;

            yield return null;
        }

        // 안전 대기
        yield return obj;
    }

    private IEnumerator LoadStageObject(string sceneName)
    {
        float objP = 0f;

        var obj = AddressablesManager.Instance.LoadLabel(sceneName);

        while (true)
        {
            objP = obj.PercentComplete;

            if (obj.IsDone) // && audio.IsDone && img.IsDone && imgA.IsDone
                break;

            yield return null;
        }

        // 안전 대기
        yield return obj;
    }

    IEnumerator AnimateLoadingText()
    {
        while (true)
        {
            loadingText.text = "Loading.";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading..";
            yield return new WaitForSeconds(0.5f);
            loadingText.text = "Loading...";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
