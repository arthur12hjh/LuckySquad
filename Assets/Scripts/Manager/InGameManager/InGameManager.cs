using Cinemachine;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

// InGameManager
// �ΰ����� ����, Additive Scene���� ����� ���� �����͸� ��� �̱��� �Ŵ���
// DontDestroyOnLoad�� �ƴ�, �ΰ��� ���Խÿ��� �����Ǵ� �̱��� �Ŵ���

// Player Initialize ����
// 1) �� ���� ��, InGameManager::Awake���� Player Data�� �޾ƿ´�.
// 2) InGameManager::Start���� PlayerData�� ������� class _playerStats = new PlayerStats()�� �����ϰ� �����͸� �Է���.
// 3) InGameManager::Start���� PlayerData�� ������� Player Prefab�� PlayerController Prefab�� Instantiate�� ��.
// 4) InGameManager::Start���� Player GameObject�� PlayerController�� �����.
public class InGameManager : MonoBehaviour
{
    public static InGameManager Instance { get; private set; }

    private bool isGamePaused = false;
    private float gameTime = 0f;
    private int currentSecond = 0;
    private int previousSecond = 0;

    public uint monsterCount { get; private set; } = 0;
    private bool isTimeEnd = false;

    [Header("Debugger")]
    [SerializeField] private PlayerStatsRef _tempStatsRef; // DataManager ���� �ý��� ��� �� ���̻� ������� ����
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _playerControllerPrefab;
    [SerializeField] private Vector2 _playerSpawnPos = new Vector2(0.6f, 0.3f);
    
    [Header("Player Data")]
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _playerObj;
    [SerializeField] private GameObject _playerControllerObj;
    [SerializeField] private CinemachineVirtualCamera _playerCamera;

    [SerializeField] private Vector3    _SpawnBound;

    SpawnPattern _spawnPattern = null;
    public   SpawnPattern OutScreenSpawnPattern => _spawnPattern;
    public event Action<int> OnTimeChange; // ���� �ð� ��ȭ �̺�Ʈ 1�ʸ��� ȣ��

    void Awake()
    {

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // 1) �� ���� ��, InGameManager::Awake���� Player Data�� �޾ƿ´�.
        // ������ �ϴ� ��Ȱ��ȭ ���ѵΰ�, ���� ���� �� ����� ������ ó�� ���� �ϼ� �� ����

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

        _spawnPattern = OutBoundSpawnPattern.Create(_SpawnBound);
    }

    private void Update()
    {
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

        // 2D ������ Body: Framing Transposer
        CinemachineFramingTransposer transposer =
            _playerCamera.AddCinemachineComponent<CinemachineFramingTransposer>();
        transposer.m_CameraDistance = 10f;
    }
    
    public PlayerStats GetPlayerStats() => _playerStats;
    public Transform GetPlayerTransform() => _playerObj.transform;

    // 일시정지 기능
    public void StopGame()
    {
        if (!isGamePaused)
        {
            Time.timeScale = 0;
            isGamePaused = true;
            Debug.Log("Game Paused");
        }
    }

    // 일시정지 해제 기능
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

    // 다시 로비로 이동
    public void EndStage()
    {
        GameManager.Instance.ChangeScene(Enums.SceneType.Lobby);
        SceneManager.LoadScene("Loading");
    }
}
