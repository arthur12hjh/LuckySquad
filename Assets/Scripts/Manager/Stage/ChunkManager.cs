using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    private static ChunkManager instance;
    public static ChunkManager Instance => instance;

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

    public GameObject chunkPrefab;
    private Transform player;

    public int chunkSize = 9;
    public int viewDistance = 1;
    public int poolSize = 20;

    private Dictionary<Vector2Int, GameObject> activeChunks = new();
    private Queue<GameObject> pool = new();

    void Start()
    {
        InitPool();
        GenerateInitialChunks();
        player = InGameManager.Instance.GetPlayerTransform();
    }

    void Update()
    {
        UpdateChunks();
    }

    // 오브젝트 풀
    void InitPool()
    {
        // 청크를 받아서 풀에 저장을 해둔다.
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(chunkPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    GameObject GetChunkFromPool()
    {
        if (pool.Count == 0)
        {
            GameObject obj = Instantiate(chunkPrefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        GameObject chunk = pool.Dequeue();
        chunk.SetActive(true);
        return chunk;
    }

    void ReturnChunkToPool(GameObject chunk)
    {
        chunk.SetActive(false);
        pool.Enqueue(chunk);
    }

    // 처음에 맵을 생성 해준다.
    void GenerateInitialChunks()
    {
        // 센터를 정해준 후에
        Vector2Int center = GetCenterCoord();
        // 그 센터의 위치에 생성을 해준다.
        CreateAround(center);
    }

    void UpdateChunks()
    {
        Vector2Int center = GetCenterCoord();

        CreateAround(center);

        List<Vector2Int> toRemove = new();

        foreach (var kv in activeChunks)
        {
            Vector2Int coord = kv.Key;

            if (Mathf.Abs(coord.x - center.x) > viewDistance ||
                Mathf.Abs(coord.y - center.y) > viewDistance)
            {
                ReturnChunkToPool(kv.Value);
                toRemove.Add(coord);
            }
        }

        foreach (var coord in toRemove)
        {
            activeChunks.Remove(coord);
        }
    }

    // 정크가 있는지 없는지 확인을 한다.
    void CreateAround(Vector2Int center)
    {
        for (int x = -viewDistance; x <= viewDistance; x++)
        {
            for (int y = -viewDistance; y <= viewDistance; y++)
            {
                Vector2Int coord = new Vector2Int(center.x + x, center.y + y);

                // 만약 없으면 생성을 한다.
                if (!activeChunks.ContainsKey(coord))
                {
                    GameObject chunk = GetChunkFromPool();
                    Vector3 worldPos = new Vector3(coord.x * chunkSize, coord.y * chunkSize, 0);
                    chunk.transform.position = worldPos;
                    activeChunks.Add(coord, chunk);
                }
            }
        }
    }

    // 청크의 위치를 정해주는데
    // 플레이어가 있으면 플레이어의 기준으로 맵을 이동 시키고
    // 플레이어가 없으면 그냥 생성된 곳 그대로 있음
    Vector2Int GetCenterCoord()
    {
        Vector3 pos = player != null ? player.position : Vector3.zero;

        return new Vector2Int(
            Mathf.FloorToInt(pos.x / chunkSize),
            Mathf.FloorToInt(pos.y / chunkSize)
        );
    }
}