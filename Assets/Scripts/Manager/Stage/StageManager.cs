using Item;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private uint currentStageIndex;                       // 현재 스테이지
    private int  currentWaveIndex;                         // 현재 웨이브
    private Dictionary<int, StageRef>        stageDatas;       // 스테이지 데이터 저장용
    private List<Tuple<int, EquipmentBase>>  stageItemDatas;   // 스테이지의 데이터로 아이템 정보 생성
    private List<Tuple<int, int, ItemData>>  ShuffleList;

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
        Instance.Initialize();
    }

    void Initialize()
    {
        stageItemDatas = new List<Tuple<int, EquipmentBase>>();
        List<int> TotalItem = currentStageData.RandomItemDatas;

        var PlayerTransform = InGameManager.Instance.GetPlayerTransform();
        foreach(var item in TotalItem)
            ADD_Item(item, PlayerTransform);

        // 플레이어 무기만 Level 1로 추가
        //ADD_Item(InGameManager.Instance.GetPlayerWeapon(), PlayerTransform);

        // 첫번째 인자에는 배열의 Tuple값
        // 두번쨰 인자에는 원본 배열의 인덱스 값
        ShuffleList = stageItemDatas.Select(
                       (item, index) => Tuple.Create(
                       index,
                       item.Item1,
                       item.Item2.ItemData)).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.End))
        {
            EventBus.Publish(new WeaponSelectEvent(1, 3));
        }

        if (Input.GetKeyUp(KeyCode.Home))
        {
            EventBus.Publish(new WeaponSelectEvent(0, 2));
        }

        if (Input.GetKeyUp(KeyCode.PageDown))
        {
            EventBus.Publish(new WeaponSelectEvent(2, 1));
        }

        if (Input.GetKeyUp(KeyCode.PageUp))
        {
            EventBus.Publish(new WeaponSelectEvent(3, 4));
        }
    }

    private void Start()
    {
        InGameManager.Instance.OnTimeChange += HandleTimeChange;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        EventBus.Subscribe<WeaponSelectEvent>(LevelEvent);
    }

    private void OnDisable()
    {
        currentStageData = null;
        InGameManager.Instance.OnTimeChange -= HandleTimeChange;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventBus.Unsubscribe<WeaponSelectEvent>(LevelEvent);
    }

    // 현재 선택가능한 item을 가져온다.
    // 최대 3개까지 가져온다.
    // Item1 : WeaponSlotIdx;
    // Item2 : Level
    // Item3 : Weapon Data
    public List<Tuple<int, int, ItemData>> Get_RandomItem()
    {
        for (int i = ShuffleList.Count - 1; i > 0; --i)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (ShuffleList[i], ShuffleList[j]) = (ShuffleList[j], ShuffleList[i]);
        }

        int MaxCnt = Math.Min(ShuffleList.Count, 3);
        return ShuffleList.Take(MaxCnt).ToList();
    }

    private void LevelEvent(WeaponSelectEvent msg)
    {
        if (msg == null) return;
        if (stageItemDatas.Count <= msg.SlotIdx) return;

        var ItemObj = stageItemDatas[msg.SlotIdx].Item2;
        ItemObj.LevelUp();

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
       // currentStageData = AddressablesManager.Instance.GetLabelObject<StageRef>($"Stage{currentStageIndex}", "ref", "Stage1");
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
    
    private void ADD_Item(int ItemID, Transform parent)
    {
        var ItemData = DataManager.Instance.FindItemData(ItemID) as WeaponData;
        if (ItemData != null)
        {
            var Prefab = DataManager.Instance.GetWeaponPrefab(ItemData.WeaponType);
            var ItemObj = ItemFactory.AbstractCreateItem(Prefab, parent, ItemID);

            if(ItemObj == null)
            {
                Debug.Log("Not Find : Prefab");
                return;
            }    

            ItemObj.SetActive(false);
            stageItemDatas.Add(new(0, ItemObj.GetComponent<EquipmentBase>()));
        }
    }

    private void CheckBossSpawnTime()
    {

    }
}
