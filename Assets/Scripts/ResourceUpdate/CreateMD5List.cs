using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

public class CreateMD5List {

    /// <summary>
    /// 将此路径下的所有文件都生成MD5文件
    /// </summary>
    /// <param name="path"></param>
    public static void Execute(string path)
    {
        Debug.LogError("Execute path = " + path);
        Dictionary<string, string> DicFileMD5 = new Dictionary<string, string>();
        MD5CryptoServiceProvider md5Generator = new MD5CryptoServiceProvider();

        FileExecute(path, DicFileMD5, md5Generator);

        XmlDocument XmlDoc = new XmlDocument();
        XmlElement XmlRoot = XmlDoc.CreateElement("Files");
        XmlDoc.AppendChild(XmlRoot);
        foreach (KeyValuePair<string, string> pair in DicFileMD5)
        {
            XmlElement xmlElem = XmlDoc.CreateElement("File");
            XmlRoot.AppendChild(xmlElem);

            xmlElem.SetAttribute("FileName", pair.Key);
            xmlElem.SetAttribute("MD5", pair.Value);
        }

        XmlDoc.Save(path + "/VersionMD5.xml");
        XmlDoc = null;
    }

    public static void FileExecute(string path, Dictionary<string, string> DicFileMD5, MD5CryptoServiceProvider md5Generator)
    {
        foreach (string filePath in Directory.GetFileSystemEntries(path))
        {
            //if (filePath.Contains("VersionMD5") || filePath.Contains(".xml"))
            //    continue;
            if (Directory.Exists( filePath ) ) //文件夹
            {
                FileExecute(filePath, DicFileMD5, md5Generator);
                continue;
            }

            FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] hash = md5Generator.ComputeHash(file);
            string strMD5 = System.BitConverter.ToString(hash);
            file.Close();

            //string key = filePath.Substring(path.Length + 1, filePath.Length - path.Length - 1);
            string key = filePath.Replace("\\", "/"); ;

            if (DicFileMD5.ContainsKey(key) == false)
                DicFileMD5.Add(key, strMD5);
            else
                Debug.LogWarning("<Two File has the same name> name = " + filePath);
        }
    }
}
