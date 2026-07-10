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

    void InitPool()
    {
        int poolSize = size * size;

        for (int i = 0; i < poolSize; ++i)
        {
            {
                GameObject tile = Instantiate(tilePrefab, transform);
                tile.SetActive(false);
                tilePool.Enqueue(tile);
            }
        }
    }

    private int GetRandomTileByWeight()
    {
        int random = Random.Range(0, 100);

        if (random < 40)
            return 0;

        if (random < 70)
            return 1;

        if (random < 85)
            return 2;

        if (random < 90)
            return 3;

        if (random < 95)
            return 4;

        if (random < 97)
            return 5;

        if (random < 98)
            return 6;

        if (random < 99)
            return 7;

        return 8;
    }

    public void Generate()
    {
        ClearTiles();

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                GameObject tile = GetTile();

                tile.transform.localPosition = new Vector3(x, y, 0);
                tile.SetActive(true);

                SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
                sr.sprite = tiles[GetRandomTileByWeight()];

                activeTiles.Add(tile);
            }
        }
    }

    GameObject GetTile()
    {
        if (tilePool.Count > 0)
            return tilePool.Dequeue();

        GameObject tile = Instantiate(tilePrefab, transform);
        tile.SetActive(false);
        return tile;
    }

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