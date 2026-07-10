using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// InGameManager
// ?�게?�의 로직, Additive Scene과의 ?�신???�한 ?�이?��? ?�는 ?��???매니?�?
// DontDestroyOnLoad가 ?�닌, ?�게??진입?�에�??�정?�는 ?��???매니?�?

// Player Initialize 로직
// 1) ??진입 ?? InGameManager::Awake?�서 Player Data�?받아?�다.
// 2) InGameManager::Start?�서 PlayerData�?기반?�로 class _playerStats = new PlayerStats()�??�성?�고 ?�이?��? ?�력??
// 3) InGameManager::Start?�서 PlayerData�?기반?�로 Player Prefab�?PlayerController Prefab??Instantiate�???
// 4) InGameManager::Start?�서 Player GameObject�?PlayerController???�록??
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    public event Action OnGameStart;
    public event Action OnGameClear;
    public event Action OnCameraShake;

    private bool isGamePaused = false;
    private float gameTime = 0f;
    private int currentSecond = 0;
    private int previousSecond = 0;

    private uint monsterDeathCount = 0;
    private bool isTimeEnd = false;

    [Header("Debugger")] [SerializeField]
    private PlayerStatsRef _tempStatsRef; // DataManager ?�동 ?�스???�용 ???�이???�용?��? ?�음

    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _playerControllerPrefab;
    [SerializeField] private Vector2 _playerSpawnPos = new Vector2(0.6f, 0.3f);
    [SerializeField] private WaveData _waveData;

    [Header("Player Data")] [SerializeField]
    private PlayerStats _playerStats;

    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _playerControllerObj;
    [SerializeField] private CinemachineVirtualCamera _playerCamera;
    private PlayerController _playerController;


    [Header("Camera Shake")]
    [SerializeField] private float _shakeForce = 1f;      // ?�이???�기
    [SerializeField] private float _shakeDuration = 0.2f; // ?�이??지???�간
    private CinemachineImpulseSource _impulseSource;
    
    public event Action<int> OnTimeChange; // 게임 ?�간 변???�벤??1초마???�출
    [SerializeField] private Vector3 _SpawnBound;

    SpawnPattern _spawnPattern = null;
    public SpawnPattern OutScreenSpawnPattern => _spawnPattern;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 1) ??진입 ?? InGameManager::Awake?�서 Player Data�?받아?�다.
        // ?�장?�??�단 비활?�화 ?�켜?�고, 추후 병합 �??�용???�이??처리 구조 ?�성 ??구현
        if (_tempStatsRef != null)
        {
            _playerStats = new PlayerStats(_tempStatsRef);
        }

        if (_playerObj == null)
        {
            _playerObj = Instantiate(_playerPrefab, _playerSpawnPos, Quaternion.identity);
        }

        if (_playerControllerObj == null)
        {
            _playerControllerObj = Instantiate(_playerControllerPrefab);
            _playerController =  _playerControllerObj.GetComponent<PlayerController>();
            _playerController.Initialize(_playerStats, _playerObj);
            
        }

        if (_playerCamera != null)
        {
            _playerCamera.Follow = _playerObj.transform;
        }
        else
        {
            InstantiatePlayerVirtualCamera();
        }

        // if (WaveManager.Instance != null)
        // {
        //     WaveManager.Instance.Initialize(_waveData);
        // }
        _spawnPattern = OutBoundSpawnPattern.Create(_SpawnBound);
    }

    void Start()
    {
        OnGameStart?.Invoke();
        monsterDeathCount = 0;
    }

    private void Update()
    {
        UpdateGameTime();
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            EndStage();
        }

        if(Input.GetKeyDown(KeyCode.O)) 
        {
            OnGameClear?.Invoke();
        }
    }

    void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void InstantiatePlayerVirtualCamera()
    {
        GameObject camObj = new GameObject("PlayerCamera");
        _playerCamera = camObj.AddComponent<CinemachineVirtualCamera>();

        _playerCamera.Priority = 10;
        _playerCamera.Follow = _playerObj.transform;
        _playerCamera.m_Lens.OrthographicSize = 4.46f;

        CinemachineFramingTransposer transposer =
            _playerCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 10f;
    }

    public PlayerStats GetPlayerStats() => _playerStats;
    public Transform GetPlayerTransform() => _playerObj.transform;

    // ?�시?��? 기능
    public void StopGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            Debug.Log("Game Paused");
        }
    }

    // ?�시?��? ?�제 기능
    public void ResumeGame()
    {
        if (isGamePaused)
        {
            Time.timeScale = 1;
            isGamePaused = false;
            Debug.Log("Game Resumed");
        }
    }

    private void UpdateGameTime()
    {
        if (previousSecond == 300)
            return;

        gameTime += Time.deltaTime;

        currentSecond = Mathf.FloorToInt(gameTime);

        if (currentSecond != previousSecond)
        {
            OnTimeChange?.Invoke(currentSecond);
            previousSecond = currentSecond;
        }
    }

    // ?�시 로비�??�동
    public void EndStage()
    {
        AddressablesManager.Instance.ReleaseLabel($"Stage{GameManager.Instance.currentStage}");
        GameManager.Instance.SetStage(0);
        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);
        SceneManager.LoadScene("Loading");
    }

    public Vector2 GetPlayerDir() => _playerController.GetPlayerDir();
    public void IncrementMonsterDeathCount() { monsterDeathCount++; }
    public uint GetMonsterDeathCount() { return monsterDeathCount; }
}
