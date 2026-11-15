using UnityEditor;
using UnityEngine;
using TMPro;

[InitializeOnLoad]
public static class ImportTMPEssentials
{
    [MenuItem("ReflexLab/Fix Pink Shaders & Import TMP")]
    public static void Fix()
    {
        Debug.Log("Importing TMP Essential Resources...");
        string packagePath = "Packages/com.unity.textmeshpro";
        AssetDatabase.ImportPackage(packagePath + "/Package Resources/TMP Essential Resources.unitypackage", false);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("TMP Essential Resources Imported!");
    }
}
