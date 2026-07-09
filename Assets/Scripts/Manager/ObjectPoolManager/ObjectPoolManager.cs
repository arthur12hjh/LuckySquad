using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance { get; private set; }
    public bool IsReady { get; private set; }

    [SerializeField] private List<ObjectPoolRef> objectPoolRefs;            // 데이터 리스트.

    private readonly Dictionary<ObjectPoolRef, ObjectPool<GameObject>> _poolDictionary = new(); // 풀링으로 만들어진 예비객체들
    private readonly Dictionary<ObjectPoolRef, GameObject> _prefabPool = new(); // 해당 SO의 프리팹
    private readonly Dictionary<ObjectPoolRef, AsyncOperationHandle<GameObject>> _asyncOperationHandles = new(); // 해당 SO의 어드레서블 핸들

    private void Awake()
    {
        if (Instance != null
            && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        // foreach (var refSO in objectPoolRefs)
        // {
        //     var parent = new GameObject($"{refSO.name}Pool").transform;
        //     parent.SetParent(transform);
        //     _poolDictionary[refSO] = CreatePool(refSO, parent);
        // }
    }

    private async void Start()
    {
        await InitializeAsync();
        IsReady = true;
    }

    private async Task InitializeAsync()
    {
        foreach (var refSO in objectPoolRefs)
        {
            // 비동기 작업 핸들. 일 좀 오래걸리니까 비동기로 팔게
            AsyncOperationHandle<GameObject> handle = refSO.prefab.LoadAssetAsync<GameObject>();
            
            // Load Asset 끝날 때까지 기다리기
            await handle.Task;

            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Error : Object Pool Ref {refSO.poolName} cannot be loaded");
                continue; // 라이브 빌드에서는 그래도 일단 실행은 돼야하니가...
            }

            _asyncOperationHandles[refSO] = handle;
            _prefabPool[refSO] = handle.Result;
            
            var parent = new GameObject($"{refSO.name}Pool").transform;
            parent.SetParent(transform);
            _poolDictionary[refSO] = CreatePool(refSO, parent);
        }
    }
    
    
    private ObjectPool<GameObject> CreatePool(ObjectPoolRef refSO, Transform parent)
    {
        ObjectPool<GameObject> pool = null;
        GameObject prefab = _prefabPool[refSO];
        pool = new ObjectPool<GameObject>(
            createFunc: () =>
            { 
                var obj =  Instantiate(prefab, parent);
                if(obj.TryGetComponent<IPoolable>(out IPoolable poolable))
                    poolable.OnSpawn(() => pool.Release(obj));
                return obj;
            },            // 생성 방식
            actionOnGet: obj => obj.SetActive(true),              // Get. 인게임 필드로 불러올 때 방식
            actionOnRelease: obj  => obj.SetActive(false),        // Release. 필드에서 이탈할 때 방식
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

    public bool Clear(ObjectPoolRef refSO)
    {
    //     비워야하는거
    // private readonly Dictionary<ObjectPoolRef, ObjectPool<GameObject>> _poolDictionary = new(); // 풀링으로 만들어진 예비객체들
    // private readonly Dictionary<ObjectPoolRef, GameObject> _prefabPool = new(); // 해당 SO의 프리팹
    // private readonly Dictionary<ObjectPoolRef, AsyncOperationHandle<GameObject>> _asyncOperationHandles = new(); // 해당 SO의 어드레서블 핸들
    
        // 지워줘야할 것 : ObjectPool<GameObject>, ScriptableObject, handle, ScriptableObject.AssetReferenceGameObject
        // 1. 먼저 ObjectPool<GameObject>를 비우자 (예비 객체 모음)
        if (!_poolDictionary.TryGetValue(refSO, out var pool))
            return false;
        
        pool.Clear();
        
        // 2. 이제 ScriptableObject 기반으로 Addressable 해지, handle, 프리팹 모음, ObjectPool 모음에서 해당 값을 빼줘야한다.
        if(_asyncOperationHandles.TryGetValue(refSO, out var handle) && handle.IsValid())
            refSO.prefab.ReleaseAsset();
        
        _poolDictionary.Remove(refSO);
        _asyncOperationHandles.Remove(refSO);
        _prefabPool.Remove(refSO);
        
        return true;
    }

    // Addressable 기반이기에, 해제를 해줘야한다.
    private void OnDestroy()
    {
        // Dispose => Clear인데 C#에서 초기화됐는지 컴파일 타임에 자동 추적해줌.
        foreach(var pool in _poolDictionary.Values)
            pool.Dispose();
        _poolDictionary.Clear();

        foreach (var refSO in _asyncOperationHandles.Keys)
        {
            if (refSO.prefab.IsValid())
                refSO.prefab.ReleaseAsset();
        }
        
        _asyncOperationHandles.Clear();
        _prefabPool.Clear();
        
        if(Instance == this) Instance = null;
    }
    
    
}