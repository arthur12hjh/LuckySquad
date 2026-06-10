using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI loadingText;

    private Enums.SceneType sceneType;
    private int currentStege = -1;

    void Start()
    {
        string sceneName = GameManager.Instance.currentSceneType.ToString();

        currentStege = GameManager.Instance.currentStage;


        SceneManager.LoadScene(sceneName);
        //if (currentStege == -1)
        //    StartCoroutine(SceneChange(sceneName));
        //else
        //    StartCoroutine(StageChange($"{sceneName}_{currentStege}"));

        //StartCoroutine(AnimateLoadingText());

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
        float objP = 0f;
        float audioP = 0f;
        float imgP = 0f;

        var obj = AddressablesManager.Instance.LoadLabel<GameObject>(sceneName, "obj");
        var audio = AddressablesManager.Instance.LoadLabel<AudioClip>(sceneName, "sound");
        var img = AddressablesManager.Instance.LoadLabel<Sprite>(sceneName, "img");

        while (true)
        {
            objP = obj.PercentComplete;
            audioP = audio.PercentComplete;
            imgP = img.PercentComplete;

            float total = (objP + audioP + imgP) / 3f;

            if (obj.IsDone && audio.IsDone && img.IsDone)
                break;

            yield return null;
        }

        // 안전 대기
        yield return obj;
        yield return audio;
        yield return img;
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
