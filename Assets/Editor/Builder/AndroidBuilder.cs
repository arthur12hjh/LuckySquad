using UnityEditor;
using UnityEditor.Build.Reporting;
using CustomBuild;

public class AndroidBuilder : IPlatformBuilder
{
    public void Build()
    {
        string productName = PlayerSettings.productName;
        string companyName = PlayerSettings.companyName;
        string version = PlayerSettings.bundleVersion;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = EditorBuildSettingsScene.GetActiveSceneList(EditorBuildSettings.scenes),
            locationPathName = $"Build/Android/{PlayerSettings.productName}_{PlayerSettings.bundleVersion}.apk",
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded)
        {
            UnityEngine.Debug.Log($"Build Success : {report.summary.outputPath}");
        }
        else
        {
            throw new System.Exception("Android Build Failed");
        }
    }
}
