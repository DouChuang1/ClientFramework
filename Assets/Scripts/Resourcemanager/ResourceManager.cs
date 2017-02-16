using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// ResourceManager 解决资源加载的方式问题。资源释放。
/// ResourceManager封装了实例化GameObject接口（重写GameObject.Instantiate，此接口对接对象池）
/// </summary>
public class ResourceManager : MonoBehaviour
{
    public static Dictionary<string, UnityEngine.Object> foreverResources = new Dictionary<string, UnityEngine.Object>();  //预加载的资源文件（不随场景销毁而销毁）
    public static Dictionary<string, UnityEngine.Object> unforeverResources = new Dictionary<string, UnityEngine.Object>();  //预加载的资源文件（随场景销毁而销毁）

    private static Dictionary<string, string> resourcesObject = new Dictionary<string, string>(); //资源Object


    public static void IncreaseAddPool(string path, string pool, int num, System.Type type)
    {
        if (resourcesObject.ContainsKey(path))
            return;

        GameObject obj = ResourceManager.LoadAssetBundleResource(path) as GameObject;
        if (obj == null)
            Debug.LogWarning(path);
        CPoolManager.instance.CreatePrefabPool(pool, obj.transform, num, type);
        resourcesObject[path] = pool;
    }

    //预加载资源
    public static void IncreaseLoadResoures(string path, bool isforerer)
    {
        if (path == null)
            return;

        if (foreverResources.ContainsKey(path))
            return;
        else if (unforeverResources.ContainsKey(path))
            return;

        UnityEngine.Object obj = ResourceManager.LoadAssetBundleResource(path);
        if (isforerer)
            foreverResources.Add(path, obj);
        else
            unforeverResources.Add(path, obj);
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="path"></param>
    /// <param name="isforerer"></param>
    /// <returns></returns>
    public static IEnumerator IncreaseLoadResouresSync(string path, bool isforerer)
    {
        if (path == null)
            yield break;

        if (foreverResources.ContainsKey(path))
            yield break;
        else if (unforeverResources.ContainsKey(path))
            yield break;

        ResourceRequest req = Resources.LoadAsync(path);
        yield return req;

        UnityEngine.Object obj = req.asset;

        if (isforerer)
            foreverResources.Add(path, obj);
        else
            unforeverResources.Add(path, obj);

    }

  

    private static UnityEngine.Object LoadResource(string resPath)
    {
        if (foreverResources.ContainsKey(resPath))
        {
            //Debug.LogError("xxxxxxxxxxxxx " + resPath);
            return foreverResources[resPath];
        }

        if (unforeverResources.ContainsKey(resPath))
        {
            //Debug.LogError("yyyyyyyyyyyyy " + resPath);
            return unforeverResources[resPath];
        }

        UnityEngine.Object @object;
        @object = ResourceManager.LoadAssetBundleResource(resPath);

        return @object;
    }

    public static void DestroyResource(GameObject obj, bool bImmediate = false)
    {
        if (obj.CompareTag("SpawnPool"))
        {
            CPoolPrefabInsBase tag = obj.GetComponent<CPoolPrefabInsBase>();
            if (tag != null)
            {
                tag.DestroyItem();
                return;
            }
        }

        if (null != obj)
        {
            string name = obj.name;
            if (!bImmediate)
            {
                UnityEngine.Object.Destroy(obj);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            obj = null;
        }
    }

    public static GameObject InstantiateResource(Vector3 position, Quaternion rotation, string resPath)
    {
        if (resourcesObject.ContainsKey(resPath))
        {
            string filename = System.IO.Path.GetFileName(resPath);
            GameObject obj = CPoolManager.instance.GetFromPool(filename, resourcesObject[resPath]);
            obj.transform.position = position;
            obj.transform.localRotation = rotation;
            return obj;
        }

        UnityEngine.Object @object = LoadResource(resPath);

        if (!(null != @object))
        {
            return null;
        }
        GameObject object2 = (GameObject)UnityEngine.Object.Instantiate(@object, position, rotation);

        if (null == object2)
        {
            return null;
        }

        return object2;
    }

    public static GameObject InstantiateResource(string resPath)
    {
        if (resourcesObject.ContainsKey(resPath))
        {
            string filename = System.IO.Path.GetFileName(resPath);
            GameObject obj = CPoolManager.instance.GetFromPool(filename, resourcesObject[resPath]);
            return obj;
        }

        UnityEngine.Object @object = LoadResource(resPath);

        if (!(null != @object))
        {
            Debug.LogError("Resources.Load is error. resPath = " + resPath);
            return null;
        }
        GameObject object2 = (GameObject)UnityEngine.Object.Instantiate(@object);
        if (null == object2)
        {
            return null;
        }

        return object2;
    }

    //AssetBudle 

    public static Dictionary<string, UnityEngine.Object> m_AssetBundleDict = new Dictionary<string, UnityEngine.Object>();
    public static Dictionary<string, int> m_AssetBundleReferenceCount = new Dictionary<string, int>(); //GameObejct引用计数

    // 资源路径path基于_resource的相对路径
    // assetName：AssetBundle中的资源名字 mainAsset
    public static UnityEngine.Object LoadAssetBundleResource(string path,  string assetName=null)
    {
        //Debug.LogError(path);
        UnityEngine.Object asset = null;
        if (!m_AssetBundleDict.ContainsKey(path))
        {
            asset = ResourceLoader.Load(path, assetName);
            if (asset == null)
            {
                Debug.LogError(" ResourceLoader.Load is error. resPath = " + path);
            }
            m_AssetBundleDict.Add(path, asset);
        }
        //string[] depencies = AssetBundleManager.AssetBundleManifestObject.GetAllDependencies("assets/resources_/" + path + ".unity3d");

        //for (int i = 0; i < depencies.Length; i++)
        //{
        //    Debug.LogError(depencies[i]);
        //    if (!m_AssetBundleReferenceCount.ContainsKey(depencies[i]))
        //        m_AssetBundleReferenceCount.Add(depencies[i], 0);
        //}
        if (AssetBundleManager.SimulateAssetBundleInEditor == AssetBundleManager.LoadMode.AssetBundleAtPath)
        {
            if (!m_AssetBundleReferenceCount.ContainsKey(path))
            {
                m_AssetBundleReferenceCount.Add(path, 0);
            }
        }
        return m_AssetBundleDict[path];   
    }

    public static GameObject InstantiateAssetBundleResource(string path, string assetName=null)
    {
        //对接对象池
        if (resourcesObject.ContainsKey(path))
        {
            string filename = System.IO.Path.GetFileName(path);
            GameObject obj = CPoolManager.instance.GetFromPool(filename, resourcesObject[path]);
            return obj;
        }

        UnityEngine.Object @object = null;
        if(!m_AssetBundleDict.ContainsKey(path))
        {
              @object=LoadAssetBundleResource(path, assetName);
        }
        else
        {
            @object = m_AssetBundleDict[path];
        }

        GameObject object1 = (GameObject)UnityEngine.Object.Instantiate(@object);
        if (null == object1)
        {
            return null;
        }
        if (AssetBundleManager.SimulateAssetBundleInEditor == AssetBundleManager.LoadMode.AssetBundleAtPath)
        {
            m_AssetBundleReferenceCount[path]++;
        }
        return object1;
       
    }

    public static GameObject GetGameObjectFromPath(string path)
    {
        if (m_AssetBundleDict.ContainsKey(path))
            return (GameObject)m_AssetBundleDict[path];
        else
            return null;
    }

    public static IEnumerator LoadAsyncObject(string path,  string assetName=null)
    {
        if (path == null)
            yield break;

        if (m_AssetBundleDict.ContainsKey(path))
        {
            yield  break;
        }
        AssetBundleLoadAssetOperation request = ResourceLoader.LoadAsync(path, assetName);
        if (request == null)
        {
            Debug.LogError("ResourceLoader.LoadAsync is error. resPath = " + path);
            yield return null;
        }
        yield return request;
        UnityEngine.Object gameObject = request.GetAsset<UnityEngine.Object>();
        m_AssetBundleDict.Add(path, gameObject);
        if (!m_AssetBundleReferenceCount.ContainsKey(path))
        {
            m_AssetBundleReferenceCount.Add(path, 0);
        }
       
    }

    public static void LoadLevel(string assetBundleName,string levelName=null,bool isAdditive=false)
    {
        ResourceLoader.LoadLevel(assetBundleName, levelName, isAdditive);
    }

    //public static IEnumerator LoadLevelAsync(string assetBundleName, string levelName = null, bool isAdditive = false)
    //{
    //    yield return CGameObject.instance.StartCoroutine(ResourceLoader.LoadLevelAsync(assetBundleName, levelName, isAdditive));
    //}

    public static void Destroy(GameObject obj,string path, bool bImmediate = false)
    {
        //Debug.LogError(path+":Destroy");
        if (null != obj)
        {
            string name = obj.name;
            if (!bImmediate)
            {
                UnityEngine.Object.Destroy(obj);
            }
            else
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }
            obj = null;
        }
        if (AssetBundleManager.SimulateAssetBundleInEditor == AssetBundleManager.LoadMode.AssetBundleAtPath)
        {

            if (--m_AssetBundleReferenceCount[path] == 0)
            {
                ResourceLoader.UnloadAssetBundle(path);
                if (m_AssetBundleDict.ContainsKey(path))
                {
                    UnityEngine.Object objectInDict = m_AssetBundleDict[path];
                    if (objectInDict != null)
                    {
                        UnityEngine.Object.DestroyImmediate(objectInDict, true);
                    }
                    m_AssetBundleDict.Remove(path);
                }
            }
        }
       
    }
}