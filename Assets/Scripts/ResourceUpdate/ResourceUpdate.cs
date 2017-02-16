using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.IO;
using System.Xml;

public class ResourceUpdate : MonoBehaviour {


#if UNITY_IOS
    public static readonly string SERVER_IP_URL = "http://192.168.253.1:8080/";
	public static readonly string SERVER_RES_URL = SERVER_IP_URL + "/AssetBundles/iOS/";
#elif UNITY_ANDROID
    public static readonly string SERVER_IP_URL = "http://192.168.253.1:8080/";
	public static readonly string SERVER_RES_URL = SERVER_IP_URL + "/AssetBundles/Android/";
#else
    public static readonly string SERVER_IP_URL = "file:///C:";
    public static readonly string SERVER_RES_URL = SERVER_IP_URL + "/AssetBundles/Windows/";
#endif

    //本地资源路径
    public static readonly string LOCAL_RES_URL = "file:///" + ResourceLoader.relativePath;
    public static readonly string LOCAL_RES_PATH = ResourceLoader.relativePath;       

    public static readonly string VERSION_FILE = "VersionMD5.xml"; //版本文件

    private Dictionary<string, string> LocalResVersion = new Dictionary<string,string>();  //本地资源版本
    private Dictionary<string, string> ServerResVersion = new Dictionary<string, string>(); //服务器资源版本
    private List<string> downloadFileList;
    private List<string> deleteFileList;
	// Use this for initialization
	void Start () {
        //LoadRes();
        downloadFileList = new List<string>();
        deleteFileList = new List<string>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //基本测试代码，资源加载
    public void LoadRes(ResFinishDownload func)
    {
        StartCoroutine(DownLoad(SERVER_RES_URL + VERSION_FILE, delegate(WWW www)
        {

            //是否需要使用XmlDocument？？ 待考虑
            XmlDocument XmlDoc = new XmlDocument();
            XmlDoc.LoadXml(www.text);
            XmlElement XmlRoot = XmlDoc.DocumentElement;

            foreach (XmlNode node in XmlRoot.ChildNodes)
            {
                if ((node is XmlElement) == false)
                    continue;

                string file = (node as XmlElement).GetAttribute("FileName");
                string md5 = (node as XmlElement).GetAttribute("MD5");
                if (ServerResVersion.ContainsKey(file) == false)
                {
                    ServerResVersion.Add(file, md5);
                }
            }

            XmlDocument localResXMl = new XmlDocument();
            Debug.LogError("Path" + ResourceLoader.relativePath);
            localResXMl.Load(ResourceLoader.relativePath + "/AssetBundles/Windows/" + VERSION_FILE);
            XmlElement localXmlRoot = localResXMl.DocumentElement;
            foreach (XmlNode lNode in localXmlRoot.ChildNodes)
            {
                if ((lNode is XmlElement) == false)
                    continue;

                string file = (lNode as XmlElement).GetAttribute("FileName");
                string md5 = (lNode as XmlElement).GetAttribute("MD5");
                if (LocalResVersion.ContainsKey(file) == false)
                {
                    LocalResVersion.Add(file, md5);
                }
            }
            foreach(var it in ServerResVersion)
            {
                if (!LocalResVersion.ContainsKey(it.Key))
                {
                    downloadFileList.Add(it.Key);
                }
                else
                {
                    if (!it.Value.Equals(LocalResVersion[it.Key]))
                    {
                        downloadFileList.Add(it.Key);
                    }  
                }    
            }
            foreach (var it in LocalResVersion)
            {
                if (!ServerResVersion.ContainsKey(it.Key))
                {
                    deleteFileList.Add(it.Key);
                }
            }

            XmlRoot = null;
            XmlDoc = null;

            //保存xml文件
            ReplaceLocalRes("AssetBundles/" + VERSION_FILE, www.bytes);

            StartCoroutine(LoadResFromServerVersion(func));
        }
        ));


        
    }

    IEnumerator LoadResFromServerVersion(ResFinishDownload func)
    {    
        var enumerator = downloadFileList.GetEnumerator();
        while(enumerator.MoveNext())
        {
            var element = enumerator.Current;
            yield return StartCoroutine(DownLoad(SERVER_IP_URL + element, delegate(WWW www)
            {
                ReplaceLocalRes(element, www.bytes);
            }));
        }

        var enumerator1 = deleteFileList.GetEnumerator();
        while (enumerator1.MoveNext())
        {
            var element1 = enumerator1.Current;
            DeleteRes(element1);
        }
        func();
        Debug.LogError("download complete");
    }

    public delegate void ResFinishDownload();

    private void versionCompare()
    {
        readNativeFile();

        LocalResVersion = new Dictionary<string, string>(); //本地资源版本
        ServerResVersion = new Dictionary<string, string>(); //服务器资源版本
        //NeedDownFiles = new List<string>(); //需要下载的资源

        //StartCoroutine(DownLoad(@"file:///" + LOCAL_RES_PATH + VERSION_FILE, delegate(WWW localVersion)
        //{
        //    //记录本地版本信息 
        //    ParseVersionFile(localVersion.text, LocalResVersion);
        //}
        //)
        //);
    }

    /// <summary>
    /// 判断目标目录下是否存在文件
    /// </summary>
    void readNativeFile()
    {
        string versionFilePath = System.IO.Path.Combine(LOCAL_RES_PATH, VERSION_FILE); //二进制版本文件路径
        if (!File.Exists(versionFilePath))
        {
            //loadLocalData();
        }
    }

    ///// <summary>
    ///// 复制文件到外部存储
    ///// </summary>
    //private void loadLocalData()
    //{
    //    StartCoroutine(DownLoad(@"file:///" + Application.dataPath + "/StreamingAssets/" + VERSION_FILE, delegate(WWW localVersion)
    //    {
    //        ReplaceLocalRes(VERSION_FILE, localVersion.bytes);
    //    }));
    //}

    private IEnumerator DownLoad(string url, HandleFinishDownload finishFun)
    {
        WWW www = new WWW(url);
        yield return www;
        
        //if (www.error != null)
        //    Debug.LogError("url = " + url + ".  error = " + www.error);

        if (finishFun != null)
        {
            finishFun(www);
        }
        www.Dispose();
    }

    public delegate void HandleFinishDownload(WWW www);

    /// <summary>
    /// 替换本地资源
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    private void ReplaceLocalRes(string fileName, byte[] data)
    {
        string filePath = System.IO.Path.Combine(LOCAL_RES_PATH ,fileName);
        filePath = filePath.Replace("\\", "/");
        string outputPath = Path.GetDirectoryName(filePath);

        if (!Directory.Exists(outputPath))
            Directory.CreateDirectory(outputPath);

        FileStream stream = new FileStream(filePath, FileMode.Create);

        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
    }

    private void DeleteRes(string fileName)
    {
        string filePath = System.IO.Path.Combine(LOCAL_RES_PATH ,fileName);
        filePath = filePath.Replace("\\", "/");
        FileInfo fi = new FileInfo(filePath);
        if (fi.Attributes.ToString().IndexOf("ReadOnly") != -1)
            fi.Attributes = FileAttributes.Normal;
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}
