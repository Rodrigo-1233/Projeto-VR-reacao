using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class ReimportRenderers
{
    [MenuItem("ReflexLab/Refresh Renderers")]
    public static void Refresh()
    {
        AssetDatabase.ImportAsset("Assets/_Project/Settings/PC_Renderer.asset", ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset("Assets/_Project/Settings/PC_RPAsset.asset", ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset("Assets/_Project/Settings/Mobile_Renderer.asset", ImportAssetOptions.ForceUpdate);
        AssetDatabase.ImportAsset("Assets/_Project/Settings/Mobile_RPAsset.asset", ImportAssetOptions.ForceUpdate);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Renderers refreshed successfully!");
    }
}
