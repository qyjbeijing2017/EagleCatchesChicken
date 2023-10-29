using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.AddressableAssets;
using NPOI.SS.Formula.Functions;

public class ECCEditor : Editor
{

    [MenuItem("ECC/BuildHotFix/Build")]
    public static void BuildHotFix()
    {
        // paths
        var target = EditorUserBuildSettings.activeBuildTarget;
        var isDebug = Debug.isDebugBuild;
        var debugDir = isDebug ? "Debug" : "Release";
        var buildPath = $"{System.Environment.CurrentDirectory}/HotFixBuild/{GameManager.platformName}/{debugDir}";
        var targetPath = $"{Application.dataPath}/HotFix/{GameManager.platformName}";

        if(!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        // build
        HybridCLR.Editor.Commands.CompileDllCommand.CompileDll(buildPath, target, isDebug);

        // copy
        var direction = new DirectoryInfo(buildPath);
        foreach (var file in direction.GetFiles())
        {
            if (file.Name.StartsWith("ECC"))
            {
                file.CopyTo($"{targetPath}/{file.Name}.bytes", true);
            }
        }
    }
}
