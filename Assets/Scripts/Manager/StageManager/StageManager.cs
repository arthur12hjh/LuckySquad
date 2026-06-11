using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

public class StageManager : MonoBehaviour
{
    private static StageManager instance;
    public static StageManager Instance => instance;

    private int currentStageIndex;                      // 현재 스테이지
    private int currentWaveIndex;                       // 현재 웨이브
    private Dictionary<int, StageRef> stageDatas;    // 스테이지 데이터 저장용

    private int stageTimer = 0;
    private int currentSecond = 0;

    private bool isWaveCheck = false;

    [SerializeField]
    private StageRef currentStageData;                  // 현재 스테이지 데이터

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

    private void Start()
    {
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        currentStageData = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (currentSecond < 5)
        {
            currentSecond = InGameManager.Instance.currentSecond;
            stageTimer = currentSecond % 60;
        }

        if (stageTimer == 25)
        {
            if (!isWaveCheck)
            {
                isWaveCheck = true;
                CheckWaveSpawnTime();
            }
        }
        else if (stageTimer == 55)
        {
            if (!isWaveCheck)
            {
                isWaveCheck = true;

                if (currentSecond == 4)
                    CheckBossSpawnTime();
                else
                    CheckWaveSpawnTime();
            }
        }
        else
        {
            isWaveCheck = false;
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
