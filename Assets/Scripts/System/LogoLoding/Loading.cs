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


        //SceneManager.LoadScene(sceneName);
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
        float audioP = 0f;
        float imgP = 0f;
        float imgPA = 0f;

        var obj = AddressablesManager.Instance.LoadLabel<GameObject>(sceneName, "obj");
        //var audio = AddressablesManager.Instance.LoadLabel<AudioClip>(sceneName, "sound");
        // var img = AddressablesManager.Instance.LoadLabel<Sprite>(sceneName, "img");
        var imgA = AddressablesManager.Instance.LoadLabel<SpriteAtlas>(sceneName, "imgAtlas");

        while (true)
        {
            objP = obj.PercentComplete;
            //audioP = audio.PercentComplete;
            //imgP = img.PercentComplete;
            imgPA = imgA.PercentComplete;

            float total = (objP + audioP + imgP + imgPA ) / 4f;

            if (obj.IsDone &&  imgA.IsDone)// img.IsDone && && audio.IsDone
                break;

            yield return null;
        }

        // 안전 대기
        yield return obj;
        //yield return audio;
        //yield return img;
        yield return imgA;
    }

    private IEnumerator LoadStageObject(string sceneName)
    {
        float objP = 0f;
        float audioP = 0f;
        float imgP = 0f;
        float imgPA = 0f;
        float DataP = 0f;

        var obj = AddressablesManager.Instance.LoadLabel<GameObject>(sceneName, "obj");
        // var audio = AddressablesManager.Instance.LoadLabel<AudioClip>(sceneName, "sound");
        // var img = AddressablesManager.Instance.LoadLabel<Sprite>(sceneName, "img");
        // var imgA = AddressablesManager.Instance.LoadLabel<SpriteAtlas>(sceneName, "imgAtlas");
        var Data = AddressablesManager.Instance.LoadLabel<StageRef>(sceneName, "ref");

        while (true)
        {
            objP = obj.PercentComplete;
            // audioP = audio.PercentComplete;
            // imgP = img.PercentComplete;
            // imgPA = imgA.PercentComplete;
            DataP = Data.PercentComplete;

            float total = (objP + audioP + imgP + DataP + imgPA) / 5f;

            if (obj.IsDone && Data.IsDone) // && audio.IsDone && img.IsDone && imgA.IsDone
                break;

            yield return null;
        }

        // 안전 대기
        yield return obj;
        // yield return audio;
        // yield return img;
        // yield return imgA;
        yield return Data;
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
