using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }

    [SerializeField] private List<ObjectPoolRef> objectPoolRefs;            // 데이터 리스트.

    private readonly Dictionary<ObjectPoolRef, ObjectPool<GameObject>> _poolDictionary = new(); // 예비객체 담는 맵

    private void Awake()
    {
        Instance = this;
        foreach (var refSO in objectPoolRefs)
        {
            var parent = new GameObject($"{refSO.name}Pool").transform;
            parent.SetParent(transform);
            _poolDictionary[refSO] = CreatePool(refSO, parent);
        }
    }

    private ObjectPool<GameObject> CreatePool(ObjectPoolRef refSO, Transform parent)
    {
        var pool = new ObjectPool<GameObject>(
            createFunc: () => Instantiate(refSO.prefab, parent),            // 생성 방식
            actionOnGet: obj => obj.SetActive(true),              // Get. 인게임 필드로 불러올 때 방식
            actionOnRelease: obj  => obj.SetActive(false),         // Release. 필드에서 이탈할 때 방식
            actionOnDestroy: obj => Destroy(obj),                 // Destroy. 아예 삭제할 때 방식
            collectionCheck: false,                                         // Release할 때 풀에 들어가있는 오브젝트인지 체크.
            defaultCapacity: refSO.initializePoolSize,                      // 처음 생성할 객체양
            maxSize: refSO.initializePoolSize * 2                           // 최대 상한선
            );
        Prewarm(pool, refSO.initializePoolSize);

        return pool;
    }

    // 사전 생성. defaultCapacity만큼 미리 만들어두기
    private void Prewarm(ObjectPool<GameObject> pool, int iSize)
    {
        var tempObjectList = new GameObject[iSize];
        for (int i = 0; i < iSize; ++i)
            tempObjectList[i] = pool.Get();
        for (int i = 0; i < iSize; ++i)
            pool.Release(tempObjectList[i]);
    }

    public GameObject Get(ObjectPoolRef refSO) => _poolDictionary[refSO].Get();
    public void Release(ObjectPoolRef refSO, GameObject obj) => _poolDictionary[refSO].Release(obj);

}