using UnityEditor;
using UnityEngine;
using System.IO;
using UnityEngine.AddressableAssets;
using NPOI.SS.Formula.Functions;
using UnityEditor.AddressableAssets.Settings;
using HybridCLR.Editor.Commands;
using System.Linq;

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
        var relativePath = $"Assets/HotFix/{GameManager.platformName}";
        var aotBuildPath = $"{System.Environment.CurrentDirectory}/HybridCLRData/AssembliesPostIl2CppStrip/{GameManager.platformName}";

        if(!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        // build
        HybridCLR.Editor.Commands.CompileDllCommand.CompileDll(buildPath, target, isDebug);
        HybridCLR.Editor.Commands.AOTReferenceGeneratorCommand.GenerateAOTGenericReference(target);

        // copy
        var direction = new DirectoryInfo(buildPath);
        var addressableAssetSettings = AssetDatabase.LoadAssetAtPath<AddressableAssetSettings>("Assets/AddressableAssetsData/AddressableAssetSettings.asset");
        foreach (var file in direction.GetFiles())
        {
            if (file.Name.StartsWith("ECC"))
            {
                file.CopyTo($"{targetPath}/{file.Name}.bytes", true);
                addressableAssetSettings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"{relativePath}/{file.Name}.bytes"), addressableAssetSettings.DefaultGroup); 
            }
        }

        var aotDir = new DirectoryInfo(aotBuildPath);
        foreach (var file in aotDir.GetFiles())
        {
            if (AOTGenericReferences.PatchedAOTAssemblyList.Contains(file.Name))
            {
                file.CopyTo($"{targetPath}/{file.Name}.bytes", true);
                addressableAssetSettings.CreateOrMoveEntry(AssetDatabase.AssetPathToGUID($"{relativePath}/{file.Name}.bytes"), addressableAssetSettings.DefaultGroup); 
            }
        }


        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

    }
}
