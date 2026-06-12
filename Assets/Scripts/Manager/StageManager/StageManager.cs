using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private int currentStageIndex;                      // 현재 스테이지
    private int currentWaveIndex;                       // 현재 웨이브
    private Dictionary<int, StageRef> stageDatas;    // 스테이지 데이터 저장용

    private int prevTimer = 0;
    private int bossIndex = 0;

    public event Action<StageRef> OnWave;
    public event Action<GameObject> OnBoss;

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
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        InGameManager.Instance.OnTimeChange += HandleTimeChange;
    }

    private void OnDisable()
    {
        currentStageData = null;
        InGameManager.Instance.OnTimeChange -= HandleTimeChange;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
    }

    private void HandleTimeChange(int currentCount)
    {
        currentStageData.StageTime = currentCount;

        // 보스 (5분)
        if (currentStageData.StageTime == 150 || currentStageData.StageTime == 300)
        {
            OnBoss?.Invoke(currentStageData.Boss[bossIndex]);
            bossIndex++;
            return;
        }

        // 웨이브 (매 분 0초)
        if (currentStageData.StageTime < 300 && currentStageData.StageTime % 60 == 0)
        {
            currentStageData.WaveIndex++;
            // 웨이브를 만들면 그 웨이브에 필요한 구조체를 넘겨줌
            OnWave?.Invoke(currentStageData);
        }
    }

    private void StageDateLoad()
    {
        var handle = AddressablesManager.Instance.LoadLabel<StageRef>($"Stage{currentStageIndex}", "ref");

        handle.Completed += h =>
        {
            OnStageLoaded(h);
        };
    }

    private void OnStageLoaded(AsyncOperationHandle<IList<StageRef>> h)
    {
        if (h.Status != AsyncOperationStatus.Succeeded)
            return;

        stageDatas = new Dictionary<int, StageRef>();

        foreach (var stage in h.Result)
            stageDatas[stage.StageIndex] = stage;

        currentStageData = stageDatas[currentStageIndex];
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
