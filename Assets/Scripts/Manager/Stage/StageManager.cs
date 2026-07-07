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

    private uint currentStageIndex;                      // ?ÑÏû¨ ?§ÌÖå?¥Ï?
    private int currentWaveIndex;                       // ?ÑÏû¨ ?®Ïù¥Î∏?
    private Dictionary<int, StageRef> stageDatas;    // ?§ÌÖå?¥Ï? ?∞Ïù¥???Ä?•Ïö©

    private int prevTimer = 0;
    private int bossIndex = 0;

    public event Action<WaveData> OnWave;
    public event Action<GameObject> OnBoss;

    private bool atOnce = true;

    [SerializeField]
    private StageRef currentStageData;                  // ?ÑÏû¨ ?§ÌÖå?¥Ï? ?∞Ïù¥??

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
        //Debug.Log($"Time :  {currentStageData.StageTime}");

        if (atOnce)
        {
            //Debug.Log("Wave Called");
            // ?®Ïù¥Î∏åÎ? ÎßåÎì§Î©?Í∑??®Ïù¥Î∏åÏóê ?ÑÏöî??Íµ¨Ï°∞Ï≤¥Î? ?òÍ≤®Ï§?
            OnWave?.Invoke(currentStageData.WaveDatas[currentStageData.WaveIndex]);
            atOnce = false;
        }

        // Î≥¥Ïä§ (5Î∂?
        if (currentStageData.StageTime == 150 || currentStageData.StageTime == 300)
        {
            Debug.Log("Boss Cerate");

            OnBoss?.Invoke(currentStageData.Boss[bossIndex]);
            bossIndex++;
            return;
        }

        // ?®Ïù¥Î∏?(Îß?Î∂?0Ï¥?
        if (currentStageData.StageTime < 300 && currentStageData.StageTime % 60 == 0)
        {
            //Debug.Log("Wave Called");
            currentStageData.WaveIndex++;
            // ?®Ïù¥Î∏åÎ? ÎßåÎì§Î©?Í∑??®Ïù¥Î∏åÏóê ?ÑÏöî??Íµ¨Ï°∞Ï≤¥Î? ?òÍ≤®Ï§?
            OnWave?.Invoke(currentStageData.WaveDatas[currentStageData.WaveIndex]);
        }
    }

    private void StageDateLoad()
    {
        string current_Stage = "Stage" + currentStageIndex;
        currentStageData = AddressablesManager.Instance.GetLabelDictionary<StageRef>($"Stage{currentStageIndex}", current_Stage);
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
