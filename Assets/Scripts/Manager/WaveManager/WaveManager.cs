using System.Collections;
using UnityEngine;

/*
 * ?¨ì´ë¸??œìŠ¤??
 
   ScriptableObject SpawnEvent
   - ?¨ì´ë¸??¨ì¼ ?°ì´?°ë? ë³´ê??œë‹¤.
   - ?Œí‹°???´í™?¸ì? ë¹„ìŠ·???ë‚Œ
   - Get??Pool Object??Keyê°?
   - ëª?ì´ˆë???ëª?ì´ˆê¹Œì§€ë¥??•í•œ??
   - ê·¸ë¦¬ê³?ê·??¬ì´ ì£¼ê¸°ë¥??•í•œ??
   - ??ë²ˆì— ëª?ë§ˆë¦¬ë¥??´ë³´?¼ì? ?•í•œ??
   - enum?¼ë¡œ ?¨ì¼ ë³´ìŠ¤?¨ì´ë¸? ëª¬ìŠ¤???¨ì´ë¸????¤ì •?œë‹¤
   
   ScriptableObject WaveData
   - ???¤í…Œ?´ì???ëª¨ë“  ?¨ì´ë¸??•ë³´ë¥??´ëŠ”??
   - Listë¡?SpawnEventë¥??´ê³  ?ˆëŠ”??
   
   WaveManager
   - WaveDataë¥?ë°›ì•„ ?€?¥í•©?ˆë‹¤.
   - float _playTime??ë§Œë“­?ˆë‹¤. ë§¤í”„?ˆì„ Time.deltaTime???”í•´ì¤ë‹ˆ??
   - StartCoroutine.RunWave(SpawnEvent)?¼ë¡œ List???ˆëŠ” SpawnEvent?¤ì„ ê°ê° ?Œë ¤ì¤ë‹ˆ??
   
   RunWave
   - interval???°ë¼ ë¯¸ë¦¬ WaitForSecondsë¥?ë§Œë“¤?´ì¤?ˆë‹¤.
   - startTime - _playTimeë§Œí¼ ê¸°ë‹¤?¤ì¤?ˆë‹¤.
   - while(endTime > _playTime) ?™ì•ˆ ObjectPool.Get; yield return interval;??ë°˜ë³µ?´ì¤?ˆë‹¤.
 */
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    private enum WaveState { Default, Running, End }
    
    //[SerializeField] private WaveData _waveData;
    [SerializeField] private WaveState _currentWaveState;
    [SerializeField] private float _playTime = 0f;

    private int _activeWaveCount = 0;
    private StageManager _stageManager;

    // Intialize??ë°”ê¿”?¼í•œ??
    // 
    public void Initialize(WaveData waveData)
    {
        _currentWaveState = WaveState.Default;
        //_waveData = waveData;
        _playTime = Time.time;
        _activeWaveCount = 0;
    }

    public void StartWaves(WaveData waveData)
    {
        Debug.Log("Waves Start");
        StopAllCoroutines();
        
        if (waveData is null || waveData.spawnEvents == null || waveData.spawnEvents.Count == 0)
        {
            Debug.LogWarning($"[{nameof(WaveManager)}] WaveDataê°€ ë¹„ì–´ ?ˆì–´ ?œì‘?????†ìŠµ?ˆë‹¤.");
            return;
        }
 
        _playTime = Time.time;
        _currentWaveState = WaveState.Running;
        _activeWaveCount = waveData.spawnEvents.Count;
 
        foreach (var spawnEvent in waveData.spawnEvents)
            StartCoroutine(RunWave(spawnEvent));
    }

    public void EndWaves()
    {
        StopAllCoroutines();
        _activeWaveCount = 0;
        _currentWaveState = WaveState.End;
    }

    void OnEnable()
    {
        SubscribeStageManager();
    }

    void OnDisable()
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.OnWave -= StartWaves;
        }
    }
    
    void Start()
    {
        SubscribeStageManager();
    }

    void Update()
    {
    }

    IEnumerator RunWave(SpawnEvent spawnEvent)
    {
        var interval = new WaitForSeconds(spawnEvent.interval);
        yield return new WaitForSeconds(spawnEvent.startTime);
        
        while (Time.time - _playTime < spawnEvent.endTime - spawnEvent.startTime)
        {
            for(int i=0; i<spawnEvent.monsterCount; ++i)
                SpawnMonster(spawnEvent.poolKey);
            yield return interval;
        }
    }

    void SpawnMonster(ObjectPoolRef objRef)
    {
        var monster = ObjectPoolManager.Instance.Get(objRef);
        monster.transform.position = new Vector3(Random.Range(-7.5f, 7.8f), Random.Range(-3.3f, 2.7f), 0f);
        monster.GetComponent<BaseEntity>().Initialize(objRef.initRef);
    }


    void SubscribeStageManager()
    {
        if (_stageManager != null)
            return;
        
        Debug.Log("OnEnable");
        if (StageManager.Instance != null)
        {
            _stageManager = StageManager.Instance;
            Debug.Log("êµ¬ë…ê³?ì¢‹ì•„???ŒëŒ?¤ì •ê¹Œì?");
            _stageManager.OnWave += StartWaves;
        }
    }
}
