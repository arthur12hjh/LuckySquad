using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;

    public static GameManager Instance
    {
        get { return instance; }
    }

    public uint currentStage { get; private set; } = 0;
    [SerializeField] public PlayerStatsRef _playerStatsRef;

    public Enums.SceneType currentSceneType { get; private set; }
    public event Action<Enums.SceneType> OnSceneChanged;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
            currentSceneType = Enums.SceneType.Loding;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ChangeScene(Enums.SceneType type)
    {
        currentSceneType = type;

        OnSceneChanged?.Invoke(type);
    }

    public void SetStage(uint stage)
    {
        currentStage = stage;
    }
}
