using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AddressablesManager : MonoBehaviour
{
    private static AddressablesManager instance;
    public static AddressablesManager Instance => instance;

    public event Action OnInitialized;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            StartCoroutine(InitFlow());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public int Progress { get; private set; } = 1;

    // 구조체는 복사가 일어나서 클래스로 만듬
    private class CacheEntry<T>
    {
        // 실제 데이터
        public T value;
        // 메모리 해제를 위해서 필요함
        public AsyncOperationHandle<T> handle;
    }

    private class Cache
    {
        public List<UnityEngine.Object> value;
        public Dictionary<string, UnityEngine.Object> keyValue;
        public AsyncOperationHandle<IList<UnityEngine.Object>> handle;
    }

    // 데이터를 캐싱해서 재사용하기 위함
    // 제네릭으로 데이터를 받고 이름이랑 결과로 저장한다
    // Hash를 이용함
    private Dictionary<Type, Dictionary<string, object>> cache = new();

    // Label로 한번에 받은 데이터를 캐싱해서 재사용하기 위함
    private Dictionary<string, Cache> labelCache = new();
    private Dictionary<string, Cache> commonCache = new();



    // 초기화
    private IEnumerator InitFlow()
    {
        yield return Addressables.InitializeAsync();
        OnInitialized?.Invoke();
    }

    // Addressables 에셋을 로드하는 함수
    // Key값으로 찾음
    public AsyncOperationHandle<T> LoadRoutine<T>(string key)
    {
        // 1. 캐시 체크
        // 만약 이미 로드가 된거라면 그냥 그거 찾아서  불러옴
        if (TryGetCache<T>(key, out T cached))
            return Addressables.ResourceManager.CreateCompletedOperation<T>(cached, null);

        // 2. 로딩 시작
        // 지금부터 비동기 시작
        var handle = Addressables.LoadAssetAsync<T>(key);

        // 3. 완료 후 캐싱

        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                SetCache<T>(key, h.Result, h);
            }
        };

        return handle;
    }

    // 캐시에 저장하는 함수
    private void SetCache<T>(string key, T value, AsyncOperationHandle<T> handle)
    {
        Type type = typeof(T);

        // 타입을 딕셔너리에 찾아서 없으면 추가
        if (!cache.ContainsKey(type))
            cache[type] = new Dictionary<string, object>();


        cache[type][key] = new CacheEntry<T>
        {
            value = value,
            handle = handle,
        };
    }

    public bool TryGetCache<T>(string key, out T value)
    {
        value = default;

        // 제네릭으로 받은 타입을 정의 해준다.
        Type type = typeof(T);

        // 만약 Dictionary가 없으면 함수를 종료한다.
        if (!cache.TryGetValue(type, out var dict))
            return false;

        // 만약 해당 타입의 캐시가 없으면 종료한다.
        if (!dict.TryGetValue(key, out var obj))
            return false;

        // 타입이 맞으면 반환
        if (obj is CacheEntry<T> entry)
        {
            value = entry.value;
            return true;
        }

        return false;
    }

    // 키값으로 해제
    public void Release<T>(string key)
    {
        // 제네릭 타입을 반환 해준다.
        Type type = typeof(T);

        // 실패시 종료
        if (!cache.TryGetValue(type, out var dict))
            return;

        if (!dict.TryGetValue(key, out var obj))
            return;

        // 제거
        if (obj is CacheEntry<T> entry)
        {
            Addressables.Release(entry.handle);
            dict.Remove(key);
        }

        if (dict.Count == 0)
            cache.Remove(type);
    }

    //label
    public AsyncOperationHandle<IList<UnityEngine.Object>> LoadLabel(string label)
    {
        // 1. 캐시 체크
        // 만약 이미 로드가 된거라면 Addressables를 호출 안함
        if (TryGetLabel(label, out List<UnityEngine.Object> cached))
            return Addressables.ResourceManager.CreateCompletedOperation<IList<UnityEngine.Object>>(cached, null);

        // 두 라벨 모두 포함된 Asset 로드
        var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null, Addressables.MergeMode.Intersection);

        // 캐시 저장
        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                var list = new List<UnityEngine.Object>(h.Result);
                var dictionary = new Dictionary<string, UnityEngine.Object>();

                foreach (var obj in list)
                {
                    dictionary[obj.name] = obj;
                }

                labelCache[label] = new Cache
                {
                    value = list,
                    keyValue = dictionary,
                    handle = h
                };
            }
        };

        return handle;
    }

    private bool TryGetLabel(string key, out List<UnityEngine.Object> value)
    {
        value = null;

        // 존재하는지 확인 한다.
        if (!labelCache.TryGetValue(key, out var obj))
            return false;

        // 데이터 타입이 맞는지 확인 후 실제 데이터를 반환한다.
        value = obj.value;
        return true;
    }

    private bool TryGetLabelmap(string key, out Dictionary<string, UnityEngine.Object> map)
    {
        map = null;

        if (!labelCache.TryGetValue(key, out var obj))
            return false;

        map = obj.keyValue;
        return true;
    }

    // 캐시에 저장된 에셋 중 이름이 일치하는 에셋을 반환한다.
    // 사용 방법, label : Lobby, Stage 등 labelType : obj, img 등, assetName : 실제 객체이름
    // test1 = AddressablesManager.Instance.GetLabelObject<GameObject>("Logo","obj","MonsterTest");

    public T GetLabelDictionary<T>(string label, string assetName)
            where T : UnityEngine.Object
    {
        // 해당 라벨이 아직 로드되지 않았다면 null 반환
        if (!TryGetLabelmap(label, out var map))
            return null;

        if (map.TryGetValue(assetName, out var cache))
            return cache as T;

        return null;
    }

    public List<T> GetLabelList<T>(string label)
        where T : UnityEngine.Object
    {
        if (!TryGetLabel(label, out var list))
            return null;

        return list.OfType<T>().ToList();
    }

    // Comm
    public AsyncOperationHandle<IList<UnityEngine.Object>> LoadCommon()
    {
        // 1. 캐시 체크
        // 만약 이미 로드가 된거라면 Addressables를 호출 안함
        if (TryGetComponentList("Common", out List<UnityEngine.Object> cached))
            return Addressables.ResourceManager.CreateCompletedOperation<IList<UnityEngine.Object>>(cached, null);

        // 라벨 Asset 로드
        var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>("Common", null, Addressables.MergeMode.Intersection);

        // 캐시 저장
        handle.Completed += h =>
        {
            if (h.Status == AsyncOperationStatus.Succeeded)
            {
                var list = new List<UnityEngine.Object>(h.Result);
                var dictionary = new Dictionary<string, UnityEngine.Object>();

                foreach (var obj in list)
                {
                    dictionary[obj.name] = obj;
                }

                commonCache["Common"] = new Cache
                {
                    value = list,
                    keyValue = dictionary,
                    handle = h
                };
            }
        };

        return handle;
    }

    private bool TryGetComponentList(string key, out List<UnityEngine.Object> List)
    {
        List = null;

        if (!commonCache.TryGetValue(key, out var cache))
            return false;

        List = cache.value;
        return true;
    }

    public T GetCommon<T>(string name)
    where T : UnityEngine.Object
    {
        if (!TryGetCommonMap("Common", out var map))
            return null;

        if (map.TryGetValue(name, out var cache))
            return cache as T;

        return null;
    }

    private bool TryGetCommonMap(string key, out Dictionary<string, UnityEngine.Object> map)
    {
        map = null;

        // 존재하는지 확인 한다.
        if (!commonCache.TryGetValue(key, out var cache))
            return false;

        map = cache.keyValue;
        return true;
    }

    public void ReleaseCommon()
    {
        if (!commonCache.TryGetValue("Common", out var obj))
            return;

        if (obj is Cache cache)
        {
            Addressables.Release(cache.handle);
            commonCache.Remove("Common");
        }
    }

    // 데이터 해제
    public void ReleaseLabel(string label)
    {
        // 실패시 반환
        if (!labelCache.TryGetValue(label, out var obj))
            return;

        // Addressables 메모리 해제
        // 캐시 제거
        if (obj is Cache cache)
        {
            Addressables.Release(cache.handle);
            commonCache.Remove("cache");
        }
    }

}