using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class SetAssetBundleName : Editor
{
    public static string sourcePath = Application.dataPath + "/Resources_";

    [@MenuItem("EditTools/SetAssetBundleName（设置AssetBundleName）")]
    static public void start()
    {
        ClearAssetBundlesName();

        SetScenceName();
        SetAssetBundleNameByDirPath(sourcePath);

        //UnityEngine.Object[] objlist = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        //Debug.LogError(objlist.Length);
        //foreach (Object obj in objlist)
        //{
        //    string path = AssetDatabase.GetAssetPath(obj);
        //    SetAssetBundleNameByPath(path);
        //}

        AssetDatabase.Refresh();
    }

    //设置某个具体的Object的AssetBundleName
    static void SetAssetBundleNameByPath(string path)
    {
        string[] dep = AssetDatabase.GetDependencies(path);
        for (int i = 0; i < dep.Length; i++)
        {
            if (dep[i].EndsWith(".cs") || dep[i].EndsWith(".js"))
                continue;

            AssetImporter depobj = AssetImporter.GetAtPath(dep[i]);
            string bundleName = dep[i].Replace(Path.GetExtension(dep[i]), ".unity3d");
            depobj.assetBundleName = bundleName;
        }
    }

    //对某个路径(文件夹)设置AssetBundleName
    static void SetAssetBundleNameByDirPath(string source)
    {
        DirectoryInfo folder = new DirectoryInfo(source);
        FileSystemInfo[] files = folder.GetFileSystemInfos();
        int length = files.Length;

        for (int i = 0; i < length; i++)
        {
            if (files[i] is DirectoryInfo)
            {
                SetAssetBundleNameByDirPath(files[i].FullName);
            }
            else
            {
                if (!files[i].Name.EndsWith(".meta"))
                {
                    string path = GetRelativePath(files[i].FullName);
                    SetAssetBundleNameByPath( path );
                }
            }
        }
    }

    //获取相对路径
    static string GetRelativePath(string fullPath)
    {
        //Debug.LogError(fullPath);
        //fullPath.Replace('\\', '/');
        //Debug.LogError(fullPath);
        string _assetPath = "Assets" + fullPath.Substring(Application.dataPath.Length);

        return _assetPath;
    }

    /// <summary>
    /// 清除之前设置过的AssetBundleName，避免产生不必要的资源也打包
    /// 之前说过，只要设置了AssetBundleName的，都会进行打包，不论在什么目录下
    /// </summary>
    static void ClearAssetBundlesName()
    {
        int length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
        string[] oldAssetBundleNames = new string[length];
        for (int i = 0; i < length; i++)
        {
            oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
        }

        for (int j = 0; j < oldAssetBundleNames.Length; j++)
        {
            AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
        }
        length = AssetDatabase.GetAllAssetBundleNames().Length;
        Debug.Log(length);
    }

    /// <summary>
    /// 更加场景打包配置，对场景进行处理，第一个场景不做处理
    /// </summary>
    static void SetScenceName()
    {
        string[] levels = GetLevelsFromBuildSettings();
        if (levels.Length < 2)
        {
            Debug.LogError("scenece length < 2. length = " + levels.Length);
            return;
        }

        for (int i = 1; i < levels.Length; i++)
        {
            SetAssetBundleNameByPath(levels[i]);
        }
    }

    //获取需要打包的场景名称
    static string[] GetLevelsFromBuildSettings()
    {
        List<string> levels = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; ++i)
        {
            if (EditorBuildSettings.scenes[i].enabled)
                levels.Add(EditorBuildSettings.scenes[i].path);
        }

        return levels.ToArray();
    }
}
