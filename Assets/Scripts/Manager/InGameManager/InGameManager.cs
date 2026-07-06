using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// InGameManager
// ?¸ê²Œ?„ì˜ ë¡œì§, Additive Sceneê³¼ì˜ ?µì‹ ???„í•œ ?°ì´?°ë? ?´ëŠ” ?±ê???ë§¤ë‹ˆ?€
// DontDestroyOnLoadê°€ ?„ë‹Œ, ?¸ê²Œ??ì§„ì…?œì—ë§??¤ì •?˜ëŠ” ?±ê???ë§¤ë‹ˆ?€

// Player Initialize ë¡œì§
// 1) ??ì§„ì… ?? InGameManager::Awake?ì„œ Player Dataë¥?ë°›ì•„?¨ë‹¤.
// 2) InGameManager::Start?ì„œ PlayerDataë¥?ê¸°ë°˜?¼ë¡œ class _playerStats = new PlayerStats()ë¡??ì„±?˜ê³  ?°ì´?°ë? ?…ë ¥??
// 3) InGameManager::Start?ì„œ PlayerDataë¥?ê¸°ë°˜?¼ë¡œ Player Prefabê³?PlayerController Prefab??Instantiateë¥???
// 4) InGameManager::Start?ì„œ Player GameObjectë¥?PlayerController???±ë¡??
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    public event Action OnGameStart;

    private bool isGamePaused = false;
    private float gameTime = 0f;
    private int currentSecond = 0;
    private int previousSecond = 0;

    public uint monsterCount { get; private set; } = 0;
    private bool isTimeEnd = false;

    [Header("Debugger")]
    [SerializeField] private PlayerStatsRef _tempStatsRef; // DataManager ?°ë™ ?œìŠ¤???¬ìš© ???”ì´???¬ìš©?˜ì? ?ŠìŒ
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _playerControllerPrefab;
    [SerializeField] private Vector2 _playerSpawnPos = new Vector2(0.6f, 0.3f);
    [SerializeField] private WaveData _waveData;

    [Header("Player Data")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _playerControllerObj;
    [SerializeField] private CinemachineVirtualCamera _playerCamera;

    public event Action<int> OnTimeChange; // ê²Œì„ ?œê°„ ë³€???´ë²¤??1ì´ˆë§ˆ???¸ì¶œ
    [SerializeField] private Vector3    _SpawnBound;

    SpawnPattern _spawnPattern = null;
    public   SpawnPattern OutScreenSpawnPattern => _spawnPattern;

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 1) ??ì§„ì… ?? InGameManager::Awake?ì„œ Player Dataë¥?ë°›ì•„?¨ë‹¤.
        // ?¹ì¥?€ ?¼ë‹¨ ë¹„í™œ?±í™” ?œì¼œ?ê³ , ì¶”í›„ ë³‘í•© ë°??¬ìš©???°ì´??ì²˜ë¦¬ êµ¬ì¡° ?„ì„± ??êµ¬í˜„
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
            _playerControllerObj.GetComponent<PlayerController>().Initialize(_playerStats, _playerObj);
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
    }
    
    private void Update()
    {
        UpdateGameTime();
        if(Input.GetKeyDown(KeyCode.P)) 
        {
            EndStage();
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

    // ?¼ì‹œ?•ì? ê¸°ëŠ¥
    public void StopGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            Debug.Log("Game Paused");
        }
    }

    // ?¼ì‹œ?•ì? ?´ì œ ê¸°ëŠ¥
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
        gameTime += Time.deltaTime;

        currentSecond = Mathf.FloorToInt(gameTime);

        if (currentSecond != previousSecond)
        {
            OnTimeChange?.Invoke(currentSecond);
            previousSecond = currentSecond;
        }
    }

    // ?¤ì‹œ ë¡œë¹„ë¡??´ë™
    public void EndStage()
    {
        AddressablesManager.Instance.ReleaseStage(($"Stage{GameManager.Instance.currentStage}"));
        GameManager.Instance.SetStage(0);
        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);
        SceneManager.LoadScene("Loading");
    }
}
