using System;
using System.Collections.Generic;

public static class EventBus
{
    private static readonly Dictionary<Type, List<Delegate>> _events = new();

    public static void Subscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);

        if (!_events.TryGetValue(type, out var list))
        {
            list = new List<Delegate>();
            _events.Add(type, list);
        }

        if (!list.Contains(callback))
            list.Add(callback);
    }

    public static void Unsubscribe<T>(Action<T> callback)
    {
        Type type = typeof(T);

        if (!_events.TryGetValue(type, out var list))
            return;

        list.Remove(callback);

        if (list.Count == 0)
            _events.Remove(type);
    }

    public static void Publish<T>(T msg)
    {
        Type type = typeof(T);

        if (!_events.TryGetValue(type, out var list))
            return;

        // 순회 중 Subscribe/Unsubscribe에 대비하여 복사
        Delegate[] callbacks = list.ToArray();

        foreach (Delegate callback in callbacks)
        {
            ((Action<T>)callback)?.Invoke(msg);
        }
    }

    public static void Clear()
    {
        _events.Clear();
    }
}
