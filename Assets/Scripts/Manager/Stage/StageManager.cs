using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private uint currentStageIndex;                      // 현재 스테이지
    private int currentWaveIndex;                       // 현재 웨이브
    private Dictionary<int, StageRef> stageDatas;    // 스테이지 데이터 저장용

    private int prevTimer = 0;
    private int bossIndex = 0;

    public event Action<WaveData> OnWave;
    public event Action<GameObject> OnBoss;

    private bool atOnce = true;

    [SerializeField]
    private StageRef currentStageData;                  // 현재 스테이지 데이터

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        InGameManager.Instance.OnTimeChange += HandleTimeChange;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        currentStageData = null;

        if (InGameManager.Instance != null)
            InGameManager.Instance.OnTimeChange -= HandleTimeChange;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void HandleTimeChange(int currentCount)
    {
        currentStageData.StageTime = currentCount;

        if (atOnce)
        {
            Debug.Log("Wave Called");
            // 웨이브를 만들면 그 웨이브에 필요한 구조체를 넘겨줌
            OnWave?.Invoke(currentStageData.CurrentWaveData);
            atOnce = false;
        }

        // 보스 (5분)
        if (currentStageData.StageTime == 150 || currentStageData.StageTime == 300)
        {
            Debug.Log("Boss Cerate");

            OnBoss?.Invoke(currentStageData.Boss[bossIndex]);
            bossIndex++;
            return;
        }

        // 웨이브 (매 분 0초)
        if (currentStageData.StageTime < 300 && currentStageData.StageTime % 60 == 0)
        {
            Debug.Log("Wave Called");
            currentStageData.WaveIndex++;
            // 웨이브를 만들면 그 웨이브에 필요한 구조체를 넘겨줌
            OnWave?.Invoke(currentStageData.CurrentWaveData);
        }
    }

    private void StageDateLoad()
    {
        string current_Stage = "Stage" + currentStageIndex;
        currentStageData = AddressablesManager.Instance.GetLabelObject<StageRef>($"Stage{currentStageIndex}", "ref", current_Stage);
    }

    public void StageSetting()
    {
        currentStageIndex = GameManager.Instance.currentStage;
        currentWaveIndex = 0;

        if (currentStageIndex >= 1)
            StageDateLoad();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StageSetting();
    }

    private void CheckWaveSpawnTime()
    {

    }

    private void CheckBossSpawnTime()
    {

    }
}
