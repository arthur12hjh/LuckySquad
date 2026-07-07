using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/*
 * 웨이브 시스템
 
   ScriptableObject SpawnEvent
   - 웨이브 단일 데이터를 보관한다.
   - 파티클 이펙트와 비슷한 느낌
   - Get할 Pool Object의 Key값
   - 몇 초부터 몇 초까지를 정한다.
   - 그리고 그 사이 주기를 정한다.
   - 한 번에 몇 마리를 내보낼지 정한다
   - enum으로 단일 보스웨이브, 몬스터 웨이브 등 설정한다
   
   ScriptableObject WaveData
   - 한 스테이지의 모든 웨이브 정보를 담는다
   - List로 SpawnEvent를 담고 있는다
   
   WaveManager
   - WaveData를 받아 저장합니다.
   - float _playTime을 만듭니다. 매프레임 Time.deltaTime을 더해줍니다.
   - StartCoroutine.RunWave(SpawnEvent)으로 List에 있는 SpawnEvent들을 각각 돌려줍니다.
   
   RunWave
   - interval에 따라 미리 WaitForSeconds를 만들어줍니다.
   - startTime - _playTime만큼 기다려줍니다.
   - while(endTime > _playTime) 동안 ObjectPool.Get; yield return interval;을 반복해줍니다.
 */
public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    private enum WaveState { Default, Running, End }
    
    //[SerializeField] private WaveData _waveData;
    [SerializeField] private WaveState _currentWaveState;
    [SerializeField] private float _playTime = 0f;
    
    [SerializeField] private Vector3[] _spawnPoints;
    
    private int _activeWaveCount = 0;
    private StageManager _stageManager;

    // Intialize는 바꿔야한다.
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
            Debug.LogWarning($"[{nameof(WaveManager)}] WaveData가 비어 있어 시작할 수 없습니다.");
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

    // interval에 따라 몬스터를 소환한다
    // 위치를 
    IEnumerator RunWave(SpawnEvent spawnEvent)
    {
        var interval = new WaitForSeconds(spawnEvent.interval);
        yield return new WaitForSeconds(spawnEvent.startTime);
        
        while (Time.time - _playTime < spawnEvent.endTime)
        {
            switch (spawnEvent.spawnType)
            {
                case SpawnType.Default: // 일반. 플레이어 주위 화면 밖
                    DefaultSpawn(spawnEvent);
                    break;
                case SpawnType.Boids:   // 떼. 플레이어 주위 화면 밖에서 뭉쳐 생성
                    BoidsSpawn(spawnEvent);
                    break;
                case SpawnType.Timing:  // 타이밍. 아직 구현 기획 없음
                    break;
                case SpawnType.Unexpected:  // 돌발. 아직 구현 기획 없음
                    break;
                case SpawnType.Boss:    // 보스. 1회만 생성. 플레이어 주위 화면 밖에서 생성
                    break;
                case SpawnType.Fixed:   // 고정형. 플레이어 위치 기준이 아닌, 고정 위치 기준 생성
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

    
    // SpawnMonster 분기 나누기
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
            Debug.Log("구독과 좋아요 알람설정까지");
            _stageManager.OnWave += StartWaves;
        }
    }
}
