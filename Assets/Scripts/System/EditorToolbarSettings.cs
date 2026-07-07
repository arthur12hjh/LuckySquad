#if UNITY_EDITOR
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public class EditorToolbarSettings
{
    const float _minTimeScale = 0f;
    const float _maxTimeScale = 5f;
    const float _padding = 10f;
    
    const string _startSceneElementId = "Scene/Play From First Scene";

    [MainToolbarElement("Timescale/Slider", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement TimescaleSlider()
    {
        var content = new MainToolbarContent("Time Scale", "Time Scale");
        var slider = new MainToolbarSlider(content, Time.timeScale, _minTimeScale,  _maxTimeScale, OnSliderValueChanged);

        slider.populateContextMenu = (menu) =>
        {
            menu.AppendAction("Reset", _ =>
            {
                Time.timeScale = 1f;
                MainToolbar.Refresh("Timescale/Slider");
            });
        };

        return slider;
    }

    [MainToolbarElement("Timescale/Reset", defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement TimescaleResetButton()
    {
        var icon = EditorGUIUtility.IconContent("Refresh").image as Texture2D;
        var content = new MainToolbarContent(icon, "Reset");
        return new MainToolbarButton(content, () => { Time.timeScale = 1f;MainToolbar.Refresh("Timescale/Slider"); });
    }
    
    
    static void OnSliderValueChanged(float value)
    {
        Time.timeScale = Mathf.Clamp(value, _minTimeScale, _maxTimeScale);
    }
    
    
    [MainToolbarElement("Project/Open Project Settings", defaultDockPosition = MainToolbarDockPosition.Right)]
    public static MainToolbarElement ProjectSettingsButton()
    {
        var icon = EditorGUIUtility.IconContent("SettingsIcon").image as Texture2D;
        var content = new MainToolbarContent(icon);
        return new MainToolbarButton(content, () => { SettingsService.OpenProjectSettings(); });
    }

// Middle 존에 배치하고, defaultDockIndex를 낮게 줘서 Play 버튼 그룹보다 왼쪽에 오게 한다
[MainToolbarElement(_startSceneElementId, defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 0)]
public static MainToolbarElement PlayFromFirstSceneButton()
{
    var icon = EditorGUIUtility.IconContent("PlayButton").image as Texture2D;

    string sceneName = GetFirstSceneName();
    var content = new MainToolbarContent("Start '" + sceneName + "'", icon, "Build Settings의 0번 씬부터 PlayMode를 시작합니다");

    var button = new MainToolbarButton(content, OnClickPlayFromFirstScene);

    // PlayMode 중에는 버튼을 비활성화한다 (Step 버튼과 같은 방식)
    button.enabled = EditorApplication.isPlaying == false;

    return button;
}

static string GetFirstSceneName()
{
    if (EditorBuildSettings.scenes.Length == 0)
    {
        return "None";
    }

    string path = EditorBuildSettings.scenes[0].path;
    return System.IO.Path.GetFileNameWithoutExtension(path);
}

static void OnClickPlayFromFirstScene()
{
    if (EditorApplication.isPlaying)
    {
        return;
    }

    if (EditorBuildSettings.scenes.Length == 0)
    {
        Debug.LogWarning("Build Settings에 등록된 씬이 없습니다.");
        return;
    }

    // 수정된 씬 저장 여부를 사용자에게 묻는다. 취소하면 진입하지 않는다
    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo() == false)
    {
        return;
    }

    string firstScenePath = EditorBuildSettings.scenes[0].path;
    SceneAsset firstScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(firstScenePath);

    // playModeStartScene을 지정하면 PlayMode가 이 씬에서 바로 시작된다
    EditorSceneManager.playModeStartScene = firstScene;
    EditorApplication.EnterPlaymode();
}

// 도메인 리로드 시 1회 실행되어 PlayMode 상태 변경 콜백을 등록한다
[InitializeOnLoadMethod]
static void RegisterPlayModeCallback()
{
    EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
}

static void OnPlayModeStateChanged(PlayModeStateChange state)
{
    // PlayMode 종료 후에는 원래 Play 버튼이 정상 동작하도록 반드시 초기화한다
    if (state == PlayModeStateChange.EnteredEditMode)
    {
        EditorSceneManager.playModeStartScene = null;
    }

    // 버튼 라벨과 enabled 상태를 갱신한다
    MainToolbar.Refresh(_startSceneElementId);
}
}

#endif