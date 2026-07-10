using UnityEngine;
using System.Collections.Generic;

public class AndroidDebugger : MonoBehaviour
{
    private readonly Queue<string> _logs = new Queue<string>();
    private const int MaxLines = 15;
    private Vector2 _scrollPosition;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Application.logMessageReceived += HandleLog;
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string condition, string stackTrace, LogType type)
    {
        // 일반 Log는 제외하고 Warning 이상만 수집한다
        if (type == LogType.Log)
        {
            return;
        }

        _logs.Enqueue($"[{type}] {condition}\n{stackTrace}");
        while (_logs.Count > MaxLines)
        {
            _logs.Dequeue();
        }
    }

    private void OnGUI()
    {
        if (_logs.Count == 0)
        {
            return;
        }

        GUI.skin.label.fontSize = 24;
        GUILayout.BeginArea(new Rect(10, 10, Screen.width - 20, Screen.height * 0.5f), GUI.skin.box);
        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

        foreach (string log in _logs)
        {
            GUILayout.Label(log);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }
}
