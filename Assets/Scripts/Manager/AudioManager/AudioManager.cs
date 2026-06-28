using System.Collections.Generic;
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

    [Header("Bmg Clips")]
    [SerializeField] private AudioClip bmgAudio; // 배경 음악

    [Header("Sfx Clips")]
    [SerializeField] private List<AudioClip> playerSfx; // 플레이어 효과음
    [SerializeField] private List<AudioClip> stageSfx; // 스테이지 모든 효과음

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
    }

    private void SettingScene()
    {
        currentStegeType = GameManager.Instance.currentSceneType;
        currentStageIndex = GameManager.Instance.currentStage;

        if (currentStageIndex >= 1)
            StageSfxLoad($"{currentStegeType.ToString()}{currentStageIndex}");
        else
            NomalSfxLoad(currentStegeType.ToString());
    } 

    public void PlayerSfxload()
    {
        Debug.Log("플레이어 사운드");
        playerSfx = AddressablesManager.Instance.GetLabelDictionary<AudioClip>("Player", "sound");
    }

    private void NomalSfxLoad(string SceneName)
    {
        bmgAudio = AddressablesManager.Instance.GetLabelObject<AudioClip>(SceneName, "sound", $"{SceneName}Bmg");
        stageSfx = AddressablesManager.Instance.GetLabelDictionary<AudioClip>(SceneName, "sound");
    }

    private void StageSfxLoad(string SceneName)
    {
        bmgAudio = AddressablesManager.Instance.GetLabelObject<AudioClip>(SceneName, "sound", $"{SceneName}Bmg");
        stageSfx = AddressablesManager.Instance.GetLabelDictionary<AudioClip>(SceneName, "sound");
    }

}
