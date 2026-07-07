using System.Collections;
using Unity.VisualScripting;
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
    
    [SerializeField] private Vector3[] _spawnPoints;

    [SerializeField]
    private WaveData _currentWaveData;
    
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
        
        
        _currentWaveData = waveData;
        
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

    // interval???°ë¼ ëª¬ìŠ¤?°ë? ?Œí™˜?œë‹¤
    // ?„ì¹˜ë¥?
    IEnumerator RunWave(SpawnEvent spawnEvent)
    {
        var interval = new WaitForSeconds(spawnEvent.interval);
        yield return new WaitForSeconds(spawnEvent.startTime);
        
        while (Time.time - _playTime < spawnEvent.endTime)
        {
            switch (spawnEvent.spawnType)
            {
                case SpawnType.Default: // ?¼ë°˜. ?Œë ˆ?´ì–´ ì£¼ìœ„ ?”ë©´ ë°?
                    DefaultSpawn(spawnEvent);
                    break;
                case SpawnType.Boids:   // ?? ?Œë ˆ?´ì–´ ì£¼ìœ„ ?”ë©´ ë°–ì—??ë­‰ì³ ?ì„±
                    BoidsSpawn(spawnEvent);
                    break;
                case SpawnType.Timing:  // ?€?´ë°. ?„ì§ êµ¬í˜„ ê¸°íš ?†ìŒ
                    break;
                case SpawnType.Unexpected:  // ?Œë°œ. ?„ì§ êµ¬í˜„ ê¸°íš ?†ìŒ
                    break;
                case SpawnType.Boss:    // ë³´ìŠ¤. 1?Œë§Œ ?ì„±. ?Œë ˆ?´ì–´ ì£¼ìœ„ ?”ë©´ ë°–ì—???ì„±
                    break;
                case SpawnType.Fixed:   // ê³ ì •?? ?Œë ˆ?´ì–´ ?„ì¹˜ ê¸°ì????„ë‹Œ, ê³ ì • ?„ì¹˜ ê¸°ì? ?ì„±
                    break;
            }
            
            
            yield return interval;
        }
    }

    void DefaultSpawn(SpawnEvent spawnEvent)
    {
        Vector3 centerPos = InGameManager.Instance.GetPlayerTransform().position;
        
        for (int i = 0; i < spawnEvent.monsterCount; ++i)
        {
            SpawnMonster(spawnEvent.poolKey, centerPos + new Vector3(Random.Range(-10f, 10f), Random.Range(-20f, 20f), 0f));
        }
    }
    
    
    void BoidsSpawn(SpawnEvent spawnEvent)
    {
        Vector3 centerPos = InGameManager.Instance.GetPlayerTransform().position;
        centerPos += _spawnPoints[Random.Range(0, _spawnPoints.Length)];
        for (int i = 0; i < spawnEvent.monsterCount; ++i)
        {
            SpawnMonster(spawnEvent.poolKey, centerPos  + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f));
        }
    }

    
    // SpawnMonster ë¶„ê¸° ?˜ëˆ„ê¸?
    void SpawnMonster(ObjectPoolRef objRef, Vector2 pos = default)
    {
        var monster = ObjectPoolManager.Instance.Get(objRef);
        // monster.transform.position = new Vector3(Random.Range(-7.5f, 7.8f), Random.Range(-3.3f, 2.7f), 0f);
        monster.transform.position = pos;
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
