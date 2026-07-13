using Item;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    private uint currentStageIndex;                       // ?Ë¢Å‚ëπ?????†Ïèô????
    private int currentWaveIndex;                         // ?Ë¢Å‚ëπ?????†ÏéåÎµ??
    private Dictionary<int, StageRef> stageDatas;       // ???†Ïèô???? ??®ÏÄ¨Îµ†????????
    private List<Tuple<int, EquipmentBase>> stageItemDatas;   // ???†Ïèô????????®ÏÄ¨Îµ†??®Î∫§Ï®??Ë¢Å‚ëπÎµ???ÔßêÔΩãÍ∂????†ÏéÑ??
    private List<Tuple<int, int, ItemData>> ShuffleList;

    private int prevTimer = 0;
    private int bossIndex = 0;
    private int WaveIndex = 0;

    public event Action<WaveData> OnWave;
    public event Action<GameObject> OnBoss;

    private bool atOnce = true;

    [SerializeField]
    private StageRef currentStageData;                  // ?Ë¢Å‚ëπ?????†Ïèô???? ??®ÏÄ¨Îµ†??

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
  
        var PlayerState = InGameManager.Instance.GetPlayerStats();
        var PlayerTransform = InGameManager.Instance.GetPlayerTransform();

        foreach (var item in TotalItem)
            ADD_Item(item, PlayerTransform);

        if(PlayerState.StartWeapon > 0)
        {
            ADD_Item(PlayerState.StartWeapon, PlayerTransform);
            stageItemDatas.Last().Item2.LevelUp();
        }

        // Á≠åÔΩÉÍ∫ÇËã°?êÏ???ÔßèÍæ©?????†Ïèô???ÑÏèÑ?£Âç†??Tuple??
        // ????†Ïèô????ÔßèÍæ©?????†Ïèô??????†Ïèô???ÑÏèÑ?£Âç†???ÔßèÍªä?????
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
        if (null != InGameManager.Instance)
            InGameManager.Instance.OnTimeChange -= HandleTimeChange;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        EventBus.Unsubscribe<WeaponSelectEvent>(LevelEvent);
    }

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
            OnWave?.Invoke(currentStageData.WaveDatas[currentStageData.WaveIndex]);
            atOnce = false;
        }

        // ËπÇÎåÅ??(5??
        if (currentStageData.StageTime == 150)
        {
            OnBoss?.Invoke(currentStageData.Boss[bossIndex]);
            bossIndex++;
            return;
        }

        if (currentStageData.StageTime == 295)
        {
            OnBoss?.Invoke(currentStageData.Boss[bossIndex]);
            return;
        }

        // ??ÔøΩÏî†??(Ôø???0??
        if (currentStageData.StageTime < 300 && currentStageData.StageTime % 60 == 0)
        {
            WaveIndex++;
            OnWave?.Invoke(currentStageData.WaveDatas[WaveIndex]);
        }
    }

    private void StageDateLoad()
    {
        currentStageData = AddressablesManager.Instance.GetLabelDictionary<StageRef>($"Stage{currentStageIndex}", "Stage1");
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

            if (ItemObj == null)
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
