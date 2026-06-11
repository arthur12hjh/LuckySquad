using Cinemachine;
using System;
using UnityEngine;

// InGameManager
// 인게임의 로직, Additive Scene과의 통신을 위한 데이터를 담는 싱글톤 매니저
// DontDestroyOnLoad가 아닌, 인게임 진입시에만 설정되는 싱글톤 매니저

// Player Initialize 로직
// 1) 씬 진입 시, InGameManager::Awake에서 Player Data를 받아온다.
// 2) InGameManager::Start에서 PlayerData를 기반으로 class _playerStats = new PlayerStats()로 생성하고 데이터를 입력해.
// 3) InGameManager::Start에서 PlayerData를 기반으로 Player Prefab과 PlayerController Prefab을 Instantiate를 해.
// 4) InGameManager::Start에서 Player GameObject를 PlayerController에 등록해.
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    private bool isGamePaused = false;
    private float gameTime = 0;

    public int currentSecond { get; private set; } = 0;
    private int previousSecond = 0;

    private bool isTimeEnd = false;

    [Header("Debugger")]
    [SerializeField] private PlayerStatsRef _tempStatsRef; // DataManager 연동 시스템 사용 시 더이상 사용하지 않음
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _playerControllerPrefab;
    [SerializeField] private Vector2 _playerSpawnPos = new Vector2(0.6f, 0.3f);

    [Header("Player Data")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _playerControllerObj;
    [SerializeField] private CinemachineVirtualCamera _playerCamera;

    public event Action<int> OnTimeChange; // 게임 시간 변화 이벤트 1초마다 호출

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 1) 씬 진입 시, InGameManager::Awake에서 Player Data를 받아온다.
        // 당장은 일단 비활성화 시켜두고, 추후 병합 및 사용자 데이터 처리 구조 완성 시 구현
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
    }

    private void Update()
    {
        if (!isTimeEnd)
            UpdateGameTime();
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

        // 2D 추적용 Body: Framing Transposer
        CinemachineFramingTransposer transposer =
            _playerCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 10f;
    }

    public PlayerStats GetPlayerStats() => _playerStats;
    public Transform GetPlayerTransform() => _playerObj.transform;

    public void StopGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            Debug.Log("Game Paused");
        }
    }

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

        if (gameTime >= 300)
        {
            gameTime = 300f;
            isTimeEnd = true;
        }

        currentSecond = Mathf.FloorToInt(gameTime);

        if (currentSecond != previousSecond)
        {
            OnTimeChange?.Invoke(currentSecond);
            previousSecond = currentSecond;
        }
    }
}
