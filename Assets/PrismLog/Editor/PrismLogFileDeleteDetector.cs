#if PRISM_LOG_CHANEL_GENERATED
using System;
using System.IO;
using PrismLog.Editor;
using UnityEditor;
using UnityEngine;

public class PrismLogFileDeleteDetector : AssetModificationProcessor
{
    private static readonly string PATH = string.Format("Assets/{0}", PrismLogChannelsGenerator.PATH);
    private static readonly string ASSETS_PATH = Application.dataPath.Replace("Assets", "");
    // Called before asset is deleted
    private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
    {
        if (IsPrismLogChannelFile(assetPath))
        {
            Debug.Log($"Detected deletion of PrismLogChannel file: {assetPath}");
            PrismLogChannelsGenerator.RemoveAsGenerated();
        }
        return AssetDeleteResult.DidNotDelete;
    }

    private static void OnWillDeleteAssets(string[] assetPaths, RemoveAssetOptions options)
    {
        foreach (string assetPath in assetPaths)
        {
            if (IsPrismLogChannelFile(assetPath))
            {
                Debug.Log($"Detected deletion of PrismLogChannel file: {assetPath}");
                PrismLogChannelsGenerator.RemoveAsGenerated();
            }
        }
    }

    private static bool IsPrismLogChannelFile(string assetPath)
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