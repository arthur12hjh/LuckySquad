using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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

    // 구조체는 복사가 일어나서 클래스로 만듬
    private class CacheEntry<T>
    {
        // 실제 데이터
        public T value;
        // 메모리 해제를 위해서 필요함
        public AsyncOperationHandle<T> handle;
        // 참조 카운트
        public int refCount;
    }

    private class LabelCache
    {
        public object value;
        public AsyncOperationHandle handle;
    }

    // 데이터를 캐싱해서 재사용하기 위함
    // 제네릭으로 데이터를 받고 이름이랑 결과로 저장한다
    // Hash를 이용함
    private Dictionary<Type, Dictionary<string, object>> cache
        = new Dictionary<Type, Dictionary<string, object>>();

    // Label로 한번에 받은 데이터를 캐싱해서 재사용하기 위함
    private Dictionary<string, LabelCache> labelCache
        = new Dictionary<string, LabelCache>();

    // 초기화
    private IEnumerator InitFlow()
    {
        yield return Addressables.InitializeAsync();
        OnInitialized?.Invoke();
    }


    // Addressables 에셋을 로드하는 함수
    // callback이 null이면 "캐싱만" 수행하고 결과는 외부로 전달하지 않음
    public void Load<T>(string key, Action<T> callback = null)
    {
        StartCoroutine(LoadRoutine(key, callback));
    }

    private IEnumerator LoadRoutine<T>(string key, Action<T> callback)
    {
        // 만약 값이 있으면 로드를 안하고 즉시 반환을 해준다.
        if (TryGetCache(key, out T cached))
        {
            callback?.Invoke(cached);
            yield break;
        }

        // Addressables를 이용하여서 비동기 로딩을 한다.
        var handle = Addressables.LoadAssetAsync<T>(key);
        yield return handle;

        // 실패시 코루틴을 종료한다.
        if (handle.Status != AsyncOperationStatus.Succeeded)
            yield break;

        // 값을 캐싱한다.
        SetCache(key, handle);

        callback?.Invoke(handle.Result);
    }



    private void SetCache<T>(string key, AsyncOperationHandle<T> handle)
    {
        // 제네릭으로 받은 타입을 정의 해준다.
        Type type = typeof(T);

        // 만약 타입 Dictionary가 없으면 생성을 해준다.
        if (!cache.ContainsKey(type))
            cache[type] = new Dictionary<string, object>();

        // 저장을 해준다.
        cache[type][key] = new CacheEntry<T>
        {
            value = handle.Result,
            handle = handle,
            refCount = 1
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
            entry.refCount++;
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
            entry.refCount--;

            if (entry.refCount > 0)
                return;

            Addressables.Release(entry.handle);
            dict.Remove(key);
        }

        if (dict.Count == 0)
            cache.Remove(type);
    }



    // 모든 객체를 다 지운다.
    public void ReleaseAll()
    {
        foreach (var typeDict in cache.Values)
        {
            foreach (var obj in typeDict.Values)
            {
                if (obj is CacheEntry<object> entry)
                {
                    Addressables.Release(entry.handle);
                }
            }
        }

        cache.Clear();
    }



    // Addressable에 있는 label이 같은 모든 객체를 한번에 불러온다.
    public IEnumerator LoadLabel<T>(string label, Action<List<T>> callback)
    {
        // 이미 생성되어 있으면 바로 값을 넘겨준다.
        if (labelCache.TryGetValue(label, out var cached))
        {
            callback?.Invoke((List<T>)cached.value);
            yield break;
        }

        // Addressable에 있는 라벨을 비동기 함수로 다 가지고 온다.
        var handle = Addressables.LoadAssetsAsync<T>(label, null);
        yield return handle;

        // 실패시 종료한다.
        if (handle.Status != AsyncOperationStatus.Succeeded)
            yield break;

        // 캐싱을 해준다
        var list = new List<T>(handle.Result);

        labelCache[label] = new LabelCache
        {
            value = list,
            handle = handle
        };

        callback?.Invoke(list);
    }



    // 데이터 해제
    public void ReleaseLabel(string label)
    {
        // 실패시 반환
        if (!labelCache.TryGetValue(label, out var cache))
            return;

        // Addressables 메모리 해제
        Addressables.Release(cache.handle);

        // 캐시 제거
        labelCache.Remove(label);
    }
}