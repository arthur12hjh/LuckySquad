using System.Collections.Generic;
using System.Linq;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    private AudioSource bgmAudioSource;

    private AudioClip bmgAudio; // 배경 음악
    private List<AudioClip> playerSfx = new(); // 플레이어 효과음
    private List<AudioClip> stageSfx = new(); // 스테이지 모든 효과음

    private uint currentStageIndex = 0;
    private Enums.SceneType currentStegeType;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        bgmAudioSource = gameObject.AddComponent<AudioSource>();
    }

    public void SettingScene()
    {
        currentStegeType = GameManager.Instance.currentSceneType;
        currentStageIndex = GameManager.Instance.currentStage;

        if(bgmAudioSource.isPlaying)
        {
            bgmAudioSource.Stop();
            bgmAudioSource.clip = null;
        }

        if (currentStageIndex >= 1)
            StageSfxLoad($"{currentStegeType.ToString()}{currentStageIndex}");
        else
            NomalSfxLoad(currentStegeType.ToString());
    }

    public void PlayBGM()
    {
        bgmAudioSource.clip = bmgAudio;
        bgmAudioSource.loop = true;
        bgmAudioSource.Play();
    }

    public void PlayerSfxload()
    {
        Debug.Log("플레이어 사운드");
        playerSfx = AddressablesManager.Instance.GetLabelList<AudioClip>("PlayerSound");
    }

    private void NomalSfxLoad(string SceneName)
    {
        // 뒤에 오디오 이름을 넣으면 된다.
        bmgAudio = AddressablesManager.Instance.GetLabelDictionary<AudioClip>(SceneName, "TestSound");
        stageSfx = AddressablesManager.Instance.GetLabelList<AudioClip>(SceneName);
    }

    private void StageSfxLoad(string SceneName)
    {
        // bmgAudio = AddressablesManager.Instance.GetLabelDictionary<AudioClip>(SceneName, "");
        stageSfx = AddressablesManager.Instance.GetLabelList<AudioClip>(SceneName);
    }

}
