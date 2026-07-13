using System;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using CustomBuild;

public class PlatformBuild
{
    public static void Build(string platform)
    {
        IPlatformBuilder builder = ParseTarget(platform) switch
        {
            BuildTarget.Android => new AndroidBuilder(),
            _ => throw new NotSupportedException()
        };

        builder.Build();
    }

    private static BuildTarget ParseTarget(string platform)
    {
        switch (platform)
        {
            case "Android":
                return BuildTarget.Android;

            case "IOS":
                return BuildTarget.iOS;

            case "Windows":
                return BuildTarget.StandaloneWindows64;

            case "WebGL":
                return BuildTarget.WebGL;

            default:
                throw new Exception($"Unknown Build Target : {platform}");
        }
    }
}
