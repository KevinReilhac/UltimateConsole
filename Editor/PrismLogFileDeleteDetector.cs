#if PRISM_LOG_CHANEL_GENERATED
using System;
using System.IO;
using PrismLog.Editor;
using UnityEditor;
using UnityEngine;

public class PrismLogFileDeleteDetector : AssetModificationProcessor
{
    private static readonly string PATH = string.Format("Assets/{0}", PrismLogChanelsGenerator.PATH);
    private static readonly string ASSETS_PATH = Application.dataPath.Replace("Assets", "");
    // Called before asset is deleted
    private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
    {
        if (IsPrismLogChanelFile(assetPath))
        {
            Debug.Log($"Detected deletion of PrismLogChanel file: {assetPath}");
            PrismLogChanelsGenerator.RemoveAsGenerated();
        }
        return AssetDeleteResult.DidNotDelete;
    }

    private static void OnWillDeleteAssets(string[] assetPaths, RemoveAssetOptions options)
    {
        foreach (string assetPath in assetPaths)
        {
            if (IsPrismLogChanelFile(assetPath))
            {
                Debug.Log($"Detected deletion of PrismLogChanel file: {assetPath}");
                PrismLogChanelsGenerator.RemoveAsGenerated();
            }
        }
    }

    private static bool IsPrismLogChanelFile(string assetPath)
    {
        if (Directory.Exists(Path.Join(ASSETS_PATH, assetPath)))
        {
            if (PATH.StartsWith(assetPath, StringComparison.Ordinal))
                return true;
        }
        return PATH.Equals(assetPath, StringComparison.Ordinal);
    }
}
#endif