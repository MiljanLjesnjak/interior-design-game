using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class LevelSaving : MonoBehaviour
{

    /// <summary>\
    /// For end of level: "\n"
    /// Each object gets it's own line of format: name/pos/rot
    /// Objects separated by "\"
    /// </summary>
    //public void SaveLevelData()
    //{
    //    string parsed_data = "";

    //    foreach (Transform obj in GameObject.Find("Placed Objects").transform)
    //    {
    //        parsed_data += obj.name + "/" + (obj.position + Vector3.down * 40f).ToString("F3") + "/" + obj.rotation.ToString("F3");
    //        parsed_data += "\\";
    //    }

    //    parsed_data = parsed_data.Substring(0, parsed_data.Length - 1);
    //    parsed_data += "\n";


    //    File.AppendAllText("Assets/Resources/_levels.txt", parsed_data);

    //    GameObject.Find("UI Scripts").GetComponent<MenuScripts>().ReloadScene();
    //}


    //Android
    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        //Don't generate if the directory exists
        if (Directory.Exists(path))
        {
            return null;
        }
        return Directory.CreateDirectory(path);
    }

    public void SaveLevelData()
    {
        string parsed_data = "";

        foreach (Transform obj in GameObject.Find("Placed Objects").transform)
        {
            parsed_data += obj.name + "/" + (obj.position + Vector3.down * 40f).ToString("F3") + "/" + obj.rotation.ToString("F3");
            parsed_data += "\\";
        }

        parsed_data = parsed_data.Substring(0, parsed_data.Length - 1);
        parsed_data += "\n";

        //string path = Application.persistentDataPath + "/level_data";
        //SafeCreateDirectory(path);

        string path = "Assets/Resources/_levels.txt";
        File.AppendAllText(path, parsed_data);


        //Debug.LogError(path);

        //StreamWriter Writer = new StreamWriter(path + "/_levels.txt", append: true);
        //Writer.Write(parsed_data);
        //Writer.Flush();
        //Writer.Close();

        GameObject.Find("UI Scripts").GetComponent<MenuScripts>().ReloadScene();
    }


}
