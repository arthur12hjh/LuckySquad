using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance => instance;

    [Header("Bmg Clips")]
    [SerializeField] private Sound[] bmgAudio; // 배경 음악
                                                 
    [Header("Sfx Clips")]
    [SerializeField] private Sound[] playerSfx; // 효과음
    [SerializeField] private Sound[] monsterSfx; // 효과음


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
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
}
