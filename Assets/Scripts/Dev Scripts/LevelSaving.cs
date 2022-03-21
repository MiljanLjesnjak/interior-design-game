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
    public void SaveLevelData()
    {
        string parsed_data = "";

        foreach (Transform obj in GameObject.Find("Prefabs Play").transform)
        {
            parsed_data += obj.name + "/" + (obj.position + Vector3.down * 40f).ToString("F3") + "/" + obj.rotation.ToString("F3");
            parsed_data += "\\";
        }

        parsed_data = parsed_data.Substring(0, parsed_data.Length - 1);
        parsed_data += "\n";

        File.AppendAllText(Application.dataPath + "/levels.txt", parsed_data);
    }

}
