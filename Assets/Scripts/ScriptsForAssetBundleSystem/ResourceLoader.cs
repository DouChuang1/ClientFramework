using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR	
using UnityEditor;
#endif



public class ResourceLoader : MonoBehaviour
{
    public static string relativePath = 
#if UNITY_EDITOR
        System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
#else
        "mnt/sdcard/loaderTest";
        //return Application.persistentDataPath;
#endif

    static string loaderPath = "assets/resources_/";

    public AssetBundleManager.LoadMode simulateAssetBundleInEditor = AssetBundleManager.LoadMode.LoadAssetAtPath;


	// Use this for initialization.
	void Start ()
	{
		//yield return StartCoroutine(Initialize() );
	}

	// Initialize the downloading url and AssetBundleManifest object.
	public IEnumerator Initialize()
	{
		// Don't destroy the game object as we base on it to run the loading script.
		DontDestroyOnLoad(gameObject);
		
#if UNITY_EDITOR
        AssetBundleManager.SimulateAssetBundleInEditor = simulateAssetBundleInEditor;
		Debug.Log ("We are " + (AssetBundleManager.SimulateAssetBundleInEditor) );
#endif

#if UNITY_EDITOR
		string platformFolderForAssetBundles = GetPlatformFolderForAssetBundles(EditorUserBuildSettings.activeBuildTarget);
        AssetBundleManager.BaseDownloadingURL = "file://" + relativePath + "/AssetBundles/" + platformFolderForAssetBundles + "/";
#else
		string platformFolderForAssetBundles =  GetPlatformFolderForAssetBundles(Application.platform);
        AssetBundleManager.BaseDownloadingURL = "file:////" + relativePath + "/AssetBundles/" + platformFolderForAssetBundles + "/";
#endif

        AssetBundleManager.BaseDownloadingPath = relativePath + "/AssetBundles/" + platformFolderForAssetBundles + "/";

        Debug.LogError("Enter Init Manifest");
		// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
		var request = AssetBundleManager.Initialize(platformFolderForAssetBundles);
		if (request != null)
			yield return StartCoroutine(request);
	}



#if UNITY_EDITOR
	public static string GetPlatformFolderForAssetBundles(BuildTarget target)
	{
		switch(target)
		{
		case BuildTarget.Android:
			return "Android";
		case BuildTarget.iOS:
			return "iOS";
		case BuildTarget.WebPlayer:
			return "WebPlayer";
		case BuildTarget.StandaloneWindows:
		case BuildTarget.StandaloneWindows64:
			return "Windows";
		case BuildTarget.StandaloneOSXIntel:
		case BuildTarget.StandaloneOSXIntel64:
		case BuildTarget.StandaloneOSXUniversal:
			return "OSX";
			// Add more build targets for your own.
			// If you add more targets, don't forget to add the same platforms to GetPlatformFolderForAssetBundles(RuntimePlatform) function.
		default:
			return null;
		}
	}
#endif

	static string GetPlatformFolderForAssetBundles(RuntimePlatform platform)
	{
		switch(platform)
		{
		case RuntimePlatform.Android:
			return "Android";
		case RuntimePlatform.IPhonePlayer:
			return "iOS";
		case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.OSXWebPlayer:
			return "WebPlayer";
		case RuntimePlatform.WindowsPlayer:
			return "Windows";
		case RuntimePlatform.OSXPlayer:
			return "OSX";
			// Add more build platform for your own.
			// If you add more platforms, don't forget to add the same targets to GetPlatformFolderForAssetBundles(BuildTarget) function.
		default:
			return null;
		}
	}

    public static IEnumerator LoadLevelAsync(string assetBundleName, string levelName = null, bool isAdditive = false)
	{
		Debug.Log("Start to load scene " + levelName + " at frame " + Time.frameCount);

        assetBundleName = "assets/scenes/" + assetBundleName;

        //获取assetName
        if (levelName == null)
            levelName = Path.GetFileName(assetBundleName);
#if UNITY_EDITOR
        if (AssetBundleManager.SimulateAssetBundleInEditor == AssetBundleManager.LoadMode.LoadAssetAtPath)
        {
            AsyncOperation m_sceneLoader;
            if (isAdditive)
            {
                m_sceneLoader = SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive);                
            }
            else
            {
                m_sceneLoader = SceneManager.LoadSceneAsync(levelName);  
            }
            yield return m_sceneLoader;
        }
        else
#endif
        {
            assetBundleName = assetBundleName + ".unity3d";

            // Load level from assetBundle.
            AssetBundleLoadOperation request = AssetBundleManager.LoadLevelAsync(assetBundleName, levelName, isAdditive);
            if (request == null)
            {
                Debug.LogError("null");
                yield break; 
            }
               
            yield return request;
        }
		// This log will only be output when loading level additively.
		Debug.Log("Finish loading scene " + levelName + " at frame " + Time.frameCount);
	}

    public static void LoadLevel(string assetBundleName, string levelName = null, bool isAdditive = false)
    {
        Debug.Log("Start to load scene " + levelName + " at frame " + Time.frameCount);

        assetBundleName = "assets/scenes/" + assetBundleName;  //Todo 资源路径

        //获取assetName
        if (levelName == null)
            levelName = Path.GetFileName(assetBundleName);
#if UNITY_EDITOR
        if (AssetBundleManager.SimulateAssetBundleInEditor == AssetBundleManager.LoadMode.LoadAssetAtPath)
        {
            if (isAdditive)
            {
                SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
            }
            else
            {
                SceneManager.LoadScene(levelName);
            }
        }
        else
#endif
        {
            assetBundleName = assetBundleName + ".unity3d";

            AssetBundleManager.LoadLevel(assetBundleName, levelName, isAdditive);

        }
        // This log will only be output when loading level additively.
        Debug.Log("Finish loading scene " + levelName + " at frame " + Time.frameCount);
    }
    /// <summary>
    /// 同步加载资源
    /// </summary>
    /// <param name="path"> 相对于Resources_的路径 </param>
    /// <param name="assetName"> 预制体名称 </param>
    /// <returns></returns>
    public static Object Load(string path, string assetName = null)
    {
        //先合成assetBundleName
        string assetBundleName = loaderPath + path + ".unity3d";

        //获取assetName
        if (assetName == null)
            assetName = Path.GetFileName(path);
        
        return AssetBundleManager.LoadAsset( assetBundleName, assetName);
    }

    /// <summary>
    /// 异步加载资源
    /// </summary>
    /// <param name="path"> 相对于Resources_的路径 </param>
    /// <param name="assetName"> 预制体名称 </param>
    /// <returns></returns>
    public static AssetBundleLoadAssetOperation LoadAsync(string path, string assetName = null)
    {
        //先合成assetBundleName
        string assetBundleName = loaderPath + path + ".unity3d";

        //获取assetName
        if (assetName == null)
            assetName = Path.GetFileName(path);

        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject));
        return request;     
    }

    public static void UnloadAssetBundle(string path)
    {
        //先合成assetBundleName
        string assetBundleName = loaderPath + path + ".unity3d";

        Debug.LogError(Path.GetFullPath(loaderPath));
        AssetBundleManager.UnloadAssetBundle(assetBundleName);
    }
}
