using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private Sprite[] tiles;

    private int size = 9;
    private Queue<GameObject> tilePool = new();
    private List<GameObject> activeTiles = new();

    void Awake()
    {
        InitPool();
    }

    void OnEnable()
    {
        Generate();
    }

    // 청프 프리팹 오브젝트 풀을 하여서 미리 생성을 해 놓는다.
    // 우선 사이즈르 12로 하는데 혹시 모를 상황을 대비해서 더 생성 해놓는다.
    void InitPool()
    {
        int poolSize = size * size;

        for (int i = 0; i < poolSize; ++i)
        {
            {
                // 프리팹 만드는 함수
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.SetActive(false);
                tilePool.Enqueue(tile);
            }
        }
    }

    // 12*12로 만듬
    public void Generate()
    {
        ClearTiles();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                // 타일 1개를 먼저 가지고 온다.
                GameObject tile = GetTile();

                // 위치를 초기화 해주고 Active를 true로 해서 보여준다.
                tile.transform.localPosition = new Vector3(x, y, 0);
                tile.SetActive(true);

                // 그리고 Sprite에 있는 값 중 랜덤으로 1개를 꺼내서 만들어 둔다.
                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                sr.sprite = tiles[Random.Range(0, tiles.Length)];

                // 그 후 List에 넣어둔다.
                activeTiles.Add(tile);
            }
        }
    }

    GameObject GetTile()
    {
        // 큐에 값이 있으면 그대로 꺼내온다.
        if (tilePool.Count > 0)
            return tilePool.Dequeue();

        // 그럴 일은 없지만 혹시 없을 수도 있는데 없을 시 생성을 하고 넘겨준다.
        GameObject tile = Instantiate(tilePrefab, transform);
        tile.SetActive(false);
        return tile;
    }

    // 생성 하기 전에 먼저 초기화를 해준다.
    void ClearTiles()
    {
        for (int i = 0; i < activeTiles.Count; i++)
        {
            GameObject tile = activeTiles[i];
            tile.SetActive(false);
            tilePool.Enqueue(tile);
        }

        activeTiles.Clear();
    }
}